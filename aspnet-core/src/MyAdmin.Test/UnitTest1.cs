using System.Runtime.InteropServices.JavaScript;

namespace MyAdmin.Test;

public class Tests
{
    public int Number;
    [SetUp]
    public void Setup()
    {
        Number = 100;
    }

    [Test]
    public void Test1()
    {
        var result = Number * Number;
        Assert.That(result > Number);
    }
    [Test]
    public void Test2()
    {
        var result = Number * Number;
        Assert.That(result < Number);
    }
}