using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.Core.Entity;

public abstract class Entity<TKey> : IEntity<TKey>
{
    public virtual required TKey Id { get; set; }
}

[Serializable]
public abstract class CreationAuditedEntity<TKey> : ICreationAuditedObject<TKey>
{
    /// <inheritdoc />
    public virtual DateTime CreationTime { get; protected set; }

    /// <inheritdoc />
    public virtual TKey? CreatorId { get; protected set; }
}

[Serializable]
public abstract class AuditedEntity<TKey> : CreationAuditedEntity<TKey>, IAuditedObject<TKey>
{
    /// <inheritdoc />
    public virtual DateTime? LastModificationTime { get; set; }

    /// <inheritdoc />
    public virtual TKey? LastModifierId { get; set; }

}