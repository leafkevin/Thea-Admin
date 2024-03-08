using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheaAdmin.Dtos;

public class MenuRouteDto
{
    [JsonIgnore]
    public string MenuId { get; set; }
    [JsonIgnore]
    public string ParentId { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public string Component { get; set; }
    public string Redirect { get; set; }
    public MenuRouteMetaDto Meta { get; set; }
    public List<MenuRouteDto> Children { get; set; }
    [JsonIgnore]
    public int Sequence { get; set; }
}
public class MenuRouteMetaDto
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string LinkUrl { get; set; }
    public bool IsHidden { get; set; }
    public bool IsFull { get; set; }
    public bool IsAffix { get; set; }
    public bool IsKeepAlive { get; set; } = true;
    /// <summary>
    /// IsHidden为true的编辑、详情页，需要设置激活的菜单路径
    /// </summary>
    public string MenuPath { get; set; }
}
