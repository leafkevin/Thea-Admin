using System;

namespace TheaAdmin.Domain.Models;

/// <summary>
/// 角色表
/// </summary>
public class Role
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }
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
