using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thea;
using Thea.ExcelExporter;
using TheaAdmin.Domain;
using TheaAdmin.Domain.Models;
using TheaAdmin.Dtos;
using Trolley;

namespace TheaAdmin.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class MemberController : ControllerBase
{
    private readonly IOrmDbFactory dbFactory;

    public MemberController(IOrmDbFactory dbFactory)
    {
        this.dbFactory = dbFactory;
    }
    [HttpPost]
    public async Task<TheaResponse> QueryPage([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Domain.Models.Member>()
            .Where(f => f.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), f => f.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), f => f.Mobile.Contains(request.Mobile))
            .Select(f => new
            {
                f.MemberId,
                f.MemberName,
                f.Mobile,
                f.Gender,
                f.Balance,
                f.Description,
                f.CreatedAt
            })
            .Page(request.PageIndex, request.PageSize)
            .OrderByDescending(f => f.MemberId)
            .ToPageListAsync();
        return TheaResponse.Succeed(result);
    }
    [HttpGet]
    public async Task<TheaResponse> Detail([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return TheaResponse.Fail(1, $"会员ID不能为空");

        using var repository = this.dbFactory.Create();
        var result = await repository.From<Domain.Models.Member>()
            .Where(f => f.MemberId == id)
            .Select(f => new
            {
                f.MemberId,
                f.MemberName,
                f.Mobile,
                f.Gender,
                f.Balance,
                f.Description
            })
            .FirstAsync();
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Create([FromBody] MemberRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberName))
            return TheaResponse.Fail(1, $"会员姓名不能为空");
        if (string.IsNullOrEmpty(request.Mobile))
            return TheaResponse.Fail(1, $"手机号码不能为空");

        using var repository = this.dbFactory.Create();
        var passport = this.User.ToPassport();
        var operatorId = passport.UserId;
        var isExists = await repository.ExistsAsync<Domain.Models.Member>(new { Mobile = request.Mobile });
        if (isExists) return TheaResponse.Fail(1, $"手机号码[{request.Mobile}]已注册为会员");

        var result = await repository.CreateAsync<Domain.Models.Member>(new
        {
            MemberId = ObjectId.NewId(),
            request.MemberName,
            request.Mobile,
            request.Gender,
            request.Description,
            request.Balance,
            Status = 1,
            CreatedAt = DateTime.Now,
            CreatedBy = operatorId,
            UpdatedAt = DateTime.Now,
            UpdatedBy = operatorId
        });
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Modify([FromBody] MemberRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberId))
            return TheaResponse.Fail(1, $"会员ID不能为空");
        if (string.IsNullOrEmpty(request.MemberName))
            return TheaResponse.Fail(1, $"会员姓名不能为空");
        if (string.IsNullOrEmpty(request.Mobile))
            return TheaResponse.Fail(1, $"手机号码不能为空");

        using var repository = this.dbFactory.Create();
        var passport = this.User.ToPassport();
        var operatorId = passport.UserId;
        var result = await repository.UpdateAsync<Domain.Models.Member>(new
        {
            request.MemberId,
            request.MemberName,
            request.Mobile,
            request.Gender,
            request.Description,
            request.Balance,
            UpdatedAt = DateTime.Now,
            UpdatedBy = operatorId
        });
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Delete([FromBody] IdRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
            return TheaResponse.Fail(1, $"会员ID不能为空");

        using var repository = this.dbFactory.Create();
        var passport = this.User.ToPassport();
        var operatorId = passport.UserId;
        var result = await repository.UpdateAsync<Domain.Models.Member>(new
        {
            MemberId = request.Id,
            Status = 2,
            UpdatedAt = DateTime.Now,
            UpdatedBy = operatorId
        });
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> BatchDelete([FromBody] IdsRequest request)
    {
        if (request.Ids == null || request.Ids.Count == 0)
            return TheaResponse.Fail(1, $"至少要选择一个会员，才能批量删除");

        var passport = this.User.ToPassport();

        using var repository = this.dbFactory.Create();
        var entities = request.Ids.Select(f => new
        {
            MemberId = f,
            Status = 2,
            UpdatedAt = DateTime.Now,
            UpdatedBy = passport.UserId
        });
        var result = await repository.UpdateAsync<Domain.Models.Member>(entities);
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Import()
    {
        if (this.Request.Form.Files.Count <= 0)
            return TheaResponse.Fail(1, "未上传任何文件");
        var formFile = this.Request.Form.Files[0];
        var stream = new MemoryStream();
        await formFile.CopyToAsync(stream);
        var importMembers = stream.Query<MemberImportRequest>().ToList();
        var mobiles = importMembers.Select(f => f.Mobile).ToList();

        using var repository = this.dbFactory.Create();
        var existsMobiles = await repository.From<Domain.Models.Member>()
            .Where(f => f.Status == DataStatus.Active && mobiles.Contains(f.Mobile))
            .Select(f => f.Mobile)
            .ToListAsync();
        if (existsMobiles.Count > 0)
        {
            foreach (var existsMobile in existsMobiles)
            {
                var removeMember = importMembers.Find(f => f.Mobile == existsMobile);
                importMembers.Remove(removeMember);
            }
        }
        var passport = this.User.ToPassport();
        var members = importMembers.Select(f => new Domain.Models.Member
        {
            MemberId = ObjectId.NewId(),
            MemberName = f.MemberName,
            Balance = f.Balance,
            Gender = f.Gender,
            Description = f.Description,
            Mobile = f.Mobile,
            Status = DataStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = passport.UserId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = passport.UserId
        });
        var count = await repository.CreateAsync<Domain.Models.Member>(members);
        if (count <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Success;
    }
    [HttpPost]
    public async Task<FileStreamResult> Export([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Domain.Models.Member>()
            .Where(f => f.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), f => f.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), f => f.Mobile.Contains(request.Mobile))
            .Select(f => new
            {
                f.MemberId,
                f.MemberName,
                f.Mobile,
                f.Gender,
                f.Balance,
                f.Description,
                f.CreatedAt
            })
            .ToListAsync();

        var genderDecorator = (object data) =>
           (Gender)data switch
           {
               Gender.Male => "男性",
               Gender.Female => "女性",
               _ => "未知"
           };
        var stream = new MemoryStream();
        var builder = new ExcelExporterBuilder().WithData(result);
        await builder.AddColumnHeader(f => f.Field(t => t.MemberId).Title("会员ID").Width(26.63))
            .AddColumnHeader(f => f.Field(t => t.MemberName).Title("会员名称").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Mobile).Title("手机号").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Gender).Title("性别").Decorator(genderDecorator).Width(6.38).Horizontal(CellHorizontalAlignment.Center))
            .AddColumnHeader(f => f.Field(t => t.Balance).Title("余额").Format("&quot;¥&quot;#,##0.00;&quot;¥&quot;\\-#,##0.00").Width(13.13))
            .AddColumnHeader(f => f.Field(t => t.Description).Title("描述").Width(35.25))
            .AddColumnHeader(f => f.Field(t => t.CreatedAt).Title("注册时间").Width(21).Format("yyyy-MM-dd HH:mm:ss").Horizontal(CellHorizontalAlignment.Center))
            .Export(stream);
        stream.Position = 0;
        return this.File(stream, "application/vnd.ms-excel");
    }
}
