using System;

namespace MySalon.Domain.Models;

/// <summary>
/// 用户表,所有登陆系统的用户信息
/// </summary>
public class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }
    /// <summary>
    /// 手机号码
    /// </summary>
    public string Mobile { get; set; }
    /// <summary>
    /// 邮件地址
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 头像地址
    /// </summary>
    public string AvatarUrl { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 盐
    /// </summary>
    public string Salt { get; set; }
    /// <summary>
    /// 解锁时间
    /// </summary>
    public DateTime LockoutEnd { get; set; }
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