namespace MyAdmin.Core.Utilities;

public  class PasswordHelper
{
    public static string HashPassword(string password)
    {
        var randomString = StringUtils.GenerateSecureRandomString(10);
        return HashPassword(password, randomString);
    }
    public static string HashPassword(string password, string salt)
    {
        return CryptoHelper.HashWithArgon2id(password, salt);
    }

    public static bool MatchPassword(string password, string salt, string target)
    {
        var hashResult = HashPassword(password, salt);
        return hashResult.Equals(target, StringComparison.OrdinalIgnoreCase);
    }
    
}