using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace Thea;

public class Passport : IPassport
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 用户账号
    /// </summary>
    //public string Account { get; set; }
    /// <summary>
    /// 用户姓名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 角色
    /// </summary>
    public string Roles { get; set; }
    public Passport(ClaimsPrincipal user)
    {
        if (user == null || user.Identity == null)
            return;
        if (user.Identity.IsAuthenticated)
        {
            var netId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            this.UserId = user.FindFirst("sub")?.Value ?? user.FindFirst(netId)?.Value;
            //this.Account = user.FindFirst("acc")?.Value;
            this.Name = user.FindFirst("name")?.Value;
            var netRole = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            this.Roles = user.FindFirst("role")?.Value ?? user.FindFirst(netRole)?.Value;
        }
    }
}
