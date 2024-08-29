using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("MaUser")]
public class MaUser:FullAuditedEntity<Guid>, IEnableObject, ITenantObject<Guid?>,IEntity<Guid>
{
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string? Salt { get; set; }
    public string Account { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Guid? TenantId { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public MaTenant Tenant { get; set; }
}
