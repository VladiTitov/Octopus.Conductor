using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octopus.Conductor.Domain.Entities;

namespace Octopus.Conductor.Infrastructure.RelationalDB.Config
{
    public class EntityDescriptionConfig : IEntityTypeConfiguration<ConductorEntityDescription>
    {
        public void Configure(EntityTypeBuilder<ConductorEntityDescription> builder)
        {
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.EntityType).IsRequired();
            builder.Property(entity => entity.InputDirectory).IsRequired();
            builder.Property(entity => entity.OutputDirectory).IsRequired();
        }
    }
}
