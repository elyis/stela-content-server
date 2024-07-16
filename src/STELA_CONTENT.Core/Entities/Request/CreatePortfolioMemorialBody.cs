using System.ComponentModel.DataAnnotations;

namespace STELA_CONTENT.Core.Entities.Request
{
    public class CreatePortfolioMemorialBody
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CemeteryName { get; set; }

        [Required]
        public required IEnumerable<Guid> MaterialIds { get; set; }
    }
}