namespace STELA_CONTENT.Core.Entities.Response
{
    public class MemorialMaterialBody
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ColorName { get; set; }
        public string? Image { get; set; }
        public string? Hex { get; set; }
    }
}