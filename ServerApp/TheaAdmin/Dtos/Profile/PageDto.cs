namespace TheaAdmin.Dtos;

public class PageDto
{
    /// <summary>
    /// 路由ID
    /// </summary>
    public string RouteId { get; set; }
    /// <summary>
    /// 路由名称
    /// </summary>
    public string RouteName { get; set; }
    /// <summary>
    /// 路由标题
    /// </summary>
    public string RouteTitle { get; set; }
    /// <summary>
    /// 路由地址
    /// </summary>
    public string RouteUrl { get; set; }
    /// <summary>
    /// 组件物理路径
    /// </summary>
    public string Component { get; set; }
    /// <summary>
    /// 菜单ID
    /// </summary>
    public string MenuId { get; set; }
    /// <summary>
    /// 重定向URL
    /// </summary>
    public string RedirectUrl { get; set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }
    /// <summary>
    /// 是否需要隐藏
    /// </summary>
    public bool IsHidden { get; set; }
    /// <summary>
    /// 是否外部连接
    /// </summary>
    public bool IsLink { get; set; }
    /// <summary>
    /// 是否全屏显示
    /// </summary>
    public bool IsFull { get; set; }
    /// <summary>
    /// 是否固定标签页
    /// </summary>
    public bool IsAffix { get; set; }
    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsKeepAlive { get; set; }
}
