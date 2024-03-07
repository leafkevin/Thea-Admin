using System.Collections.Generic;

namespace Thea;

public interface IPassport
{
    /// <summary>
    /// 用户ID
    /// </summary>
    string UserId { get; }
    /// <summary>
    /// 用户姓名
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 用户账号
    /// </summary>
    //string Account { get; }
    /// <summary>
    /// 角色Ids，多个角色逗号分割，如：1，2
    /// </summary>
    string Roles { get; }
}