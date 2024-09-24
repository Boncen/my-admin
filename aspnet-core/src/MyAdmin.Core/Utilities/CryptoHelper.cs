using System.Text;
using Konscious.Security.Cryptography;

namespace MyAdmin.Core.Utilities;

public class CryptoHelper
{
    /// <summary>
    /// recommend for hash password
    /// </summary>
    /// <param name="input"></param>
    /// <param name="salt"></param>
    /// <param name="bc">影响最后生成的字符串的长度</param>
    /// <returns></returns>
    public static string HashWithArgon2id(string input, string salt, int bc = 32)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var argon2 = new Argon2i(bytes);
        argon2.Iterations = 40;
        argon2.Salt = Encoding.UTF8.GetBytes(salt);
        argon2.MemorySize = 8192;
        argon2.DegreeOfParallelism = 16;
        var hash = argon2.GetBytes(bc);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}