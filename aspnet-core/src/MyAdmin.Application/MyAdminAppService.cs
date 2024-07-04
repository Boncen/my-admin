using System;
using System.Collections.Generic;
using System.Text;
using MyAdmin.Localization;
using Volo.Abp.Application.Services;

namespace MyAdmin;

/* Inherit your application services from this class.
 */
public abstract class MyAdminAppService : ApplicationService
{
    protected MyAdminAppService()
    {
        LocalizationResource = typeof(MyAdminResource);
    }
}
