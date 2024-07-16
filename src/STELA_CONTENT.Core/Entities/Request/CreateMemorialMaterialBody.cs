using System.ComponentModel.DataAnnotations;

namespace STELA_CONTENT.Core.Entities.Request
{
    public class CreateMemorialMaterialBody
    {
        public string Name { get; set; }

        [RegularExpression("^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$")]
        public string? Hex { get; set; }
        public string ColorName { get; set; }
    }
}