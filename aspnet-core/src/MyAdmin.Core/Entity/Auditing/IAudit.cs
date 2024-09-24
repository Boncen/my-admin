namespace MyAdmin.Core.Entity.Auditing;

public interface IAuditedObject<TKey> : ICreationAuditedObject<TKey>, IModificationAuditedObject<TKey>
{
}

public interface ICreationAuditedObject<TKey> : IHasCreationTime, IMayHaveCreator<TKey>
{

}

public interface IHasCreationTime
{
    DateTime CreationTime { get;set; }
}
public interface IHasModificationTime
{
    DateTime? LastModificationTime { get;set; }
}

public interface IMayHaveCreator<TKey>
{
    TKey? CreatorId { get;set; }
}
public interface IModificationAuditedObject<TKey> : IHasModificationTime
{
    TKey? LastModifierId { get;set; }
}
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}

public interface IHasDeletionTime : ISoftDelete
{
    DateTime? DeletionTime { get; set; }
}
public interface IDeletionAuditedObject<TKey> : IHasDeletionTime
{
    TKey? DeleterId { get; set;}
}
public interface IFullAuditedObject<TKey> : IAuditedObject<TKey>, IDeletionAuditedObject<TKey>
{

}

[Serializable]
public abstract class FullAuditedEntity<TKey> : AuditedEntity<TKey>, IFullAuditedObject<TKey>
{
    public virtual bool IsDeleted { get; set; } = false;

    public virtual TKey? DeleterId { get; set; }

    public virtual DateTime? DeletionTime { get; set; }
}