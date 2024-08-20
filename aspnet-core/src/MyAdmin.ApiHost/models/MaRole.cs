using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.ApiHost.models;
[Table("MaRole")]
public class MaRole:FullAuditedEntity<Guid>, IEnableObject, ITenantObject<Guid>,IEntity<Guid>
{
    public bool IsEnabled { get; set; }
    public Guid TenantId { get; set; }
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    
}