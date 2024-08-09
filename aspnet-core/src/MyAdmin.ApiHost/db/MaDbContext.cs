using Microsoft.EntityFrameworkCore;
using MyAdmin.Core;

namespace MyAdmin.ApiHost;

public class MaDbContext : DbContext
{
    public DbSet<Log> Logs { get; set; }
    public MaDbContext(DbContextOptions<MaDbContext> dbContextOptions):base(dbContextOptions)
    {
        // try
        // {
        //     Database.EnsureCreated();
        // }
        // catch (Exception ex)
        // {
        //     throw new MAException("数据库连接错误", ex);
        // }
    }
}
