using System.Security.Cryptography;

namespace MyAdmin.Core.Utilities;

public static class StringUtils
{
    private const string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
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
    public static string GenerateSecureRandomString(int length, bool upper = false)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var charBytes = new byte[length];
            rng.GetBytes(charBytes);
            var chars = Enumerable.Range(0, length)
                .Select(i => ValidChars[charBytes[i] % ValidChars.Length])
                .ToArray();
            var result = new string(chars);
            return upper ? result.ToUpper(): result;
        }
    }
}