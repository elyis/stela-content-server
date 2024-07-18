namespace STELA_CONTENT.Core.IService
{
    public interface ICacheService
    {
        Task<T?> GetCache<T>(string key);
        Task SetCache<T>(string key, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration);
        Task RemoveCache(string key);
    }
}