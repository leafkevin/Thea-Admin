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
            f.Member(t => t.MemberId).Field(nameof(Member.MemberId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.MemberName).Field(nameof(Member.MemberName)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.Mobile).Field(nameof(Member.Mobile)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.Description).Field(nameof(Member.Description)).DbColumnType("varchar(500)").NativeDbType(MySqlDbType.VarChar).Position(4).Length(500);
            f.Member(t => t.Gender).Field(nameof(Member.Gender)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(5);
            f.Member(t => t.Balance).Field(nameof(Member.Balance)).DbColumnType("double(10,2)").NativeDbType(MySqlDbType.Double).Position(6);
            f.Member(t => t.Status).Field(nameof(Member.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(7).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Member.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(8).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Member.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(9).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Member.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(10).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Member.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(11).Required();
        });
        builder.Entity<Deposit>(f =>
        {
            f.ToTable("mos_deposit").Key(t => t.DepositId);
            f.Member(t => t.DepositId).Field(nameof(Deposit.DepositId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.MemberId).Field(nameof(Deposit.MemberId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.Amount).Field(nameof(Deposit.Amount)).DbColumnType("double(10,2)").NativeDbType(MySqlDbType.Double).Position(3).Required();
            f.Member(t => t.Bonus).Field(nameof(Deposit.Bonus)).DbColumnType("double(10,2)").NativeDbType(MySqlDbType.Double).Position(4);
            f.Member(t => t.BeginBalance).Field(nameof(Deposit.BeginBalance)).DbColumnType("double(10,2)").NativeDbType(MySqlDbType.Double).Position(5);
            f.Member(t => t.EndBalance).Field(nameof(Deposit.EndBalance)).DbColumnType("double(10,2)").NativeDbType(MySqlDbType.Double).Position(6);
            f.Member(t => t.Description).Field(nameof(Deposit.Description)).DbColumnType("varchar(500)").NativeDbType(MySqlDbType.VarChar).Position(7).Length(500);
            f.Member(t => t.Status).Field(nameof(Deposit.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(8).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Deposit.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(9).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Deposit.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(10).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Deposit.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(11).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Deposit.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(12).Required();
        });
        builder.Entity<User>(f =>
        {
            f.ToTable("sys_user").Key(t => t.UserId);
            f.Member(t => t.UserId).Field(nameof(User.UserId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.Name).Field(nameof(User.Name)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.Account).Field(nameof(User.Account)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.Mobile).Field(nameof(User.Mobile)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(4).Length(50).Required();
            f.Member(t => t.Email).Field(nameof(User.Email)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(5).Length(200);
            f.Member(t => t.Description).Field(nameof(User.Description)).DbColumnType("varchar(500)").NativeDbType(MySqlDbType.VarChar).Position(6).Length(500);
            f.Member(t => t.AvatarUrl).Field(nameof(User.AvatarUrl)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(7).Length(200);
            f.Member(t => t.Password).Field(nameof(User.Password)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(8).Length(200);
            f.Member(t => t.Salt).Field(nameof(User.Salt)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(9).Length(50);
            f.Member(t => t.LockoutEnd).Field(nameof(User.LockoutEnd)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(10);
            f.Member(t => t.Status).Field(nameof(User.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(11).Required();
            f.Member(t => t.CreatedBy).Field(nameof(User.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(12).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(User.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(13).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(User.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(14).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(User.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(15).Required();
        });
        builder.Entity<Role>(f =>
        {
            f.ToTable("sys_role").Key(t => t.RoleId);
            f.Member(t => t.RoleId).Field(nameof(Role.RoleId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.RoleName).Field(nameof(Role.RoleName)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.Description).Field(nameof(Role.Description)).DbColumnType("varchar(500)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(500);
            f.Member(t => t.Status).Field(nameof(Role.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(4).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Role.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(5).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Role.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(6).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Role.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(7).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Role.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(8).Required();
        });
        builder.Entity<UserRole>(f =>
        {
            f.ToTable("sys_user_role").Key(t => new { t.UserId, t.RoleId });
            f.Member(t => t.UserId).Field(nameof(UserRole.UserId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.RoleId).Field(nameof(UserRole.RoleId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(UserRole.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(UserRole.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(4).Required();
        });
        builder.Entity<Menu>(f =>
        {
            f.ToTable("sys_menu").Key(t => t.MenuId);
            f.Member(t => t.MenuId).Field(nameof(Menu.MenuId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.MenuName).Field(nameof(Menu.MenuName)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.RouteName).Field(nameof(Menu.RouteName)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.RouteUrl).Field(nameof(Menu.RouteUrl)).DbColumnType("varchar(100)").NativeDbType(MySqlDbType.VarChar).Position(4).Length(100);
            f.Member(t => t.RedirectUrl).Field(nameof(Menu.RedirectUrl)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(5).Length(200);
            f.Member(t => t.Description).Field(nameof(Menu.Description)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(6).Length(200);
            f.Member(t => t.ParentId).Field(nameof(Menu.ParentId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(7).Length(50);
            f.Member(t => t.RouteType).Field(nameof(Menu.RouteType)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(8);
            f.Member(t => t.Icon).Field(nameof(Menu.Icon)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(9).Length(50);
            f.Member(t => t.IsStatic).Field(nameof(Menu.IsStatic)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(10);
            f.Member(t => t.Sequence).Field(nameof(Menu.Sequence)).DbColumnType("int").NativeDbType(MySqlDbType.Int32).Position(11);
            f.Member(t => t.Status).Field(nameof(Menu.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(12).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Menu.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(13).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Menu.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(14).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Menu.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(15).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Menu.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(16).Required();
        });        
        builder.Entity<RoleMenu>(f =>
        {
            f.ToTable("sys_role_menu").Key(t => new { t.RoleId, t.MenuId });
            f.Member(t => t.RoleId).Field(nameof(RoleMenu.RoleId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.MenuId).Field(nameof(RoleMenu.MenuId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(RoleMenu.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(RoleMenu.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(4).Required();
        });
        builder.Entity<Route>(f =>
        {
            f.ToTable("sys_route").Key(t => t.RouteId);
            f.Member(t => t.RouteId).Field(nameof(Route.RouteId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.RouteName).Field(nameof(Route.RouteName)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.RouteTitle).Field(nameof(Route.RouteTitle)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(200).Required();
            f.Member(t => t.RouteUrl).Field(nameof(Route.RouteUrl)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(4).Length(200).Required();
            f.Member(t => t.Component).Field(nameof(Route.Component)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(5).Length(200).Required();
            f.Member(t => t.Description).Field(nameof(Route.Description)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(6).Length(200);
            f.Member(t => t.RedirectUrl).Field(nameof(Route.RedirectUrl)).DbColumnType("varchar(200)").NativeDbType(MySqlDbType.VarChar).Position(7).Length(200);
            f.Member(t => t.Icon).Field(nameof(Route.Icon)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(8).Length(50);
            f.Member(t => t.IsStatic).Field(nameof(Route.IsStatic)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(9);
            f.Member(t => t.IsHidden).Field(nameof(Route.IsHidden)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(10);
            f.Member(t => t.IsLink).Field(nameof(Route.IsLink)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(11);
            f.Member(t => t.IsFull).Field(nameof(Route.IsFull)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(12);
            f.Member(t => t.IsAffix).Field(nameof(Route.IsAffix)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(13);
            f.Member(t => t.IsKeepAlive).Field(nameof(Route.IsKeepAlive)).DbColumnType("tinyint(1)").NativeDbType(MySqlDbType.Bool).Position(14);
            f.Member(t => t.Status).Field(nameof(Route.Status)).DbColumnType("tinyint(4)").NativeDbType(MySqlDbType.Byte).Position(15).Required();
            f.Member(t => t.CreatedBy).Field(nameof(Route.CreatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(16).Length(50).Required();
            f.Member(t => t.CreatedAt).Field(nameof(Route.CreatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(17).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(Route.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(18).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(Route.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(19).Required();
        });        
        builder.Entity<MenuPage>(f =>
        {
            f.ToTable("sys_menu_page").Key(t => new { t.MenuId, t.RouteId });
            f.Member(t => t.MenuId).Field(nameof(MenuPage.MenuId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(1).Length(50).Required();
            f.Member(t => t.RouteId).Field(nameof(MenuPage.RouteId)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(2).Length(50).Required();
            f.Member(t => t.UpdatedBy).Field(nameof(MenuPage.UpdatedBy)).DbColumnType("varchar(50)").NativeDbType(MySqlDbType.VarChar).Position(3).Length(50).Required();
            f.Member(t => t.UpdatedAt).Field(nameof(MenuPage.UpdatedAt)).DbColumnType("datetime").NativeDbType(MySqlDbType.DateTime).Position(4).Required();
        });        
    }
}
