using Microsoft.AspNetCore.Mvc;
using MySalon.Domain.Services;
using MySalon.Dtos;
using System.Threading.Tasks;
using Thea;

namespace MySalon.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly AccountService accountService;

    public AccountController(AccountService accountService)
    {
        this.accountService = accountService;
    }
    [HttpPost]
    public async Task<TheaResponse> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.UserAccount))
            return TheaResponse.Fail(1, $"账号{nameof(request.UserAccount)}不能为空");
        if (string.IsNullOrEmpty(request.Password))
            return TheaResponse.Fail(1, $"密码{nameof(request.Password)}不能为空");

        return await this.accountService.Login(request.UserAccount, request.Password);
    }
    [HttpPost]
    public async Task<TheaResponse> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return TheaResponse.Fail(1, $"刷新token{nameof(request.RefreshToken)}不能为空");

        return await this.accountService.RefreshToken(request.RefreshToken);
    }
    [HttpPost]
    public TheaResponse HashPassword([FromBody] PasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
            return TheaResponse.Fail(1, $"密码不能为空");

        var hashedPassword = Utilities.HashPassword(request.Password, out var salt);
        return TheaResponse.Succeed(new { HashedPassword = hashedPassword, Salt = salt });
    }
}