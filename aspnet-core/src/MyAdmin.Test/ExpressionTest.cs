using System.Linq.Expressions;

namespace MyAdmin.Test;

[TestFixture]
public class ExpressionTest
{
    [SetUp]
    public void Init()
    {
        _data = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };
    }

    private List<int> _data;

    [Test]
    public void TestAnd()
    {
        var param = Expression.Parameter(typeof(int), "x");
        var constExp = Expression.Constant(7, 7.GetType());
        var greaterExp = Expression.GreaterThan(param, constExp);

        var lessExp = Expression.LessThan(param, Expression.Constant(9));

        var body = Expression.And(greaterExp, lessExp);

        var ld = Expression.Lambda<Func<int, bool>>(body, param);
        var func = ld.Compile();
        var res = _data.Where(func).ToList();
        Assert.Greater(res.Count(), 1);
    }
}