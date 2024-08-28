using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Model.Dto;

public class UserDto: FullAuditedEntity<Guid>
{
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public string Name { get; set; }
    public string Account { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Guid? TenantId { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public MaTenant? Tenant { get; set; }
}

public class AddUserDto
{
    public string Name { get; set; }
    public string Account { get; set; }
    public string Password { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    [ValidateTableField(TableName = nameof(MaTenant), FieldName = nameof(MaTenant.Id), ErrorMessage = "租户不存在")]
    public Guid? TenantId { get; set; }
}