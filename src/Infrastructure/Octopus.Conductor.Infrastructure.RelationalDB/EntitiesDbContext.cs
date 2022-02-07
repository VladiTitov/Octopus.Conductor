using Microsoft.EntityFrameworkCore;
using Octopus.Conductor.Domain.Entities;
using Octopus.Conductor.Infrastructure.RelationalDB.Config;

namespace Octopus.Conductor.Infrastructure.RelationalDB
{
    public class EntitiesDbContext : DbContext
    {
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options) : base(options)
        { }

        public DbSet<ConductorEntityDescription> EntityDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntityDescriptionConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
