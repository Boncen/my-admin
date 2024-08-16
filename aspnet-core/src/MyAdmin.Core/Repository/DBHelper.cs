using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MyAdmin.Core.Extensions;
using MyAdmin.Core.Framework;
using MySql.Data.MySqlClient;

namespace MyAdmin.Core.Repository;

public class DBHelper: IDisposable
{
    public readonly IDbConnection Connection;
    public DBHelper(IConfiguration configuration)
    {
        Connection = new MySqlConnection(configuration["ConnectionStrings:Default"]);
    }

    public async Task<int> InsertAsync<T>(T entity, string[]? columns = null)
    {
        if (entity == null)
        {
            return 0;
        }
        var dic = entity.ToDictionary();
        var valueString = dic.ToUrlString(ignoreNull:true, seperator:',');
        string sql = $"insert into {typeof(T).Name} values({valueString})";
        return await Connection.ExecuteAsync(sql);
    }
    
    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
 