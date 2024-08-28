using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Model.Dto;

public class RoleDto: FullAuditedEntity<Guid>
{
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public MaTenant? Tenant { get; set; }
}

public class AddRoleDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }
}