using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyAdmin;

[Dependency(ReplaceServices = true)]
public class MyAdminBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MyAdmin";
}
