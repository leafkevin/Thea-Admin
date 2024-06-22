using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thea;
using Thea.ExcelExporter;
using TheaAdmin.Domain;
using TheaAdmin.Domain.Models;
using TheaAdmin.Domain.Services;
using TheaAdmin.Dtos;
using Trolley;
using Trolley.MySqlConnector;

namespace TheaAdmin.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class DepositController : ControllerBase
{
    private readonly IOrmDbFactory dbFactory;
    private readonly DepositService depositService;

    public DepositController(DepositService depositService, IOrmDbFactory dbFactory)
    {
        this.depositService = depositService;
        this.dbFactory = dbFactory;
    }
    [HttpPost]
    public async Task<TheaResponse> QueryPage([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Domain.Models.Member, Deposit>()
            .InnerJoin((a, b) => a.MemberId == b.MemberId)
            .Where((a, b) => a.Status == DataStatus.Active && b.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), (a, b) => a.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), (a, b) => a.Mobile.Contains(request.Mobile))
            .OrderByDescending((a, b) => b.CreatedAt)
            .SelectFlattenTo((a, b) => new DepositResponse
            {
                MemberId = a.MemberId,
                Description = b.Description
            })
            .Page(request.PageIndex, request.PageSize)
            .ToPageListAsync();
        var memberIds = result.Data.Select(f => f.MemberId).ToList();
        var lastDepositIds = await repository.From<Deposit>()
            .WithTable(f => f.From<Deposit>()
                .Where(t => memberIds.Contains(t.MemberId) && t.Status == DataStatus.Active)
                .GroupBy(t => t.MemberId)
                .Select((x, a) => new { a.MemberId, CreatedAt = x.Max(a.CreatedAt) }))
            .InnerJoin((a, b) => a.MemberId == b.MemberId && a.CreatedAt == b.CreatedAt)
            .Select((a, b) => a.DepositId)
            .ToListAsync();
        foreach (var item in result.Data)
        {
            item.IsAllowCancel = lastDepositIds.Contains(item.DepositId);
        }
        return TheaResponse.Succeed(result);
    }
    [HttpGet]
    public async Task<TheaResponse> Detail([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return TheaResponse.Fail(1, $"充值ID不能为空");

        using var repository = this.dbFactory.Create();
        var result = await repository.From<Deposit, Domain.Models.Member>()
            .InnerJoin((a, b) => a.MemberId == b.MemberId)
            .Select((a, b) => new
            {
                a.DepositId,
                a.MemberId,
                b.MemberName,
                b.Mobile,
                a.Amount,
                a.BeginBalance,
                a.Bonus,
                a.EndBalance,
                a.Description,
                a.CreatedAt
            })
            .FirstAsync();
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Create(DepositRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberId))
            return TheaResponse.Fail(1, $"会员Id不能为空");
        if (request.Amount <= 0)
            return TheaResponse.Fail(1, $"充值金额不能必须大于0");

        var passport = this.User.ToPassport();
        return await this.depositService.Create(request.MemberId, request.Amount, request.Bonus, request.Description, passport.UserId);
    }
    [HttpPost]
    public async Task<TheaResponse> Modify([FromBody] DepositRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberId))
            return TheaResponse.Fail(1, $"会员ID不能为空");
        if (string.IsNullOrEmpty(request.DepositId))
            return TheaResponse.Fail(1, $"充值ID不能为空");
        if (request.Amount <= 0)
            return TheaResponse.Fail(1, $"充值金额>=0");
        var passport = this.User.ToPassport();
        return await this.depositService.Modify(request.DepositId, request.MemberId, request.Amount, request.Bonus, request.Description, passport.UserId);
    }
    [HttpPost]
    public async Task<TheaResponse> Cancel([FromBody] IdRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
            return TheaResponse.Fail(1, $"充值ID不能为空");

        var passport = this.User.ToPassport();
        return await this.depositService.Cancel(request.Id, passport.UserId);
    }
    [HttpPost]
    public async Task<FileStreamResult> Export([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Domain.Models.Member, Deposit>()
            .InnerJoin((a, b) => a.MemberId == b.MemberId)
            .Where((a, b) => a.Status == DataStatus.Active && b.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), (a, b) => a.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), (a, b) => a.Mobile.Contains(request.Mobile))
            .OrderByDescending((a, b) => b.CreatedAt)
            .Select((a, b) => new
            {
                b.DepositId,
                a.MemberId,
                a.MemberName,
                a.Mobile,
                b.BeginBalance,
                b.Amount,
                b.Bonus,
                b.EndBalance,
                b.Description,
                b.CreatedAt
            })
            .ToListAsync();
        var stream = new MemoryStream();
        var builder = new ExcelExporterBuilder().WithData(result);
        await builder.AddColumnHeader(f => f.Field(t => t.MemberId).Title("会员ID").Width(26.63))
            .AddColumnHeader(f => f.Field(t => t.MemberName).Title("会员名称").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Mobile).Title("手机号").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.BeginBalance).Title("充值前余额").Format("&quot;¥&quot;#,##0.00;&quot;¥&quot;\\-#,##0.00").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Amount).Title("充值金额").Format("&quot;¥&quot;#,##0.00;&quot;¥&quot;\\-#,##0.00").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Bonus).Title("赠送金额").Format("&quot;¥&quot;#,##0.00;&quot;¥&quot;\\-#,##0.00").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.EndBalance).Title("充值后余额").Format("&quot;¥&quot;#,##0.00;&quot;¥&quot;\\-#,##0.00").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Description).Title("描述").Width(35.25))
            .AddColumnHeader(f => f.Field(t => t.CreatedAt).Title("充值日期").Width(21).Format("yyyy-MM-dd HH:mm:ss").Horizontal(CellHorizontalAlignment.Center))
            .Export(stream);
        stream.Position = 0;
        return this.File(stream, "application/vnd.ms-excel");
    }
}
