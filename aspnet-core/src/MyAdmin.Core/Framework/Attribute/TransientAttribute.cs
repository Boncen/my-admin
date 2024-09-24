namespace MyAdmin.Core.Framework.Attribute;

public class TransientAttribute: System.Attribute
{
    /// <summary>
    /// for keyedService
    /// </summary>
    public string Key { get; set; }

    public TransientAttribute()
    {
        
    }

    public TransientAttribute(string _key)
    {
        Key = _key;
    }
}