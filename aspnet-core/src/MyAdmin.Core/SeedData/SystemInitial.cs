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
            Name = "首页",
            Code = "Home",
            MenuType = MenuType.Page,
            Level = 0,
            Order = 1,
            LastModifierId = AdminUserId,
        },

        #endregion

        #region 系统管理

        new MaMenu()
        {
            Id = MenuId_sys_manager,
            Name = "系统管理",
            Code = "SystemManager",
            MenuType = MenuType.Category,
            Level = 0,
            Order = 2,
            LastModifierId = AdminUserId
        },

        #region 用户管理

        new MaMenu()
        {
            Id = MenuId_user,
            Name = "用户管理",
            Code = "UserManager",
            Url = "/sys/user/index",
            MenuType = MenuType.Page,
            Level = 1,
            Order = 1,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_user_add,
            Name = "用户管理-新增",
            Code = "UserManager-Add",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 1,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_user_update,
            Name = "用户管理-更新",
            Code = "UserManager-Update",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 1,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_user_delete,
            Name = "用户管理-删除",
            Code = "UserManager-Delete",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 1,
            LastModifierId = AdminUserId
        },

        #endregion

        #region 角色管理

        new MaMenu()
        {
            Id = MenuId_role,
            Name = "角色管理",
            Code = "RoleManager",
            Url = "/sys/role/index",
            MenuType = MenuType.Page,
            Level = 1,
            Order = 2,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_role_add,
            Name = "角色管理-新增",
            Code = "RoleManager-Add",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 2,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_role_update,
            Name = "角色管理-更新",
            Code = "RoleManager-Update",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 2,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_role_delete,
            Name = "角色管理-删除",
            Code = "RoleManager-Delete",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 2,
            LastModifierId = AdminUserId
        },

        #endregion
       
        #region 租户管理

        new MaMenu()
        {
            Id = MenuId_tenant,
            Name = "租户管理",
            Code = "TenantManager",
            Url = "/sys/tenant/index",
            MenuType = MenuType.Page,
            Level = 1,
            Order = 3,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_tenant_add,
            Name = "租户管理-新增",
            Code = "TenantManager-Add",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 3,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_tenant_update,
            Name = "租户管理-更新",
            Code = "TenantManager-Update",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 3,
            LastModifierId = AdminUserId
        },
        new MaMenu()
        {
            Id = MenuId_tenant_delete,
            Name = "租户管理-删除",
            Code = "TenantManager-Delete",
            MenuType = MenuType.Button,
            Level = 1,
            Order = 3,
            LastModifierId = AdminUserId
        },

        #endregion
        new MaMenu()
        {
            Id = MenuId_log,
            Name = "系统日志",
            Code = "Log",
            MenuType = MenuType.Page,
            Level = 0,
            Order = 3,
            LastModifierId = AdminUserId
        },

        #endregion
        
    };
}