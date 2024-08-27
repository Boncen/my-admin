namespace MyAdmin.Core.Framework.Attribute;

public class ScopedAttribute: System.Attribute
{
    /// <summary>
    /// for keyedService
    /// </summary>
    public string Key { get; set; }
    public ScopedAttribute()
    {
        
    }

    public ScopedAttribute(string _key)
    {
        Key = _key;
    }
}