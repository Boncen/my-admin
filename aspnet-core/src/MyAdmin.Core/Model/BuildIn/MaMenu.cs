using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("MaMenu")]
public class MaMenu:FullAuditedEntity<Guid>,  ITenantObject<Guid?>,IEntity<Guid>,IOrderObject
{
    public Guid? TenantId { get; set; }
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    [Comment("路由路径")]
    public string? Url { get; set; }

    public string? Icon { get; set; }
    public int Order { get; set; } = 0;
    public MenuType MenuType { get; set; } = MenuType.Page;
    public int Level { get; set; } = 0;
    public required string Code { get; set; }
    
    // public ICollection<RoleMenu> RoleMenus { get; set; }
    // public MaUser? Creator { get; set; }
    // public MaUser? LastModifier { get; set; }
    // public MaUser? Deleter { get; set; }
    // public MaTenant? Tenant { get; set; }
}

public enum MenuType
{
    Page = 1,
    Button = 2,
    Category = 3,
}