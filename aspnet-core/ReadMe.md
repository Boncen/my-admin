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

通过特性还可以传递 key,通过 key 注入特定依赖。key 暂时只支持 string 类型。

## 仓储模式

通过 `IRepository` 注入仓储。对于自定义的 `DbContext` 子类，在注入的时候需要多传递 DbContext 类型参数。

```csharp
private readonly IRepository<Order, Guid, AdminTemplateDbContext> _orderRepository;
```

自定义的 `DbContext` 需要继承 `MaDbContext`。

## 缓存

默认使用内存缓存，在配置文件 appsetting 文件中添加：

```json
{
    "MaFrameworkOptions": {
        "Cache": {
            "CacheType": 1 // CacheTypeEnum
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

-   列别名的 key 设置为下划线加原值，则不会返回，需要匹配大小写。
-   对于需要特殊化处理的某些表，设置项的值可以设置为 `表名.字段名`，需要匹配大小写。

### get 请求

实例：

```
http://localhost:5026/easy?@target=user&@page=1&@count=21&@columns=id,name&@orderasc=field&@orderdesc=field
```

-   @target 表名
-   @page 页码
-   @count 返回条数
-   @columns 请求返回的列，不传默认返回全部
-   @orderAsc
-   @orderDesc

其他参数请求直接拼上即可。如：`&name=foo&type=bar`

加上参数 `&@total=<任意数值>` 告诉后端该分页接口需要返回 total

### post 请求

#### 添加数据

http://localhost:5026/easy

字段前不能加 `@` 符号.

添加单条

```json
{
    "TestEasy": {
        /** 表字段 */
    }
}
```

批量添加

```json
{
    "TestEasy": [
        {
            /** 表字段 */
        },
        {
            /** 表字段 */
        },
        {
            /** 表字段 */
        }
    ]
}
```

对于非自增长的主键字段，如果没有给定值，则框架会自动获取该表主键类型并自动添加默认值。

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

where 语句的与或

```
    "@where": {
      "@or": {
        "salt": {
          "type": "lessThan",
          "value": 8894561230
        },
        "name": {
          "value": "dev"
        }
      },
      "@and": {
        "Mobile":{
          "value": "1234567890"
        }
      }
    }
```

where 条件中 type 取值如下：

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

#### 连表查询

使用连表查询的约定：

-   必须传 columns
-   对前端暴露了过多的表结构信息

```json
{
    "user": {
        "@page": 1,
        "@count": 10,
        "@total": 0,
        "@columns": "MaUser.Name as username, MaRole.Name as rolename",
        "@join": [
            {
                "targetJoin": "UserRole",
                "joinField": "userid",
                "onField": "id"
            },
            {
                "targetJoin": "MaRole",
                "joinField": "id",
                "onField": "RoleId",
                "targetOn": "UserRole"
            }
        ],
        "@where": {
            "salt": {
                "type": "lessThan",
                "value": 8894561230
            },
            "MaRole.Name": {
                "value": "dev"
            }
        }
    }
}
```

以上参数转化的 sql 如下：

```sql
SELECT MaUser.Name as username, MaRole.Name as rolename from MaUser
JOIN UserRole ON UserRole.userid = MaUser.id
JOIN MaRole ON MaRole.id = UserRole.RoleId
WHERE MaUser.salt < '8894561230' AND MaRole.Name='dev' limit 10 offset 0
```

#### 排序

```json
{
    "user": {
        "@page": 1,
        "@count": 10,
        "@total": 0,
        "@order": [
            {
                "field": "name",
                "type": "asc"
            }
        ]
    }
}
```

### 删除

批量删除

```json
http://localhost:5066/easy?target=TestEasy&ids=17,18
```

删除

```json
http://localhost:5066/easy?target=TestEasy&id=17
```

### 更新

```json
[
    {
        "@id": "20",
        "Account": "测试啊3",
        "@target": "TestEasy"
    }
]
```

批量更新

```json
[
    {
        "@ids": "20,21,22",
        "Account": "测试啊3",
        "@target": "TestEasy"
    }
]
```

根据条件查询更新，多个更新操作。

```json
[
    {
        "@ids": "20,21,22",
        "Account": "测试啊4",
        "@target": "TestEasy"
    },
    {
        "@ids": "21,22",
        "isAvalid": 1,
        "@target": "TestEasy"
    },
    {
        "@where": {
            "Id": {
                "type": "lessThan",
                "value": 20
            }
        },
        "Account": "测试啊3",
        "@target": "TestEasy"
    }
]
```

### 子表查询

```json
{
    "TestEasy": {
        "@page": 1,
        "@count": 10,
        "@total": 0,
        "@columns": "Address",
        "@children": {
            "target": "TestEasyItem", // 子表名
            "targetField": "ParentId", // 子表关联父表的列
            "parentField": "Id", // 父表id列
            "page": 1,
            "count": 2,
            "customResultFieldName": "items", //
            "columns": "Level" // 只查询指定列
        }
    }
}
```

-   子表查询会自动返回 `@children` 中的 `targetField` 列和 `parentField` 列，即使没有指定。
-   不支持嵌套

###

### 特殊值

框架预设了一些特殊变量，以供特殊场景。

如下所示，通过指定id为 `$CURRENT_USER_ID$` 表示查询当前登录用户。
```json
{
  "user": {
    "@page": 1,
    "@count": 1,
    "@where": {
      "id": {
        "value": "$CURRENT_USER_ID$"
      }
    },
    "@order": [
      {
        "field": "name",
        "type": "asc"
      }
    ]
  }
}
```

特殊值：
- $CURRENT_USER_ID$
- $DATE_NOW$
- $DATETIME_NOW$
- $TODAY$
- $YESTERDAY$
- $TOMORROW$
- $THIS_MONTH$
- $LAST_7_DAYS$
- $LAST_6_MONTH$
- $THIS_YEAR$

