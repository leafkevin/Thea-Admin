using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Thea;

namespace TheaAdmin.Domain.Services;

public class TokenService
{
    private const string _DesKey = "thea!@#$%^abc123";
    private const string _PrivateSecretKey = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBAOLbEj3v8jBu7UxK\r\nW5Vu2GAI+R6Z/InexHVvHt3SrbKNWJ/4hA+Vv/Cxh5pZMEVW/Np1I3ooQkgrpzAS\r\nMJI5ymz6Wm83yzzU+iYTiNaBCyoATnPVZpq0sNgZlogNL8AySNk1ONFS0Q3WxcJr\r\n02kGKTf1huq3f113wCVtz3LOLWeVAgMBAAECgYEAv1EN01pWn+4NBjf3gNDYfjVT\r\nEzG+Pu+2M2uhVmXkwx5dTZwik5oxI2Y0/ECXawLvf7UmVFE7hO37s5jDia5fPW5c\r\nD9vMKSez/SRq550vqU0HROqVPXUyswobmhOylKF2UtEDX4gtGEhX6xPWOH1Mi03O\r\nZMENDDMkRLyURt4/moECQQD2Ljq4eII9TUvOm+Qt5qC1dZ8SyZwQm0Thfwaa26ti\r\ng8IUXHpfcqW17tR/gujPbCYV+nDU4xEdUL72qete7EbxAkEA6+eDrJNpi7XolGN+\r\nL7jV0z3oGnM7768U0pZhF498wig/LZQqD69LZShlc+GmoduzqSBbnUE4zAFBpdu6\r\ndyUS5QJBAJMHFzD3YCmGkaDqwAOd+xuFDSVmXZwZb7ERcXtpeNlUgcQxWzDIQyn+\r\nYtFo+Oxw1epIcbzjhGQyxmqBHz7I9LECQQC2196rKCaqbvgx61umyXCSJm178s0F\r\n3YIaJwxiIojkRCWTwj9HoOqjIUhhJQjuc0cxUy8vF5paJK8pSsGgD1AxAkEAvgUF\r\nKs8+OSN9ZA08MoUY7ad0/u9vkVtmLdvoJ1C2sQ1sWd29XWCb4tVLWlAmTC/7eh74\r\nLa0+ueAkM7yKPwBVxw==";
    //private const string _PublicSecrectKery = "-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDi2xI97/Iwbu1MSluVbthgCPke\r\nmfyJ3sR1bx7d0q2yjVif+IQPlb/wsYeaWTBFVvzadSN6KEJIK6cwEjCSOcps+lpv\r\nN8s81PomE4jWgQsqAE5z1WaatLDYGZaIDS/AMkjZNTjRUtEN1sXCa9NpBik39Ybq\r\nt39dd8Albc9yzi1nlQIDAQAB\r\n-----END PUBLIC KEY-----";

    public (string, string, long) Create(string userId, string userName, string roleIds)
    {
        var sessionId = ObjectId.NewId();
        var extraClaims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Subject, userId),
            new Claim(JwtClaimTypes.SessionId, sessionId),
            new Claim(JwtClaimTypes.Name, userName),
            new Claim(JwtClaimTypes.Role, roleIds)
        };
        var lifeTime = TimeSpan.FromDays(1);
        var expiresDate = DateTime.Now.Add(lifeTime);
        var expires = new DateTimeOffset(expiresDate).ToUnixTimeSeconds();
        var rsa = RSA.Create();
        byte[] privateKeys = Convert.FromBase64String(_PrivateSecretKey);
        rsa.ImportPkcs8PrivateKey(privateKeys, out _);
        var securityKey = new RsaSecurityKey(rsa);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        var securityToken = new JwtSecurityToken("thea", "thea", extraClaims, DateTime.UtcNow, expiresDate, signingCredentials);

        var refreshToken = Utilities.DESEncrypt($"{userId}:{roleIds}:{sessionId}", _DesKey);
        return (new JwtSecurityTokenHandler().WriteToken(securityToken), refreshToken, expires);
    }
    public TheaResponse Resolve(string refreshToken)
    {
        var decryptedRefreshToken = Utilities.DESDecrypt(refreshToken, _DesKey);
        if (!decryptedRefreshToken.Contains(":"))
            return TheaResponse.Fail(1, "无效的刷新token");
        var userInfos = decryptedRefreshToken.Split(':');
        return TheaResponse.Succeed((userInfos[0], userInfos[1]));
    }
}
