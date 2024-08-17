namespace MyAdmin.Core.Utilities;

public static class StringUtils
{
    public static string GetConnectionStringItem(string connectionString, string item)
    {
        if (!Check.HasValue(connectionString) || !Check.HasValue(item))
        {
            return string.Empty;
        }

        var items = connectionString.Split(',');
        foreach (var connItem in items)
        {
            if (connItem.Equals(item, StringComparison.OrdinalIgnoreCase))
            {
                return connItem.Split('=')[1];
            }
        }
        return string.Empty;
    } 
}