using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Model.Dto;

public class UserDto: FullAuditedEntity<Guid>
{
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public required string Name { get; set; }
    public required string Account { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Guid? TenantId { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
    public MaTenant? Tenant { get; set; }
}

public class AddUserDto
{
    public required string Name { get; set; }
    public required string Account { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    [ValidateTableField(TableName = nameof(MaTenant), FieldName = nameof(MaTenant.Id), ErrorMessage = "租户不存在")]
    public Guid? TenantId { get; set; }
}

public class UserSearchDto:PageRequest
{
    public required string Name { get; set; }
    public required string Account { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Guid? TenantId { get; set; }
}