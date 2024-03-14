using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thea;
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
        var result = await repository.From<Member>()
            .Where(f => f.Status == DataStatus.Active)
            .And(!string.IsNullOrEmpty(request.MemberName), f => f.MemberName.Contains(request.MemberName))
            .And(!string.IsNullOrEmpty(request.Mobile), f => f.Mobile.Contains(request.Mobile))
            .Page(request.PageIndex, request.PageSize)
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
            .ToPageListAsync();
        return TheaResponse.Succeed(result);
    }
    [HttpGet]
    public async Task<TheaResponse> Detail([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return TheaResponse.Fail(1, $"会员ID不能为空");

        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
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
        var result = await repository.CreateAsync<Member>(new
        {
            MemberId = ObjectId.NewId(),
            MemberName = request.MemberName,
            Mobile = request.Mobile,
            Gender = request.Gender,
            Description = request.Description,
            Balance = request.Balance,
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
        var result = await repository.UpdateAsync<Member>(new
        {
            MemberId = request.MemberId,
            MemberName = request.MemberName,
            Mobile = request.Mobile,
            Gender = request.Gender,
            Description = request.Description,
            Balance = request.Balance,
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
        var result = await repository.UpdateAsync<Member>(new
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
        var result = await repository.UpdateAsync<Member>(entities);
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Import([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
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
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Export([FromBody] MemberQueryRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
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
        return TheaResponse.Succeed(result);
    }
}
