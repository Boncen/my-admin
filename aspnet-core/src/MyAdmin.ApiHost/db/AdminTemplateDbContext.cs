using Microsoft.EntityFrameworkCore;
using MyAdmin.ApiHost.models;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.Db;

public class AdminTemplateDbContext : MaDbContext, ITransient
{
    public DbSet<Order> Orders { get; set; }

    public AdminTemplateDbContext()
    {
    }

    public AdminTemplateDbContext(DbContextOptions<MaDbContext> dbContextOptions) : base(dbContextOptions)
    {
#if DEBUG
        try
        {
            Database.EnsureCreated();
        }
        catch (System.Exception ex)
        {
            throw new MAException("数据库连接错误", ex);
        }
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}