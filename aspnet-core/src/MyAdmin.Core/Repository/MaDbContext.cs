using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Repository;

public class MaDbContext: DbContext
{
    public DbSet<MaLog> Logs { get; set; }
    public DbSet<MaUser> Users { get; set; }
    public DbSet<MaRole> MaRoles { get; set; }
    public DbSet<MaPermission> MaPermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    public MaDbContext()
    {
        
    }
    public MaDbContext(DbContextOptions<MaDbContext> dbContextOptions):base(dbContextOptions)
    {
// #if DEBUG
//         try
//         {
//             Database.EnsureCreated();
//         }
//         catch (System.Exception ex)
//         {
//             throw new MAException("数据库连接错误", ex);
//         }
// #endif
    }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
        
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);
        
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
        
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);
        
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
    }
}