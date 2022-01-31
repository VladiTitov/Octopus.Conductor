using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octopus.Conductor.Core.Entities;

namespace Octopus.Conductor.Infrastructure.Config
{
    public class EntityDescriptionConfig : IEntityTypeConfiguration<EntityDescription>
    {
        public void Configure(EntityTypeBuilder<EntityDescription> builder)
        {
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.EntityType).IsRequired();
            builder.Property(entity => entity.InputDirectory).IsRequired();
            builder.Property(entity => entity.OutputDirectory).IsRequired();
        }
    }
}
