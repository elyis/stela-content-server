namespace STELA_CONTENT.Core.Entities.Response
{
    public class BaseMemorialBody
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }

        public List<MemorialMaterialBody> Materials { get; set; } = new();
    }

    public class MemorialBody : BaseMemorialBody
    {
        public string Description { get; set; }
        public float Price { get; set; }
        public string StelaSize { get; set; }
    }

    public class ShortMemorialBody : BaseMemorialBody
    {
    }
}