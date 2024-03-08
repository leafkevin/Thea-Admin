using System;

namespace TheaAdmin.Domain.Models;

/// <summary>
/// 会员订单表
/// </summary>
public class Order
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public string OrderId { get; set; }
    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; }
    /// <summary>
    /// 设计师ID
    /// </summary>
    public string StylistId { get; set; }
    /// <summary>
    /// 是否指定理发师
    /// </summary>
    public bool IsAppointed { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 消费余额
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public DataStatus Status { get; set; }
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