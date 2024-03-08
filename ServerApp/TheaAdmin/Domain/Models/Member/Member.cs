using System;

namespace TheaAdmin.Domain.Models;

/// <summary>
/// 会员表
/// </summary>
public class Member
{
    /// <summary>
    /// 会员ID
    /// </summary>
    public string MemberId { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string MemberName { get; set; }
    /// <summary>
    /// 手机号码
    /// </summary>
    public string Mobile { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; set; }
    /// <summary>
    /// 余额
    /// </summary>
    public double Balance { get; set; }
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
