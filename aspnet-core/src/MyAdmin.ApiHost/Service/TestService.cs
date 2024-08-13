using MyAdmin.Core.Framework;

namespace MyAdmin.ApiHost.Service;

public class TestService: IScoped
{
    public string GetServiceName()
    {
        return nameof(TestService) + this.GetHashCode();
    }
}