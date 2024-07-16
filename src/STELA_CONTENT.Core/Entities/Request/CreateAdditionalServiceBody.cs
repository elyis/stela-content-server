using System.ComponentModel.DataAnnotations;

namespace STELA_CONTENT.Core.Entities.Request
{
    public class CreateAdditionalServiceBody
    {
        public string Name { get; set; }

        [Range(1, float.MaxValue)]
        public float Price { get; set; }
    }
}