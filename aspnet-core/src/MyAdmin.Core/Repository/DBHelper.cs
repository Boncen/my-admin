using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Options;
using MySql.Data.MySqlClient;

namespace MyAdmin.Core.Repository;

public class DBHelper: IDisposable
{
    public readonly IDbConnection Connection;
    // private readonly DBType dbType;
    // private readonly string TableSchema;
    public DBHelper(IConfiguration configuration, IOptions<MaFrameworkOptions> options)
    {
        var connectionString = configuration["ConnectionStrings:Default"];
        // dbType = options.Value.DBType;
        // TableSchema = GetTableSchema(connectionString);
        Connection = new MySqlConnection(connectionString);
    }

    // private string? GetTableSchema(string connectionString)
    // {
    //     var result = StringUtils.GetConnectionStringItem(connectionString, "Database");
    //     if (!Check.HasValue(result))
    //     {
    //         result = StringUtils.GetConnectionStringItem(connectionString, "Initial Catalog");
    //     }
    //
    //     return result;
    // }
    
    // public void UpdateDatabase(Assembly assembly)
    // {
    //     var types = assembly.GetTypes();
    //     foreach (var t in types)
    //     {
    //         if (t.IsInterface)
    //         {
    //             continue;
    //         }
    //
    //         if (t is IEntity)
    //         {
    //             UpdateTable(t);
    //         }
    //     }
    // }
    //
    // private void UpdateTable(Type type)
    // {
    //     var tableName = type.Name;
    //     var properties = type.GetProperties();
    //
    //     if (properties.Length < 1)
    //     {
    //         return;
    //     }
    //
    //     List<TableEntityPropertyInfo> propertyInfos = new List<TableEntityPropertyInfo>();
    //     // 获取当前类型的属性名，类型，数据库名类型
    //     foreach (var prop in properties)
    //     {
    //         var propType = prop.PropertyType;
    //         var propMinLength = 0;
    //         var propMaxLength = 0;
    //         var lengthAttr = prop.GetCustomAttribute<LengthAttribute>();
    //         if (lengthAttr != null)
    //         {
    //             propMinLength = lengthAttr.MinimumLength;
    //             propMaxLength = lengthAttr.MaximumLength;
    //         }
    //         
    //         var propDbTypeName = DBTypeMapper(propType, propMinLength, propMaxLength);
    //         var isNull = propType.IsReferenceOrNullableType();
    //         var desc = prop.GetDescription();
    //         propertyInfos.Add(new TableEntityPropertyInfo()
    //         {
    //             Name = prop.Name,
    //             DbType = propDbTypeName,
    //             MinLength = propMinLength,
    //             MaxLength = propMaxLength,
    //             IsNull = isNull,
    //             Desc = desc
    //         });
    //     }
    //     // 判断表是否存在
    //     // 存在，从数据库获取表所有字段
    //     //比较差异，补充字段，更新字段，删除字段
    //     //不存在，创建表
    // }
    //
    // private string DBTypeMapper(Type propType, int minLength, int maxLength)
    // {
    //     var result = string.Empty;
    //     var typeName = propType.Name;
    //
    //     switch (dbType)
    //     {
    //         case DBType.MySql:
    //             result = MapMysqlDBType(typeName, minLength, maxLength);
    //             break;
    //         case DBType.Postgre:
    //             throw new UnSupposedFeatureException();
    //             break;
    //         case DBType.MsSql:
    //             throw new UnSupposedFeatureException();
    //             break;
    //     }
    //
    //     return result;
    // }
    //
    // private string MapMysqlDBType(string typeName,int minLength, int maxLength)
    // {
    //     
    //     return string.Empty;
    // }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}

// public class TableEntityPropertyInfo
// {
//     public string Name { get; set; }
//     public string DbType { get; set; }
//     public int MinLength { get; set; }
//     public int MaxLength { get; set; }
//     public bool IsNull { get; set; } = false;
//     public string? Desc { get; set; }
// }