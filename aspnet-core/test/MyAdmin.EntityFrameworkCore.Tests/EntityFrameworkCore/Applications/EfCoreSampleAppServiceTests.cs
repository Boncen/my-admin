using MyAdmin.Samples;
using Xunit;

namespace MyAdmin.EntityFrameworkCore.Applications;

[Collection(MyAdminTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyAdminEntityFrameworkCoreTestModule>
{

}
