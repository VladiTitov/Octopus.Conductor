using Microsoft.EntityFrameworkCore;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Infrastructure.Data.Config;

namespace Octopus.Conductor.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<ConductorEntityDescription> EntityDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntityDescriptionConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
