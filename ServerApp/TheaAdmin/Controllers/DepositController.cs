using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thea;
using TheaAdmin.Domain;
using TheaAdmin.Domain.Models;
using TheaAdmin.Domain.Services;
using TheaAdmin.Dtos;
using Trolley;

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
        var result = await repository.From<Member>()
            .LeftJoin<Deposit>((a, b) => a.MemberId == b.MemberId)
            .Where((a, b) => a.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), (a, b) => a.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), (a, b) => a.Mobile.Contains(request.Mobile))
            .GroupBy((a, b) => new { a.MemberId, a.MemberName, a.Mobile, a.Balance })
            .Select((x, a, b) => new
            {
                a.MemberId,
                a.MemberName,
                a.Mobile,
                a.Balance,
                DepositTimes = x.Count(b.DepositId).IsNull(0),
                LastDepositedAt = x.Max(b.CreatedAt).IsNull(a.CreatedAt)
            })
            .OrderByDescending(f => f.LastDepositedAt)
            .Page(request.PageIndex, request.PageSize)
            .ToPageListAsync();
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
}
