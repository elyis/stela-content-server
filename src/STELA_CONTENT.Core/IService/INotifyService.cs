namespace STELA_CONTENT.Core.IService
{
    public interface INotifyService
    {
        void Publish<T>(T message, string queueName);
    }
}