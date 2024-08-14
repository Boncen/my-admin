namespace MyAdmin.Core.Utilities;

public static class Converter
{
    public static string ToJsonString(this Object? obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }
    
    
}