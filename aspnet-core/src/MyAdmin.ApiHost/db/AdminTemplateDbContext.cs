using Microsoft.EntityFrameworkCore;
using MyAdmin.ApiHost.models;
using MyAdmin.Core.Framework;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.Db;

public class AdminTemplateDbContext : MaDbContext,ITransient
{
    public DbSet<Order> Orders { get; set; }

    public AdminTemplateDbContext()
    {
        
    } 
    public AdminTemplateDbContext(DbContextOptions<MaDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
