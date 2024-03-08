using TheaAdmin.Domain.Models;
using TheaAdmin.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thea;
using Trolley;
using Microsoft.Extensions.Logging;

namespace TheaAdmin.Domain.Services;

public class ProfileService
{
    private readonly IOrmDbFactory dbFactory;
    private readonly TokenService tokenService;

    public ProfileService(IOrmDbFactory dbFactory, TokenService tokenService)
    {
        this.dbFactory = dbFactory;
        this.tokenService = tokenService;
    }
    public async Task<TheaResponse> SwitchRole(string userId, string roleId)
    {
        using var repository = this.dbFactory.Create();
        var userInfo = await repository.GetAsync<User>(userId);
        if (userInfo == null)
            return TheaResponse.Fail(2, $"用户{userId}不存在");
        if (userInfo.Status != DataStatus.Active)
            return TheaResponse.Fail(3, $"用户{userId}已失效或删除");
        var isExists = await repository.ExistsAsync<UserRole>(new { UserId = userId, RoleId = roleId });
        if (!isExists) return TheaResponse.Fail(4, "角色不存在，请刷新后重试");
        (var accessToken, var refreshToken, var expires) = this.tokenService.Create(userId, userInfo.Name, roleId);

        //登录后已经选过角色了，此时只有一个角色了
        var response = await this.GetMyRoutes(roleId);
        if (!response.IsSuccess) return response;

        return TheaResponse.Succeed(new
        {
            userInfo.UserId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = expires,
            Roles = roleId,
            MenuRoutes = response.Data
        });
    }
    public async Task<TheaResponse> GetMyRoles(string userId)
    {
        using var repository = this.dbFactory.Create();
        var roles = await repository
            .From<UserRole>()
            .InnerJoin<Role>((a, b) => a.RoleId == b.RoleId)
            .Where((a, b) => a.UserId == userId)
            .Select((a, b) => new
            {
                b.RoleId,
                b.RoleName,
                b.Description
            })
            .ToListAsync();
        if (roles.Count <= 0)
            return TheaResponse.Fail(1, "用户没有分配任何角色");
        return TheaResponse.Succeed(roles);
    }
    public async Task<TheaResponse> GetMyRoutes(string roleId)
    {
        using var repository = this.dbFactory.Create();
        var cteQuery = repository
            .From<RoleMenu>()
            .InnerJoin<Menu>((a, b) => a.MenuId == b.ParentId)
            .Where((a, b) => a.RoleId == roleId)
            .Select((a, b) => new
            {
                b.MenuId,
                b.ParentId
            })
            .UnionAllRecursive((f, self) => f
                .From<Menu>()
                .InnerJoin(self, (a, b) => a.ParentId == b.MenuId)
                .Select((a, b) => new
                {
                    a.MenuId,
                    a.ParentId
                }))
            .AsCteTable("MenuList");
        var menuItems = await repository
            .From<Menu>()
            .InnerJoin(cteQuery, (a, b) => a.MenuId == b.MenuId)
            .Select((a, b) => a)
            .OrderBy(f => f.Sequence)
            .ToListAsync();
        if (menuItems.Count <= 0)
            return TheaResponse.Fail(1, "没有配置任何菜单数据");

        var rootId = menuItems.First().ParentId;
        var menuIds = menuItems.FindAll(f => f.MenuType == MenuType.Page).Select(f => f.MenuId).ToList();
        var myPages = await repository.QueryAsync<PageRoute>(f => menuIds.Contains(f.MenuId));
        var result = new List<MenuRouteDto>();
        var myMenus = menuItems.FindAll(f => f.ParentId == rootId);
        var menuRoutes = new List<MenuRouteDto>();
        this.AddChildren("/", myMenus, menuRoutes, menuItems, myPages);
        return TheaResponse.Succeed(menuRoutes);
    }
    public async Task<TheaResponse> ResetPassword(string userId, string password, string operatorId)
    {
        if (string.IsNullOrEmpty(password))
            return TheaResponse.Fail(1, "密码不能为空");

        var hashedPassword = Utilities.HashPassword(password, out var salt);
        using var repository = this.dbFactory.Create();
        var result = await repository.UpdateAsync<User>(new
        {
            UserId = userId,
            Password = hashedPassword,
            Salt = salt,
            UpdatedAt = DateTime.Now,
            UpdatedBy = operatorId
        });
        if (result <= 0) return TheaResponse.Fail(1, "操作失败，请重试");
        return TheaResponse.Success;
    }
    private void AddChildren(string parentPath, List<Menu> myMenus, List<MenuRouteDto> menuRoutes, List<Menu> menuItems, List<PageRoute> pages)
    {
        foreach (var myMenu in myMenus)
        {
            var menuRoute = new MenuRouteDto
            {
                MenuId = myMenu.MenuId,
                ParentId = myMenu.ParentId,
                Name = myMenu.RouteName,
                Meta = new MenuRouteMetaDto
                {
                    Title = myMenu.MenuName,
                    Icon = myMenu.Icon
                }
            };
            menuRoutes.Add(menuRoute);
            if (myMenu.MenuType == MenuType.Page)
            {
                menuRoute.Children = new();
                var myPages = pages.FindAll(f => f.MenuId == myMenu.MenuId);
                foreach (var myPage in myPages)
                {
                    var myPageRoute = new MenuRouteDto
                    {
                        Name = myPage.RouteName,
                        Path = myPage.RouteUrl,
                        Component = myPage.Component,
                        Meta = new MenuRouteMetaDto
                        {
                            Title = myPage.RouteTitle,
                            Icon = myPage.Icon,
                            MenuPath = menuRoute.Path,
                            IsAffix = myPage.IsAffix,
                            IsHidden = myPage.IsHidden,
                            IsFull = myPage.IsFull,
                            IsKeepAlive = myPage.IsFull
                        }
                    };

                    if (myPage.IsLink) myPageRoute.Meta.LinkUrl = myPage.RedirectUrl;
                    else myPageRoute.Redirect = myPage.RedirectUrl;
                    menuRoute.Children.Add(myPageRoute);
                }
            }
            else
            {
                menuRoute.Path = $"{parentPath}{myMenu.RouteName}";
                var children = menuItems.FindAll(f => f.ParentId == myMenu.MenuId);
                if (children != null && children.Count > 0)
                {
                    menuRoute.Children = new();
                    this.AddChildren(menuRoute.Path, children, menuRoute.Children, menuItems, pages);
                }
            }
        }
    }
}
