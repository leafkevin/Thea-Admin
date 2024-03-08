using MySqlConnector;
using TheaAdmin.Domain.Models;
using Trolley;

namespace TheaAdmin.Domain;

class ModelConfiguration : IModelConfiguration
{
    public void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Member>(f =>
        {
            f.ToTable("mos_member").Key(t => t.MemberId);
            f.Member(t => t.MemberId).Field(nameof(Member.MemberId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.MemberName).Field(nameof(Member.MemberName)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Mobile).Field(nameof(Member.Mobile)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Description).Field(nameof(Member.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Gender).Field(nameof(Member.Gender)).NativeDbType(MySqlDbType.UByte);
            f.Member(t => t.Balance).Field(nameof(Member.Balance)).NativeDbType(MySqlDbType.Double);
            f.Member(t => t.Status).Field(nameof(Member.Status)).NativeDbType(MySqlDbType.UByte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Member.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Member.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Member.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Member.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<Deposit>(f =>
        {
            f.ToTable("mos_deposit").Key(t => t.DepositId);
            f.Member(t => t.DepositId).Field(nameof(Deposit.DepositId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.MemberId).Field(nameof(Deposit.MemberId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Amount).Field(nameof(Deposit.Amount)).NativeDbType(MySqlDbType.Double).Required();
            f.Member(t => t.Bonus).Field(nameof(Deposit.Bonus)).NativeDbType(MySqlDbType.Double);
            f.Member(t => t.BeginBalance).Field(nameof(Deposit.BeginBalance)).NativeDbType(MySqlDbType.Double);
            f.Member(t => t.EndBalance).Field(nameof(Deposit.EndBalance)).NativeDbType(MySqlDbType.Double);
            f.Member(t => t.Description).Field(nameof(Deposit.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Status).Field(nameof(Deposit.Status)).NativeDbType(MySqlDbType.Byte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Deposit.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Deposit.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Deposit.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Deposit.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<User>(f =>
        {
            f.ToTable("sys_user").Key(t => t.UserId);
            f.Member(t => t.UserId).Field(nameof(User.UserId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Name).Field(nameof(User.Name)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Account).Field(nameof(User.Account)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Mobile).Field(nameof(User.Mobile)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Email).Field(nameof(User.Email)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Description).Field(nameof(User.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.AvatarUrl).Field(nameof(User.AvatarUrl)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Password).Field(nameof(User.Password)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Salt).Field(nameof(User.Salt)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.LockoutEnd).Field(nameof(User.LockoutEnd)).NativeDbType(MySqlDbType.DateTime);
            f.Member(t => t.Status).Field(nameof(User.Status)).NativeDbType(MySqlDbType.UByte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(User.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(User.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(User.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(User.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<Role>(f =>
        {
            f.ToTable("sys_role").Key(t => t.RoleId);
            f.Member(t => t.RoleId).Field(nameof(Role.RoleId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RoleName).Field(nameof(Role.RoleName)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Description).Field(nameof(Role.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Status).Field(nameof(Role.Status)).NativeDbType(MySqlDbType.UByte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Role.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Role.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Role.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Role.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<UserRole>(f =>
        {
            f.ToTable("sys_user_role").Key(t => new { t.UserId, t.RoleId });
            f.Member(t => t.UserId).Field(nameof(UserRole.UserId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RoleId).Field(nameof(UserRole.RoleId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(UserRole.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(UserRole.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<Menu>(f =>
        {
            f.ToTable("sys_menu").Key(t => t.MenuId);
            f.Member(t => t.MenuId).Field(nameof(Menu.MenuId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RouteName).Field(nameof(Menu.RouteName)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.MenuName).Field(nameof(Menu.MenuName)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Description).Field(nameof(Menu.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.ParentId).Field(nameof(Menu.ParentId)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.MenuType).Field(nameof(Menu.MenuType)).NativeDbType(MySqlDbType.UByte);
            f.Member(t => t.Icon).Field(nameof(Menu.Icon)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Sequence).Field(nameof(Menu.Sequence)).NativeDbType(MySqlDbType.Int32);
            f.Member(t => t.Status).Field(nameof(Menu.Status)).NativeDbType(MySqlDbType.UByte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Menu.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Menu.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Menu.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Menu.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<RoleMenu>(f =>
        {
            f.ToTable("sys_role_menu").Key(t => new { t.RoleId, t.MenuId });
            f.Member(t => t.RoleId).Field(nameof(RoleMenu.RoleId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.MenuId).Field(nameof(RoleMenu.MenuId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(RoleMenu.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(RoleMenu.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
        builder.Entity<PageRoute>(f =>
        {
            f.ToTable("sys_page_route").Key(t => t.RouteId);
            f.Member(t => t.RouteId).Field(nameof(PageRoute.RouteId)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RouteName).Field(nameof(PageRoute.RouteName)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RouteUrl).Field(nameof(PageRoute.RouteUrl)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.RouteTitle).Field(nameof(PageRoute.RouteTitle)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.Component).Field(nameof(PageRoute.Component)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.MenuId).Field(nameof(PageRoute.MenuId)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Description).Field(nameof(PageRoute.Description)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.RedirectUrl).Field(nameof(PageRoute.RedirectUrl)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.Icon).Field(nameof(PageRoute.Icon)).NativeDbType(MySqlDbType.VarChar);
            f.Member(t => t.IsNeedAuth).Field(nameof(PageRoute.IsNeedAuth)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.IsHidden).Field(nameof(PageRoute.IsHidden)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.IsLink).Field(nameof(PageRoute.IsLink)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.IsFull).Field(nameof(PageRoute.IsFull)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.IsAffix).Field(nameof(PageRoute.IsAffix)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.IsKeepAlive).Field(nameof(PageRoute.IsKeepAlive)).NativeDbType(MySqlDbType.Bool);
            f.Member(t => t.Status).Field(nameof(PageRoute.Status)).NativeDbType(MySqlDbType.UByte).Required();
            f.Member(t => t.CreatedBy).Field(nameof(PageRoute.CreatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.CreatedAt).Field(nameof(PageRoute.CreatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(PageRoute.UpdatedBy)).NativeDbType(MySqlDbType.VarChar).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(PageRoute.UpdatedAt)).NativeDbType(MySqlDbType.DateTime).Required();
        });
    }
}
