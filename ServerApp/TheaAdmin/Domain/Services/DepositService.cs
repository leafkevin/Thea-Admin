using MiniExcelLibs;
using System;
using System.Threading.Tasks;
using Thea;
using TheaAdmin.Domain.Models;
using Trolley;

namespace TheaAdmin.Domain.Services;

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
        var memberInfo = await repository.GetAsync<Domain.Models.Member>(memberId);
        var depositAmount = amount + bonus;

        int result = 0;
        try
        {
            await repository.BeginTransactionAsync();
            result = await repository.CreateAsync<Deposit>(new Deposit
            {
                DepositId = ObjectId.NewId(),
                MemberId = memberId,
                Amount = amount,
                BeginBalance = memberInfo.Balance,
                EndBalance = memberInfo.Balance + depositAmount,
                Bonus = bonus,
                Description = description,
                Status = DataStatus.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = operatorId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = operatorId
            });
            result += await repository.Update<Domain.Models.Member>()
                .Set(new
                {
                    Balance = memberInfo.Balance + depositAmount,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = operatorId
                })
                .Where(f => f.MemberId == memberId)
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
    public async Task<TheaResponse> Modify(string depositId, string memberId, double amount, double bonus, string description, string operatorId)
    {
        if (string.IsNullOrEmpty(depositId))
            return TheaResponse.Fail(1, $"充值ID不能为空");
        if (string.IsNullOrEmpty(memberId))
            return TheaResponse.Fail(1, $"会员ID不能为空");
        if (amount <= 0)
            return TheaResponse.Fail(1, $"充值金额必须>=0");

        int result = 0;
        using var repository = this.dbFactory.Create();
        try
        {
            var myLastDeposit = await repository.From<Deposit>()
                .Where(f => f.MemberId == memberId && f.Status == DataStatus.Active)
                .OrderByDescending(f => f.CreatedAt)
                .Take(1)
                .FirstAsync();
            if (myLastDeposit.DepositId != depositId)
                return TheaResponse.Fail(2, "本次充值已经无法修复，只能修改最后一笔充值记录");
            var depositAmount = amount + bonus;

            await repository.BeginTransactionAsync();
            result = await repository.UpdateAsync<Deposit>(new
            {
                DepositId = depositId,
                MemberId = memberId,
                Amount = amount,
                EndBalance = myLastDeposit.BeginBalance + depositAmount,
                Bonus = bonus,
                Description = description,
                UpdatedAt = DateTime.Now,
                UpdatedBy = operatorId
            });
            result += await repository.Update<Domain.Models.Member>()
                .Set(new
                {
                    Balance = myLastDeposit.BeginBalance + depositAmount,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = operatorId
                })
                .Where(f => f.MemberId == memberId)
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
    public async Task<TheaResponse> Cancel(string depositId, string operatorId)
    {
        if (string.IsNullOrEmpty(depositId))
            return TheaResponse.Fail(1, $"充值ID不能为空");

        int result = 0;
        using var repository = this.dbFactory.Create();
        try
        {
            var myDeposit = await repository.GetAsync<Deposit>(depositId);
            var myLastDeposit = await repository.From<Deposit>()
                .Where(f => f.MemberId == myDeposit.MemberId && f.Status == DataStatus.Active)
                .OrderByDescending(f => f.CreatedAt)
                .Take(1)
                .FirstAsync();
            if (myLastDeposit.DepositId != depositId)
                return TheaResponse.Fail(2, "本次充值已经无法撤销，只能撤销最后一笔充值记录");

            await repository.BeginTransactionAsync();
            result = await repository.UpdateAsync<Deposit>(new
            {
                DepositId = depositId,
                Status = DataStatus.Deleted,
                UpdatedAt = DateTime.Now,
                UpdatedBy = operatorId
            });
            result += await repository.Update<Domain.Models.Member>()
                .Set(new
                {
                    Balance = myLastDeposit.BeginBalance,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = operatorId
                })
                .Where(f => f.MemberId == myDeposit.MemberId)
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
