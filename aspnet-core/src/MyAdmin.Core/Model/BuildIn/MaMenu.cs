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
     [Comment("路由名称")]
    public required string Name { get; set; }
    [Comment("路由路径")]
    public string? Path { get; set; }

    public string? Icon { get; set; }
    public int Order { get; set; } = 0;
    public MenuType MenuType { get; set; } = MenuType.Page;
    // public int Level { get; set; } = 0;
    // public required string Code { get; set; }
    // public bool RequiresAuth { get; set; }
    public string? Locale { get; set; }
    public string? Label { get; set; }
}

public enum MenuType
{
    Page = 1,
    Button = 2,
    /// <summary>
    /// 
    /// </summary>
    Category = 3,
}