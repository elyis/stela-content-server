using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Configurations
{
    public class MemorialMaterialsConfiguration : IEntityTypeConfiguration<MemorialMaterials>
    {
        public void Configure(EntityTypeBuilder<MemorialMaterials> builder)
        {
            builder.HasKey(e => new
            {
                e.MemorialId,
                e.MaterialId
            });
        }
    }
}