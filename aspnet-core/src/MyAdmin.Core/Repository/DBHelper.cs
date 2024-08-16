using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
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

    public int InsertAsync<T>(T entity)
    {
        string sql = $"insert into {typeof(T).Name}";
        return 0;
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
 