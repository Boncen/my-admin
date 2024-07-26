using Microsoft.EntityFrameworkCore;

namespace MyAdmin.ApiHost;

public class MaDbContext : DbContext
{
    public DbSet<Log> Logs { get; set; }
    public MaDbContext(DbContextOptions<MaDbContext> dbContextOptions):base(dbContextOptions)
    {
        
    }
}
