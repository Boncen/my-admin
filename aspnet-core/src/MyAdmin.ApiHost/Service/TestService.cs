using MyAdmin.Core.Framework;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.ApiHost.Service;

public class TestService: IScoped
{
    public string GetServiceName()
    {
        return nameof(TestService) + this.GetHashCode();
    }
}

[Transient]
public class TestServiceAttr
{
    public string GetServiceName()
    {
        return nameof(TestServiceAttr) + this.GetHashCode();
    }
}

[Transient("HelloKeyedTest")]
public class TestKeyedServiceAttr
{
    public string GetServiceName()
    {
        return nameof(TestKeyedServiceAttr) + this.GetHashCode();
    }
}