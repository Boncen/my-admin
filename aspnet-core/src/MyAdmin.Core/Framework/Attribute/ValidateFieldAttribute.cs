namespace MyAdmin.Core.Framework.Attribute;

public class ValidateTableFieldAttribute: System.Attribute
{
    public required string TableName { get; set; }
    public required string FieldName { get; set; }
    public string? ErrorMessage { get; set; }
}