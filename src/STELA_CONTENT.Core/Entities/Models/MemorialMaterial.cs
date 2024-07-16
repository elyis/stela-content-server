using System.ComponentModel.DataAnnotations;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.Entities.Models
{
    public class MemorialMaterial
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ColorName { get; set; }
        public string? Image { get; set; }

        [MaxLength(6)]
        public string? Hex { get; set; }

        public List<MemorialMaterials> Memorials { get; set; } = new List<MemorialMaterials>();
        public List<PortfolioMemorialMaterials> PortfolioMemorialMaterials { get; set; } = new List<PortfolioMemorialMaterials>();

        public MemorialMaterialBody ToMemorialMaterialBody()
        {
            return new MemorialMaterialBody
            {
                Id = Id,
                ColorName = ColorName,
                Hex = Hex != null ? $"#{Hex}" : null,
                Image = Image == null ? null : $"{Constants.WebUrlToMaterialImage}/{Image}",
                Name = Name
            };
        }
    }
}