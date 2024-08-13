using MyAdmin.Core.Framework;

namespace MyAdmin.ApiHost.Service;

public class TestSingtonService: ISingleton
{
    public string GetServiceName()
    {
        return nameof(TestSingtonService) + this.GetHashCode();
    }
}