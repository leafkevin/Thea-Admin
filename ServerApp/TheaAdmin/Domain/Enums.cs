namespace TheaAdmin.Domain;

public enum DataStatus : byte
{
    None,
    Active,
    Disabled,
    Deleted
}
public enum Gender : byte
{
    Unknown,
    Male,
    Female
}
public enum RouteType : byte
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    Root,
    /// <summary>
    /// 菜单项
    /// </summary>
    Menu,
    /// <summary>
    /// 叶子菜单，链接页面
    /// </summary>
    Leaf,
    /// <summary>
    /// 组件路由
    /// </summary>
    Page
}
