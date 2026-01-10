using EFCoreExperements.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace EFCoreExperements.Core.Context
{
    public class MainContext : DbContext
    {
        public DbSet<LargeEntity> LargeEntities { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public void CreateTables()
        {
            Database.EnsureCreated();
        }
    }
}