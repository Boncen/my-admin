using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.Core.Entity;

public abstract class Entity<TKey> : IEntity<TKey>
{
    public virtual required TKey Id { get; set; }
    public object?[] GetKeys()
    {
        return new object[] { Id };
    }
}

[Serializable]
public abstract class CreationAuditedEntity<TKey> : ICreationAuditedObject<TKey>
{
    public virtual DateTime CreationTime { get; set; }

    public virtual TKey? CreatorId { get; set; }
}

[Serializable]
public abstract class AuditedEntity<TKey> : CreationAuditedEntity<TKey>, IAuditedObject<TKey>
{
    public virtual DateTime? LastModificationTime { get; set; }

    public virtual TKey? LastModifierId { get; set; }

}