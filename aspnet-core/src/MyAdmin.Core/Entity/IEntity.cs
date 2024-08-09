namespace MyAdmin.Core.Entity;


public interface IEntity
{
    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object?[] GetKeys();
    
}
public interface IEntity<TKey>: IEntity
{
    public TKey Id { get; set; }

}