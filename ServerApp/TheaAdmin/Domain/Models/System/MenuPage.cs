using System;

namespace TheaAdmin.Domain.Models;

/// <summary>
/// 菜单页面关联表，描述菜单与页面路由的关联关系
/// </summary>
public class MenuPage
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public string MenuId { get; set; }
    /// <summary>
    /// 路由ID
    /// </summary>
    public string RouteId { get; set; }
    /// <summary>
    /// 最后更新人
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 最后更新日期
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
