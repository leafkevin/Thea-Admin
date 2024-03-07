using System;

namespace MySalon.Domain.Models;

/// <summary>
/// 菜单表
/// </summary>
public class Menu
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public string MenuId { get; set; }
    /// <summary>
    /// 路由名称
    /// </summary>
    public string RouteName { get; set; }
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 上级菜单ID
    /// </summary>
    public string ParentId { get; set; }
    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType MenuType { get; set; }
    /// <summary>
    /// 路由ID
    /// </summary>
    public string RouteId { get; set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }
    /// <summary>
    /// 序号
    /// </summary>
    public int Sequence { get; set; }
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
