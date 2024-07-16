using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Configurations
{
    public class MemorialMaterialConfiguration : IEntityTypeConfiguration<MemorialMaterial>
    {
        public void Configure(EntityTypeBuilder<MemorialMaterial> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Hex)
                   .HasMaxLength(6)
                   .IsRequired(false);
        }
    }
}