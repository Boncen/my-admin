using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.SeedData;

public class SystemInitial
{
    public static Guid AdminRoleId = Guid.NewGuid();
    public static Guid AdminUserId = Guid.NewGuid();
    public static MaUser[] Users = new[]
    {
        new MaUser()
        {
            Id = AdminUserId,
            Account = "admin",
            CreationTime = DateTime.Now,
            IsDeleted = false,
            IsEnabled = true,
            Name = "admin",
            Salt = "7894561230",
            Password = "73470577544c7293365739ab3c5037b2313d3a353e190cd04aebf760bd2bb8f2", // admin
        }
    };

    public static MaRole[] Roles = new[]
    {
        new MaRole()
        {
            Id = AdminRoleId,
            CreationTime = DateTime.Now,
            IsDeleted = false,
            IsEnabled = true,
            Name = "admin",
            Description = "admin role",
            CreatorId = AdminUserId,
        }
    };

    public static UserRole[] UserRoles = new[]
    {
        new UserRole()
        {
            UserId = AdminUserId,
            RoleId = AdminRoleId
        }
    };
}