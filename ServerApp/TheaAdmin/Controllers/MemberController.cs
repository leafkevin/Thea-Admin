using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheaAdmin.Domain.Models;
using TheaAdmin.Dtos;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Thea;
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
    public async Task<TheaResponse> QueryPage(QueryPageRequest request)
    {
        using var repository = this.dbFactory.Create();
        var result = await repository.From<Member>()
            .Where(!string.IsNullOrEmpty(request.QueryText), f => f.Mobile.Contains(request.QueryText)
                || f.MemberName.Contains(request.QueryText))
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
    [HttpPost]
    public async Task<TheaResponse> Create(MemberRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberName))
            return TheaResponse.Fail(1, $"��Ա��������Ϊ��");
        if (string.IsNullOrEmpty(request.Mobile))
            return TheaResponse.Fail(1, $"�ֻ����벻��Ϊ��");

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
            return TheaResponse.Fail(2, $"����ʧ�ܣ�������");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Modify(MemberRequest request)
    {
        if (string.IsNullOrEmpty(request.MemberId))
            return TheaResponse.Fail(1, $"��ԱID����Ϊ��");
        if (string.IsNullOrEmpty(request.MemberName))
            return TheaResponse.Fail(1, $"��Ա��������Ϊ��");
        if (string.IsNullOrEmpty(request.Mobile))
            return TheaResponse.Fail(1, $"�ֻ����벻��Ϊ��");

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
            return TheaResponse.Fail(2, $"����ʧ�ܣ�������");
        return TheaResponse.Succeed(result);
    }
    [HttpPost]
    public async Task<TheaResponse> Delete(IdRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
            return TheaResponse.Fail(1, $"��ԱID����Ϊ��");

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
            return TheaResponse.Fail(2, $"����ʧ�ܣ�������");
        return TheaResponse.Succeed(result);
    }
}
