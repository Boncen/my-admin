using Volo.Abp.Settings;

namespace MyAdmin.Settings;

public class MyAdminSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MyAdminSettings.MySetting1));
    }
}
