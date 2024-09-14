# 准备工作

安装 dotnet-ef
```shell
dotnet tool install --global dotnet-ef
echo 'export PATH=$PATH:$HOME/.dotnet/tools' >> ~/.bashrc
```

```shell
dotnet ef migrations add InitialCreate --context <YourDbContext>
dotnet ef database update --context <YourDbContext>
```

# 快速开始
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMaFramework( builder.Configuration, (o)=>{
    // other config
}, Assembly.GetExecutingAssembly());
var app = builder.Build();
app.UseMaFramework(builder.Configuration);
app.Run();
```


# 功能

## 便捷注册依赖

通过实现 `IScoped`，`ISingleton`,`ITransient` 或者 加上特性 `ScopedAttribute`, `SingletonAttribute`,`TransientAttribute`

通过特性还可以传递key,通过key注入特定依赖。key暂时只支持string类型。

## 仓储模式

通过 `IRepository` 注入仓储。对于自定义的 `DbContext` 子类，在注入的时候需要多传递 DbContext 类型参数。
```csharp
private readonly IRepository<Order, Guid, AdminTemplateDbContext> _orderRepository;
```

自定义的 `DbContext` 需要继承 `MaDbContext`。

## 缓存
默认使用内存缓存，在配置文件appsetting文件中添加：
```json
{
  "MaFrameworkOptions": {
    "Cache": {
    }
  }
}
```

## EasyApi

The default endpoint name is `/easy`.

### 表别名
```
  "TableAlias": {
        "user": "MaUser"
      },
```

如果所查询的表未配置在别名列表中，则需要完全匹配大小写。


### 列别名
MaFrameworkOptions:EasyApi:ColumnAlias
```
 "ColumnAlias": {
        "n": "Name",
        "_IsEnabled": "IsEnabled",
        "_Password": "Password",
        "passwd": "MaUser.Password",
      }
```

- 列别名的key设置为下划线加原值，则不会返回，需要匹配大小写。 
- 对于需要特殊化处理的某些表，设置项的值可以设置为 `表名.字段名`，注意大小写。




