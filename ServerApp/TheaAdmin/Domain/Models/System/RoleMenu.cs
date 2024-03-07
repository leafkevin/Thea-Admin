using System;

namespace MySalon.Domain.Models;

/// <summary>
/// 角色菜单关联表
/// </summary>
public class RoleMenu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// 菜单ID
    /// </summary>
    public string MenuId { get; set; }
    /// <summary>
    /// 最后更新人
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 最后更新日期
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
