using System.ComponentModel.DataAnnotations;

namespace STELA_CONTENT.Core.Entities.Request
{
    public class CreateMemorialBody
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, Range(1, float.MaxValue)]
        public float Price { get; set; }

        [Required, Range(1, float.MaxValue)]
        public float StelaLength { get; set; }

        [Required, Range(1, float.MaxValue)]
        public float StelaWidth { get; set; }

        [Required, Range(1, float.MaxValue)]
        public float StelaHeight { get; set; }

        [Required]
        public required IEnumerable<Guid> MaterialIds { get; set; }
    }
}