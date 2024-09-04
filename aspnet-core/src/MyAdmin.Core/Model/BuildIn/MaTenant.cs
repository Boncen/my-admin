using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model.BuildIn;

[BuiltIn]
[Table("MaTenant")]
public class MaTenant:FullAuditedEntity<Guid>,IEntity<Guid>,IEnableObject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // public string? FullName { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? ExpirationDate { get; set; }
    // public MaUser? Creator { get; set; }
    // public MaUser? LastModifier { get; set; }
    // public MaUser? Deleter { get; set; }
}