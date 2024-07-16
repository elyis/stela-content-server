using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Configurations
{
    public class PortfolioMemorialMaterialsConfiguration : IEntityTypeConfiguration<PortfolioMemorialMaterials>
    {
        public void Configure(EntityTypeBuilder<PortfolioMemorialMaterials> builder)
        {
            builder.HasKey(e => new
            {
                e.MemorialMaterialId,
                e.PortfolioMemorialId
            });
        }
    }
}