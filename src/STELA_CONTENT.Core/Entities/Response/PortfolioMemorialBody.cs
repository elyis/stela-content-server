namespace STELA_CONTENT.Core.Entities.Response
{
    public class BasePortfolioMemorialBody
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Images { get; set; } = new();
        public string CemeteryName { get; set; }

        public List<MemorialMaterialBody> Materials { get; set; } = new();
    }

    public class PortfolioMemorialBody : BasePortfolioMemorialBody
    {
        public string Description { get; set; }
    }

    public class ShortPortfolioMemorialBody : BasePortfolioMemorialBody
    {
    }
}