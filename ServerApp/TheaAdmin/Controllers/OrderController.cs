using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thea;
using TheaAdmin.Domain.Models;
using TheaAdmin.Domain.Services;
using TheaAdmin.Dtos;
using Trolley;
using Trolley.MySqlConnector;

namespace TheaAdmin.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class OrderController : ControllerBase
{
    private readonly IOrmDbFactory dbFactory;
    private readonly OrderService orderService;

    public OrderController(IOrmDbFactory dbFactory, OrderService orderService)
    {
        this.dbFactory = dbFactory;
        this.orderService = orderService;
    }
    [HttpGet]
    public async Task<TheaResponse> QueryPage(QueryPageRequest request)
    {
        if (string.IsNullOrEmpty(request.QueryText))
            return TheaResponse.Fail(1, $"查询文本{request.QueryText}不能为空");

        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
            .Where(f => f.Mobile.Contains(request.QueryText) || f.MemberName.Contains(request.QueryText))
            .Page(request.PageIndex, request.PageSize)
            .Select(f => new
            {
                f.MemberId,
                f.MemberName,
                f.Mobile,
                f.Balance
            })
            .ToPageListAsync();
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Create([FromBody] CreateOrderRequest request)
    {
        if (request.Amount <= 0)
            return TheaResponse.Fail(1, $"消费金额{request.Amount}必须大于等于0");

        var passport = this.User.ToPassport();
        return await this.orderService.Create(request.MemberId, request.Amount, request.StylistId, request.Description, request.IsAppointed, passport.UserId);
    }
    [HttpGet]
    public async Task<TheaResponse> Detail(string orderId)
    {
        if (string.IsNullOrEmpty(orderId))
            return TheaResponse.Fail(1, $"订单ID{orderId}不能为空");

        using var repository = this.dbFactory.Create();
        var result = await repository.GetAsync<Order>(orderId);
        if (result == null)
            return TheaResponse.Fail(1, $"订单ID{orderId}不存在");
        return TheaResponse.Succeed(result);
    }
}
