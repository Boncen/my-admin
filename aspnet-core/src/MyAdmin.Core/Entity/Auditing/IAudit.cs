namespace MyAdmin.Core.Entity.Auditing;

public interface IAuditedObject<TKey> : ICreationAuditedObject<TKey>, IModificationAuditedObject<TKey>
{
    TKey? LastModifierId { get; }
}

public interface ICreationAuditedObject<TKey> : IHasCreationTime, IMayHaveCreator<TKey>
{

}

public interface IHasCreationTime
{
    DateTime CreationTime { get; }
}
public interface IHasModificationTime
{
    DateTime? LastModificationTime { get; }
}

public interface IMayHaveCreator<TKey>
{
    TKey? CreatorId { get; }
}
public interface IModificationAuditedObject<TKey> : IHasModificationTime
{
    TKey? LastModifierId { get; }
}
public interface ISoftDelete
{
    bool IsDeleted { get; }
}

public interface IHasDeletionTime : ISoftDelete
{
    DateTime? DeletionTime { get; }
}
public interface IDeletionAuditedObject<TKey> : IHasDeletionTime
{
    TKey? DeleterId { get; }
}
public interface IFullAuditedObject<TKey> : IAuditedObject<TKey>, IDeletionAuditedObject<TKey>
{

}

[Serializable]
public abstract class FullAuditedEntity<TKey> : AuditedEntity<TKey>, IFullAuditedObject<TKey>
{
    /// <inheritdoc />
    public virtual bool IsDeleted { get; set; }

    /// <inheritdoc />
    public virtual TKey? DeleterId { get; set; }

    /// <inheritdoc />
    public virtual DateTime? DeletionTime { get; set; }
}