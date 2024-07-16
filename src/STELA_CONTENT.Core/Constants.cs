namespace STELA_CONTENT.Core
{
    public static class Constants
    {
        public static readonly string FileServerUrl = Environment.GetEnvironmentVariable("FILE_SERVER_URL") ?? throw new Exception("FILE_SERVER_URL is not found in enviroment variables");
        public static readonly string WebUrlToMemorialImage = $"{FileServerUrl}/api/memorial/download";
        public static readonly string WebUrlToMaterialImage = $"{FileServerUrl}/api/material/download";
        public static readonly string WebUrlToPortfolioMemorialImage = $"{FileServerUrl}/api/portfolio-memorial/download";
        public static readonly string WebUrlToAdditionalServiceImage = $"{FileServerUrl}/api/additional-service/download";
    }
}