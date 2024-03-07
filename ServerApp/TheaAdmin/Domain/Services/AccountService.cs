using MySalon.Domain.Models;
using MySalon.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Thea;
using Trolley;

namespace MySalon.Domain.Services;

public class AccountService
{
    private readonly IOrmDbFactory dbFactory;
    private readonly TokenService tokenService;
    private readonly ProfileService profileService;

    public AccountService(IOrmDbFactory dbFactory, TokenService tokenService, ProfileService profileService)
    {
        this.dbFactory = dbFactory;
        this.tokenService = tokenService;
        this.profileService = profileService;
    }
    public async Task<TheaResponse> Login(string userAccount, string password)
    {
        using var repository = this.dbFactory.Create();
        var userInfo = await repository.QueryFirstAsync<User>(new { Account = userAccount });
        if (userInfo == null)
            return TheaResponse.Fail(2, $"账号{userAccount}不存在");
        if (userInfo.Status != DataStatus.Active)
            return TheaResponse.Fail(3, $"账号{userAccount}已失效或删除");
        if (!Utilities.VerifyPassword(password, userInfo.Salt, userInfo.Password))
            return TheaResponse.Fail(4, $"账号{userAccount}密码不正确");

        var roleIds = await repository.From<UserRole>()
            .Where(f => f.UserId == userInfo.UserId)
            .Select(f => f.RoleId)
            .ToListAsync();
        if (roleIds.Count == 0)
            return TheaResponse.Fail(5, $"用户{userInfo.UserId}还没有分配角色");

        int code = 0;
        string myRoleIds = null;
        object menuRoutes = null;
        if (roleIds.Count > 1)
        {
            code = 9;
            myRoleIds = string.Join(',', roleIds);
        }
        else
        {
            myRoleIds = roleIds[0];
            var response = await this.profileService.GetMyRoutes(myRoleIds);
            if (!response.IsSuccess) return response;
            menuRoutes = response.Data as List<MenuRouteDto>;
        }
        (var accessToken, var refreshToken, var expires) = this.tokenService.Create(userInfo.UserId, userInfo.Name, myRoleIds);

        return TheaResponse.Succeed(new
        {
            userInfo.UserId,
            UserName = userInfo.Name,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = expires,
            Roles = myRoleIds,
            MenuRoutes = menuRoutes
        }, code);
    }
    public async Task<TheaResponse> RefreshToken(string refreshToken)
    {
        var response = this.tokenService.Resolve(refreshToken);
        if (!response.IsSuccess) return response;
        (var userId, var roles) = ((string, string))response.Data;
        string myRoleId = null;
        if (!roles.Contains(','))
            myRoleId = roles;

        using var repository = this.dbFactory.Create();
        var myUserInfo = await repository.GetAsync<User>(userId);
        if (myUserInfo == null)
            return TheaResponse.Fail(2, $"用户{userId}不存在");
        if (myUserInfo.Status != DataStatus.Active)
            return TheaResponse.Fail(3, $"用户{userId}已失效或删除");

        var roleIds = await repository.From<UserRole>()
               .Where(f => f.UserId == userId)
               .Select(f => f.RoleId)
               .ToListAsync();
        if (roleIds.Count == 0)
            return TheaResponse.Fail(5, $"用户{userId}还没有分配角色");

        int code = 0;
        string myRoleIds = null;
        if (!string.IsNullOrEmpty(myRoleId))
        {
            if (roleIds.Contains(myRoleId))
                myRoleIds = myRoleId;
            else
            {
                if (roleIds.Count > 1)
                {
                    myRoleIds = string.Join(',', roleIds);
                    code = 9;
                }
                else myRoleIds = roleIds[0];
            }
        }
        else
        {
            if (roleIds.Count > 1)
            {
                myRoleIds = string.Join(',', roleIds);
                code = 9;
            }
            else myRoleIds = roleIds[0];
        }
        (var accessToken, refreshToken, var expires) = this.tokenService.Create(userId, myUserInfo.Name, myRoleIds);
        object menuRoutes = null;
        if (code == 0)
        {
            response = await this.profileService.GetMyRoutes(myRoleIds);
            if (!response.IsSuccess) return response;
            menuRoutes = response.Data;
        }
        return TheaResponse.Succeed(new
        {
            myUserInfo.UserId,
            UserName = myUserInfo.Name,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = expires,
            Roles = myRoleIds,
            MenuRoutes = menuRoutes
        });
    }
}
