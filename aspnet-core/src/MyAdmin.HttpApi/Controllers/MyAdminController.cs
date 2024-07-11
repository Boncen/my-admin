using MyAdmin.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyAdmin.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyAdminController : AbpControllerBase
{
    protected MyAdminController()
    {
        LocalizationResource = typeof(MyAdminResource);
    }
}
