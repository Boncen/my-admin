using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.SeedData;

namespace MyAdmin.Core.Repository;

public class MaDbContext: DbContext
{
    public DbSet<MaLog> Logs { get; set; }
    public DbSet<MaUser> Users { get; set; }
    public DbSet<MaRole> MaRoles { get; set; }
    public DbSet<MaMenu> MaMenus { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RoleMenu> RolePermissions { get; set; }
    public DbSet<MaTenant> Tenants { get; set; }

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
        #region seedData
        modelBuilder.Entity<MaRole>().HasData(SystemInitial.Roles);
        modelBuilder.Entity<MaUser>().HasData(SystemInitial.Users);
        modelBuilder.Entity<UserRole>().HasData(SystemInitial.UserRoles);
        modelBuilder.Entity<MaMenu>().HasData(SystemInitial.Menus);
        #endregion

        #region audit properties user foreign key

        // modelBuilder.Entity<MaRole>().HasOne(r => r.Creator);
        // modelBuilder.Entity<MaRole>().HasOne(r => r.LastModifier);
        // modelBuilder.Entity<MaRole>().HasOne(r => r.Deleter);
        // modelBuilder.Entity<MaRole>().HasOne(r => r.Tenant);
        //
        // modelBuilder.Entity<MaUser>().HasOne(r => r.Creator);
        // modelBuilder.Entity<MaUser>().HasOne(r => r.LastModifier);
        // modelBuilder.Entity<MaUser>().HasOne(r => r.Deleter);
        // modelBuilder.Entity<MaUser>().HasOne(r => r.Tenant);
        //
        //
        // modelBuilder.Entity<MaTenant>().HasOne(r => r.Creator);
        // modelBuilder.Entity<MaTenant>().HasOne(r => r.LastModifier);
        // modelBuilder.Entity<MaTenant>().HasOne(r => r.Deleter);
        //
        // modelBuilder.Entity<MaMenu>().HasOne(r => r.Creator);
        // modelBuilder.Entity<MaMenu>().HasOne(r => r.LastModifier);
        // modelBuilder.Entity<MaMenu>().HasOne(r => r.Deleter);
        // modelBuilder.Entity<MaMenu>().HasOne(r => r.Tenant);

        #endregion
        
        
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<RoleMenu>()
            .HasKey(rp => new { rp.RoleId, rp.MenuId });
        //
        // modelBuilder.Entity<UserRole>()
        //     .HasOne(ur => ur.User)
        //     .WithMany(u => u.UserRoles)
        //     .HasForeignKey(ur => ur.UserId);
        //
        // modelBuilder.Entity<UserRole>()
        //     .HasOne(ur => ur.Role)
        //     .WithMany(r => r.UserRoles)
        //     .HasForeignKey(ur => ur.RoleId);
        //
        // modelBuilder.Entity<RoleMenu>()
        //     .HasOne(rp => rp.Role)
        //     .WithMany(r => r.RoleMenus)
        //     .HasForeignKey(rp => rp.RoleId);
        //
        // modelBuilder.Entity<RoleMenu>()
        //     .HasOne(rp => rp.Menu)
        //     .WithMany(p => p.RoleMenus)
        //     .HasForeignKey(rp => rp.MenuId);
    }
}