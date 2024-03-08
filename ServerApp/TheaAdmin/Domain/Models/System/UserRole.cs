using System;

namespace TheaAdmin.Domain.Models;

/// <summary>
/// 用户角色关联表
/// </summary>
public class UserRole
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 角色ID
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// 最后更新人
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 最后更新日期
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
