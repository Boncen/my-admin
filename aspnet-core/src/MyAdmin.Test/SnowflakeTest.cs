using System.Runtime.InteropServices.JavaScript;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Test;
public class SnowflakeTest
{
    public SnowflakeTest()
    {
        
    }

    [Test]
    public void TestGenerateId()
    {
        Snowflake snowflake = new Snowflake(100);
        HashSet<long> set = new HashSet<long>();

        int[] workIds = new[] { 100, 101, 102, 103, 104 };
        workIds.AsParallel().ForAll(x =>
        {
            var id = snowflake.GenerateId();
            Console.WriteLine(id);
            set.Add(id);
        });
        
        
        
        Assert.IsTrue(set.Count == workIds.Length );
    }
}