using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySalon.Domain.Models;
using MySalon.Domain.Services;
using MySalon.Dtos;
using System.Threading.Tasks;
using Thea;
using Trolley;

namespace MySalon.Controllers;

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
    public async Task<TheaResponse> QueryPage(QueryPageRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
            .LeftJoin<Deposit>((a, b) => a.MemberId == b.MemberId)
            .Where(!string.IsNullOrEmpty(request.QueryText), (a, b) => a.Mobile.Contains(request.QueryText)
                || a.MemberName.Contains(request.QueryText))
            .GroupBy((a, b) => new { a.MemberId, a.MemberName, a.Mobile, a.Balance })
            .Select((x, a, b) => new
            {
                a.MemberId,
                a.MemberName,
                a.Mobile,
                a.Balance,
                DepositTimes = x.Count(b.DepositId)
            })
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
