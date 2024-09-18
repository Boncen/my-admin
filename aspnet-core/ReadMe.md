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
      "CacheType": 1, // CacheTypeEnum
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
- 对于需要特殊化处理的某些表，设置项的值可以设置为 `表名.字段名`，需要匹配大小写。


### get请求

实例：
```
http://localhost:5026/easy?target=user&page=1&count=21&columns=id,name, account
```
- target  表名
- page    页码
- count   返回条数
- columns 请求返回的列，不传默认返回全部

其他参数请求直接拼上即可。如：`&name=foo&type=bar`

加上参数 `&total=<任意数值>` 告诉后端该分页接口需要返回total

### post请求

#### 添加数据
http://localhost:5026/easy

字段前不能加 `@` 符号.

添加单条
```json
{
  "TestEasy": {   /** 待添加字段 */    },
}

```

批量添加
```json
{
  "TestEasy": [
      {   /** 待添加字段 */    },
      {   /** 待添加字段 */    },
      {   /** 待添加字段 */    },
  ]
}
```

#### 查询
在一次请求中查询多个表, 为了防止混淆，关键的字段前面需要带上`@`.
```json
{
  "user": {
    "@page": 1,
    "@count": 10,
    "@total": 0,
    "@where": {
      "salt": {
        "type": "lessThan",
        "value": 8894561230
      },
      "name": {
        "value": "dev"
      }
    }
  },
  "MaLog": {
    "@page": 1,
    "@count": 3,
    "@total": 0
  }
}
```

where条件中type取值如下：
```
contains
in
lessThan
greaterThan
lessThanOrEqual
greaterThanOrEqual
equal
notEqual
```