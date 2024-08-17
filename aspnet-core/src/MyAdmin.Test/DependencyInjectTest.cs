using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Repository;

namespace MyAdmin.Test;
[TestFixture]
public class DependencyInjectTest:IDisposable
{
    public IContainer _container;

    public IServiceProvider _serviceProvider;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var builder = new ContainerBuilder();
        RegisterServices(builder);
        _container = builder.Build();
        _serviceProvider = new AutofacServiceProvider(_container);
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        _container.Dispose();
    }
    private void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<DBHelper>();
    }
    [Test]
    public void TestMethod()
    {
        // 获取需要测试的类的实例,需要将该类依赖的类提前注册到容器中
        var dbHelper = _serviceProvider.GetRequiredService<DBHelper>();
        var state = dbHelper.Connection.State;
        // 断言
        Assert.That(state != null);
    }
}