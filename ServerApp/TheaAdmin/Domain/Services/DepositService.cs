using MySalon.Domain.Models;
using MySalon.Dtos;
using System.Threading.Tasks;
using System;
using Thea;
using Trolley;

namespace MySalon.Domain.Services;

public class DepositService
{
    private readonly IOrmDbFactory dbFactory;

    public DepositService(IOrmDbFactory dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public async Task<TheaResponse> Create(string memberId, double amount, double bonus, string description, string operatorId)
    {
        if (string.IsNullOrEmpty(memberId))
            return TheaResponse.Fail(1, $"会员Id不能为空");
        if (amount <= 0)
            return TheaResponse.Fail(1, $"充值金额不能必须大于0");

        using var repository = this.dbFactory.Create();  
        var memberInfo = await repository.GetAsync<Member>(memberId);
        var depositAmount = amount + bonus;

        int result = 0;
        try
        {
            await repository.BeginTransactionAsync();
            result = await repository.CreateAsync<Deposit>(new Deposit
            {
                DepositId = ObjectId.NewId(),
                MemberId =memberId,
                Amount = amount,
                BeginBalance = memberInfo.Balance,
                EndBalance = memberInfo.Balance + depositAmount,
                Bonus = bonus,
                Description = description,
                Status = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = operatorId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = operatorId
            });
            result += await repository.Update<Member>()
                .Set(new
                {
                    Balance = memberInfo.Balance + depositAmount,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = operatorId
                })
                .ExecuteAsync();
            await repository.CommitAsync();
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync();
            return TheaResponse.Fail(-1, $"操作失败，请重试，Detail:{ex.Message}");
        }
        if (result <= 0)
            return TheaResponse.Fail(2, $"操作失败，请重试");
        return TheaResponse.Success;
    }
}
