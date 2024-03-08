using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheaAdmin.Domain.Services;
using TheaAdmin.Dtos;
using TheaAdmin.Dtos.Authorization;
using System.Threading.Tasks;
using Thea;

namespace TheaAdmin.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class ProfileController : ControllerBase
{
    private readonly ProfileService profileService;

    public ProfileController(ProfileService profileService)
    {
        this.profileService = profileService;
    }
    [HttpGet]
    public async Task<TheaResponse> GetMyRoles()
    {
        var passport = this.User.ToPassport();
        return await this.profileService.GetMyRoles(passport.UserId);
    }
    [HttpPost]
    public async Task<TheaResponse> SwitchRole([FromBody] SwitchRoleRequest request)
    {
        var passport = this.User.ToPassport();
        return await this.profileService.SwitchRole(passport.UserId, request.RoleId);
    }
    [HttpPost]
    public async Task<TheaResponse> ResetPassword([FromBody] PasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
            return TheaResponse.Fail(1, $"密码{nameof(request.Password)}不能为空");

        var passport = this.User.ToPassport();
        return await this.profileService.ResetPassword(passport.UserId, request.Password, passport.UserId);
    }
}