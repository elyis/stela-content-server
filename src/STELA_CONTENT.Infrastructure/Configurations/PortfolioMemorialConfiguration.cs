using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Configurations
{
    public class PortfolioMemorialConfiguration : IEntityTypeConfiguration<PortfolioMemorial>
    {
        public void Configure(EntityTypeBuilder<PortfolioMemorial> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}