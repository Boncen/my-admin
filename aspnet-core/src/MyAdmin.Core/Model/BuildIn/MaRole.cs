using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;
[BuiltIn]
[Table("MaRole")]
public class MaRole:FullAuditedEntity<Guid>, IEnableObject, ITenantObject<Guid?>,IEntity<Guid>
{
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    [Comment("角色编码")]
    public string Code { get; set; }
    
    // public ICollection<RoleMenu> RoleMenus { get; set; }
    // public ICollection<UserRole> UserRoles { get; set; }
    // public MaTenant? Tenant { get; set; }
    // public MaUser? Creator { get; set; }
    // public MaUser? LastModifier { get; set; }
    // public MaUser? Deleter { get; set; }
}