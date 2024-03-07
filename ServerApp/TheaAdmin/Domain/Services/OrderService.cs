using MySalon.Domain.Models;
using System.Threading.Tasks;
using System;
using Thea;
using Trolley;

namespace MySalon.Domain.Services;

public class OrderService
{
    private readonly IOrmDbFactory dbFactory;

    public OrderService(IOrmDbFactory dbFactory)
    {
        this.dbFactory = dbFactory;
    }
    public async Task<TheaResponse> Create(string memberId, double amount, string stylistId, string description, bool isAppointed, string createdBy)
    {
        var orderId = ObjectId.NewId();
        using var repository = this.dbFactory.Create();
        Member memberInfo = null;
        if (!string.IsNullOrEmpty(memberId))
        {
            memberInfo = await repository.GetAsync<Member>(memberId);
            if (memberInfo.Balance < amount)
                return TheaResponse.Fail(1, $"会员账户余额不足，还剩{memberInfo.Balance:N2}元");
            memberInfo.Balance -= amount;
        }
        int result = 0;
        try
        {
            await repository.BeginTransactionAsync();
            result = await repository.CreateAsync<Order>(new
            {
                OrderId = orderId,
                MemberId = memberId ?? "anonym",
                Amount = amount,
                Description = description,
                IsAppointed = isAppointed,
                Status = DataStatus.Active,
                StylistId = stylistId,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy,
                UpdatedAt = DateTime.Now,
                UpdatedBy = createdBy
            });
            if (memberInfo != null)
            {
                await repository.UpdateAsync<Member>(new
                {
                    memberInfo.MemberId,
                    memberInfo.Balance,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = createdBy
                });
            }
            await repository.CommitAsync();
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync();
            return TheaResponse.Fail(-1, $"操作失败，请重试，Detail:{ex.Message}");
        }
        if (result <= 0)
            return TheaResponse.Fail(1, $"操作失败，请重试");
        return TheaResponse.Success;
    }
}
