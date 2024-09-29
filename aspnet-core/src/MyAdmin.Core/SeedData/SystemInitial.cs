using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.SeedData;

public class SystemInitial
{
    public static Guid AdminRoleId = Guid.NewGuid();
    public static Guid DevRoleId = Guid.NewGuid();
    public static Guid AdminUserId = Guid.NewGuid();
    public static Guid DevUserId = Guid.NewGuid();


    public static Guid MenuId_home = Guid.NewGuid();
    public static Guid MenuId_sys_manager = Guid.NewGuid();
    public static Guid MenuId_user = Guid.NewGuid();
    public static Guid MenuId_user_add = Guid.NewGuid();
    public static Guid MenuId_user_delete = Guid.NewGuid();
    public static Guid MenuId_user_update = Guid.NewGuid();

    public static Guid MenuId_role = Guid.NewGuid();
    public static Guid MenuId_role_add = Guid.NewGuid();
    public static Guid MenuId_role_delete = Guid.NewGuid();
    public static Guid MenuId_role_update = Guid.NewGuid();

    public static Guid MenuId_tenant = Guid.NewGuid();
    public static Guid MenuId_tenant_add = Guid.NewGuid();
    public static Guid MenuId_tenant_delete = Guid.NewGuid();
    public static Guid MenuId_tenant_update = Guid.NewGuid();

    public static Guid MenuId_menu = Guid.NewGuid();
    public static Guid MenuId_menu_add = Guid.NewGuid();
    public static Guid MenuId_menu_delete = Guid.NewGuid();
    public static Guid MenuId_menu_update = Guid.NewGuid();

    public static Guid MenuId_log = Guid.NewGuid();
    public static Guid MenuId_log_delete = Guid.NewGuid();

    public static MaUser[] Users = new[]
    {
        new MaUser()
        {
            Id = AdminUserId,
            Account = "admin",
            IsEnabled = true,
            Name = "admin",
            Salt = "7894561230",
            Password = "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", // admin
        },
        new MaUser()
        {
            Id = DevUserId,
            Account = "dev",
            IsEnabled = true,
            Name = "dev",
            Salt = "7894561230",
            Password = "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", // admin
        },
    };

    public static MaRole[] Roles = new[]
    {
        new MaRole()
        {
            Id = AdminRoleId,
            CreationTime = DateTime.Now,
            IsEnabled = true,
            Name = "admin",
            Description = "admin role",
            CreatorId = DevUserId,
            Code = "Admin",
            LastModifierId = AdminUserId
        },
        new MaRole()
        {
            Id = DevRoleId,
            CreationTime = DateTime.Now,
            IsEnabled = true,
            Name = "dev",
            Description = "dev role",
            CreatorId = DevUserId,
            Code = "Dev",
            LastModifierId = AdminUserId
        }
    };

    public static UserRole[] UserRoles = new[]
    {
        new UserRole()
        {
            UserId = AdminUserId,
            RoleId = AdminRoleId
        },
        new UserRole()
        {
            UserId = DevUserId,
            RoleId = DevRoleId
        },
    };

    public static MaMenu[] Menus = new[]
    {
        #region home

        new MaMenu()
        {
            Id = MenuId_home,
            Name = "home",
            Label = "首页",
            Path = "/",
            MenuType = MenuType.Page,
            Order = 1,
            LastModifierId = AdminUserId,
            Locale = "menu.server.home"
        },

        #endregion

        #region 系统管理

        new MaMenu()
        {
            Id = MenuId_sys_manager,
            Name = "SysManager",
            Label = "系统管理",
            Path = "/sys",
            Locale = "menu.server.sysmanager",
            MenuType = MenuType.Category,
            Order = 2,
            LastModifierId = AdminUserId
        },

        #region 用户管理

        new MaMenu()
        {
            Id = MenuId_user,
            Name = "users",
            Label = "用户管理",
            Locale = "menu.server.users",
            Path = "/sys/user/index",
            MenuType = MenuType.Page,
            Order = 1,
            LastModifierId = AdminUserId,
            ParentId = MenuId_sys_manager,
        },
        new MaMenu()
        {
            Id = MenuId_user_add,
            Name = "add_user",
            Label = "添加用户",
            Locale = "menu.server.adduser",
            MenuType = MenuType.Button,
            Order = 1,
            LastModifierId = AdminUserId,
            ParentId = MenuId_user
        },
        new MaMenu()
        {
            Id = MenuId_user_update,
            Name = "update_user",
            Label = "更新用户",
            Locale = "menu.server.updateuser",
            MenuType = MenuType.Button,
            Order = 1,
            LastModifierId = AdminUserId,
            ParentId = MenuId_user
        },
        new MaMenu()
        {
            Id = MenuId_user_delete,
            Name = "delete_user",
            Label = "删除用户",
            Locale = "menu.server.deleteuser",
            MenuType = MenuType.Button,
            Order = 1,
            LastModifierId = AdminUserId,
            ParentId = MenuId_user
        },

        #endregion

        #region 角色管理

        new MaMenu()
        {
            Id = MenuId_role,
            Name = "roles",
            Label = "角色管理",
            Locale = "menu.server.roles",
            Path = "/sys/role/index",
            MenuType = MenuType.Page,
            Order = 2,
            LastModifierId = AdminUserId,
            ParentId = MenuId_sys_manager
        },
        new MaMenu()
        {
            Id = MenuId_role_add,
            Name = "add_role",
            Label = "添加角色",
            Locale = "menu.server.addrole",
            MenuType = MenuType.Button,
            Order = 2,
            LastModifierId = AdminUserId,
            ParentId = MenuId_role
        },
        new MaMenu()
        {
            Id = MenuId_role_update,
            Name = "update_role",
            Label = "更新角色",
            Locale = "menu.server.updaterole",
            MenuType = MenuType.Button,
            Order = 2,
            LastModifierId = AdminUserId,
            ParentId = MenuId_role
        },
        new MaMenu()
        {
            Id = MenuId_role_delete,
            Name = "delete_role",
            Label = "删除角色",
            Locale = "menu.server.deleterole",
            MenuType = MenuType.Button,
            Order = 2,
            LastModifierId = AdminUserId,
            ParentId = MenuId_role
        },

        #endregion
       
        #region 租户管理

        new MaMenu()
        {
            Id = MenuId_tenant,
            Name = "tenants",
            Label = "租户管理",
            Locale = "menu.server.tenants",
            Path = "/sys/tenant/index",
            MenuType = MenuType.Page,
            Order = 3,
            LastModifierId = AdminUserId,
            ParentId = MenuId_sys_manager
        },
        new MaMenu()
        {
            Id = MenuId_tenant_add,
            Name = "add_tenant",
            Label = "添加租户",
            Locale = "menu.server.addtenant",
            MenuType = MenuType.Button,
            Order = 3,
            LastModifierId = AdminUserId,
            ParentId = MenuId_tenant
        },
        new MaMenu()
        {
            Id = MenuId_tenant_update,
            Name = "update_tenant",
            Label = "更新租户",
            Locale = "menu.server.updatetenant",
            MenuType = MenuType.Button,
            Order = 3,
            LastModifierId = AdminUserId,
            ParentId = MenuId_tenant
        },
        new MaMenu()
        {
            Id = MenuId_tenant_delete,
            Name = "delete_tenant",
            Label = "删除租户",
            Locale = "menu.server.deletetenant",
            MenuType = MenuType.Button,
            Order = 3,
            LastModifierId = AdminUserId,
            ParentId = MenuId_tenant
        },

        #endregion

        #region 菜单管理

        new MaMenu()
        {
            Id = MenuId_menu,
            Name = "menus",
            Label = "菜单管理",
            Locale = "menu.server.menus",
            Path = "/sys/menu/index",
            MenuType = MenuType.Page,
            Order = 4,
            LastModifierId = AdminUserId,
            ParentId = MenuId_sys_manager
        },
        new MaMenu()
        {
            Id = MenuId_menu_add,
            Name = "add_menu",
            Label = "添加菜单",
            Locale = "menu.server.addmenu",
            MenuType = MenuType.Button,
            Order = 4,
            LastModifierId = AdminUserId,
            ParentId = MenuId_menu
        },
        new MaMenu()
        {
            Id = MenuId_menu_update,
            Name = "update_menu",
            Label = "更新菜单",
            Locale = "menu.server.updatemenu",
            MenuType = MenuType.Button,
            Order = 4,
            LastModifierId = AdminUserId,
            ParentId = MenuId_menu
        },
        new MaMenu()
        {
            Id = MenuId_menu_delete,
            Name = "delete_menu",
            Label = "删除菜单",
            Locale = "menu.server.deletemenu",
            MenuType = MenuType.Button,
            Order = 4,
            LastModifierId = AdminUserId,
            ParentId = MenuId_menu
        },

        #endregion
        new MaMenu()
        {
            Id = MenuId_log,
            Name = "sys_log",
            Label = "系统日志",
            Locale = "menu.server.syslog",
            MenuType = MenuType.Page,
            Order = 5,
            LastModifierId = AdminUserId,
            ParentId = MenuId_sys_manager
        },

        #endregion
        
    };
}