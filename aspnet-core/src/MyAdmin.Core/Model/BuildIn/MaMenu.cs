using System.ComponentModel.DataAnnotations.Schema;
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
    public string Name { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public int Order { get; set; }
    public MenuType MenuType { get; set; } = MenuType.Page;
    public int Level { get; set; } = 0;
    public string Code { get; set; }
    
    public ICollection<RoleMenu> RoleMenus { get; set; }
}

public enum MenuType
{
    Page = 1,
    Button = 2
}