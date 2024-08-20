using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.ApiHost.models;

[Table("MaUser")]
public class MaUser:FullAuditedEntity<Guid>, IEnableObject, ITenantObject<Guid>,IEntity<Guid>
{
    [Key]
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Account { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Guid TenantId { get; set; }
}
