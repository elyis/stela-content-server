using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.Entities.Models
{
    public class AdditionalService
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string? Image { get; set; } = null;

        public AdditionalServiceBody ToAdditionalServiceBody()
        {
            return new AdditionalServiceBody
            {
                Id = Id,
                Name = Name,
                Price = Price,
                UrlImage = Image != null ? $"{Constants.WebUrlToAdditionalServiceImage}/{Image}" : null
            };
        }
    }
}