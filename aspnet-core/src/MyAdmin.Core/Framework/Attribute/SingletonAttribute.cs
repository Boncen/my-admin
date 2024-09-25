namespace MyAdmin.Core.Framework.Attribute;

public class SingletonAttribute: System.Attribute
{
    /// <summary>
    /// for keyedService
    /// </summary>
    public string? Key { get; set; }
    public SingletonAttribute()
    {
        
    }

    public SingletonAttribute(string _key)
    {
        Key = _key;
    }
}