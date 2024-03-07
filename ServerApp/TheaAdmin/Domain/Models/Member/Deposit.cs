using System;

namespace MySalon.Domain.Models;

/// <summary>
/// 充值表
/// </summary>
public class Deposit
{
    /// <summary>
    /// 充值ID
    /// </summary>
    public string DepositId { get; set; }
    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; }
    /// <summary>
    /// 充值金额
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// 赠送金额
    /// </summary>
    public double Bonus { get; set; }
    /// <summary>
    /// 充值前余额
    /// </summary>
    public double BeginBalance { get; set; }
    /// <summary>
    /// 充值后余额
    /// </summary>
    public double EndBalance { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public sbyte Status { get; set; }
    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; }
    /// <summary>
    /// 创建日期
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// 最后更新人
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 最后更新日期
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
