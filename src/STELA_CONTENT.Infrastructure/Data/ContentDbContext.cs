using Microsoft.EntityFrameworkCore;
using STELA_CONTENT.Core.Entities.Models;

namespace STELA_CONTENT.Infrastructure.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentDbContext).Assembly);
        }

        public DbSet<Memorial> Memorials { get; set; }
        public DbSet<MemorialMaterial> Materials { get; set; }
        public DbSet<MemorialMaterials> MemorialMaterials { get; set; }
        public DbSet<PortfolioMemorial> PortfolioMemorials { get; set; }

        public DbSet<PortfolioMemorialMaterials> PortfolioMemorialMaterials { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }
    }
}