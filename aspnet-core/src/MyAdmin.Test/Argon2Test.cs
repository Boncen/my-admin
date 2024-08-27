using System.Runtime.InteropServices.JavaScript;

namespace MyAdmin.Test;
[TestFixture("HelloPassword", "salt12345678")]
[TestFixture("HelloPassword", "salt87654321")]
public class Argon2Test
{
    private string _password;
    private string _salt;
    public Argon2Test(string input,string salt)
    {
        _password = input;
        _salt = salt;
    }

    [Test]
    public void TestArgon2Hash()
    {
        var result = MyAdmin.Core.Utilities.CryptoHelper.HashWithArgon2id(_password, _salt);
        Assert.IsTrue(result.Length > 1 );
    }
}