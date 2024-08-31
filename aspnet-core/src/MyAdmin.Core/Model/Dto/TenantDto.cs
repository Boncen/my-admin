using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.Core.Model.Dto;

public class TenantDto:FullAuditedEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class AddTenantDto
{
    public string Name { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class TenantSearchDto
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
}