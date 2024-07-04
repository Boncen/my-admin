using Xunit;

namespace MyAdmin.EntityFrameworkCore;

[CollectionDefinition(MyAdminTestConsts.CollectionDefinitionName)]
public class MyAdminEntityFrameworkCoreCollection : ICollectionFixture<MyAdminEntityFrameworkCoreFixture>
{

}
