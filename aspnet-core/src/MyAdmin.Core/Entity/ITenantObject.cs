namespace MyAdmin.Core.Entity;

public interface ITenantObject<TKey>
{
    TKey TenantId { get; set; }
}