using EFCoreExperements.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreExperements.Core.Context
{
    public class MainContext : DbContext
    {
        public DbSet<LargeEntity> LargeEntities { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
        }

        public void CreateTables()
        {

        }
    }
}
