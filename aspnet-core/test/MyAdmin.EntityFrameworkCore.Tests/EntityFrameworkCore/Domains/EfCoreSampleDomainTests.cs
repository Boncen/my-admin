using MyAdmin.Samples;
using Xunit;

namespace MyAdmin.EntityFrameworkCore.Domains;

[Collection(MyAdminTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyAdminEntityFrameworkCoreTestModule>
{

}
