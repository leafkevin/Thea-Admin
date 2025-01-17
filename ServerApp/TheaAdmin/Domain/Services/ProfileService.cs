﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thea;
using TheaAdmin.Domain.Models;
using TheaAdmin.Dtos;
using Trolley;
using Trolley.MySqlConnector;

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
            .Where((a, b) => a.RoleId == roleId && b.Status == DataStatus.Active)
            .Select((a, b) => new
            {
                b.MenuId,
                b.ParentId
            })
            .UnionAllRecursive((f, self) => f
                .From<Menu>()
                .InnerJoin(self, (a, b) => a.ParentId == b.MenuId)
                .Where((a, b) => a.Status == DataStatus.Active)
                .Select((a, b) => new
                {
                    a.MenuId,
                    a.ParentId
                }))
            .AsCteTable("MenuList");
        var menuItems = await repository
            .From<Menu>()
            .InnerJoin(cteQuery, (a, b) => a.MenuId == b.MenuId)
            .Where((a, b) => a.Status == DataStatus.Active)
            .Select((a, b) => a)
            .Union(f => f.From<Menu>()
                .Where(f => f.IsStatic && f.Status == DataStatus.Active)
                .Select(f => f))
            .ToListAsync();
        if (menuItems.Count <= 0)
            return TheaResponse.Fail(1, "没有配置任何菜单数据");

        var rootId = menuItems.First(f => !string.IsNullOrEmpty(f.ParentId)).ParentId;
        var menuIds = menuItems.FindAll(f => f.RouteType == RouteType.Leaf).Select(f => f.MenuId).ToList();
        var myPages = await repository.From<Route>()
            .LeftJoin<MenuPage>((a, b) => a.RouteId == b.RouteId)
            .Where((a, b) => a.Status == DataStatus.Active && (menuIds.Contains(b.MenuId) || a.IsStatic))
            .SelectFlattenTo<PageDto>((a, b) => new())
            .ToListAsync();
        var result = new List<RouteDto>();
        var myMenus = menuItems.FindAll(f => f.ParentId == rootId || f.IsStatic);
        if (myMenus.Count > 1)
            myMenus.Sort((x, y) => x.Sequence.CompareTo(y.Sequence));
        var menuRoutes = new List<RouteDto>();
        this.AddMenuRoutes(myMenus, menuRoutes, menuItems, myPages);
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
    private void AddMenuRoutes(List<Menu> myMenus, List<RouteDto> menuRoutes, List<Menu> menuItems, List<PageDto> pages)
    {
        foreach (var myMenu in myMenus)
        {
            var menuRoute = new RouteDto
            {
                MenuId = myMenu.MenuId,
                ParentId = myMenu.ParentId,
                Path = myMenu.RouteUrl,
                Meta = new MenuRouteMetaDto
                {
                    Title = myMenu.MenuName,
                    Icon = myMenu.Icon,
                    RouteType = myMenu.RouteType
                },
                Children = new(),
                Sequence = myMenu.Sequence
            };
            menuRoutes.Add(menuRoute);
            if (myMenu.RouteType == RouteType.Leaf)
            {
                var myPages = pages.FindAll(f => f.MenuId == myMenu.MenuId);
                //菜单有页面，就有redirect
                var mainRoute = myPages.Find(f => !f.IsHidden);
                menuRoute.Redirect = mainRoute.Component;

                foreach (var myPage in myPages)
                {
                    RouteDto myPageRoute = null;
                    menuRoute.Children.Add(myPageRoute = new RouteDto
                    {
                        Name = myPage.RouteName,
                        Path = myPage.RouteUrl,
                        Meta = new MenuRouteMetaDto
                        {
                            Title = myPage.RouteTitle,
                            RouteType = RouteType.Page
                        }
                    });
                    myPageRoute.Meta.MenuPath = myMenu.RouteUrl;

                    myPageRoute.Component = myPage.Component;
                    myPageRoute.Meta.IsAffix = myPage.IsAffix;
                    myPageRoute.Meta.IsHidden = myPage.IsHidden;
                    myPageRoute.Meta.IsFull = myPage.IsFull;
                    myPageRoute.Meta.IsKeepAlive = myPage.IsKeepAlive;

                    if (myPage.IsLink) myPageRoute.Meta.LinkUrl = myPage.RedirectUrl;
                }
            }
            else
            {
                var children = menuItems.FindAll(f => f.ParentId == myMenu.MenuId);
                if (children.Count > 0)
                {
                    if (children.Count > 1)
                        children.Sort((x, y) => x.Sequence.CompareTo(y.Sequence));
                    menuRoute.Children = new();
                    this.AddMenuRoutes(children, menuRoute.Children, menuItems, pages);
                }
            }
        }
    }
}
