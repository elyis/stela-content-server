using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Configurations
{
    public class MemorialConfiguration : IEntityTypeConfiguration<Memorial>
    {
        public void Configure(EntityTypeBuilder<Memorial> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}