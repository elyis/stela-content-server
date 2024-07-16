using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.Infrastructure.Service
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly string _hostname;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _additionalServiceImageQueue;
        private readonly string _memorialImageQueue;
        private readonly string _portfolioMemorialImageQueue;
        private readonly string _materialImageQueue;

        public RabbitMqBackgroundService(
            IServiceScopeFactory serviceFactory,
            string hostname,
            string userName,
            string password,
            string additionalServiceImageQueue,
            string memorialImageQueue,
            string portfolioMemorialImageQueue,
            string materialImageQueue)
        {
            _hostname = hostname;
            _userName = userName;
            _password = password;
            _serviceFactory = serviceFactory;
            _additionalServiceImageQueue = additionalServiceImageQueue;
            _memorialImageQueue = memorialImageQueue;
            _portfolioMemorialImageQueue = portfolioMemorialImageQueue;
            _materialImageQueue = materialImageQueue;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _userName,
                Password = _password,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            InitQueues();
        }

        private void InitQueues()
        {
            _channel.QueueDeclare(
                queue: _additionalServiceImageQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: _memorialImageQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: _portfolioMemorialImageQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: _materialImageQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            SetupConsumer(_additionalServiceImageQueue, HandleAdditionalServiceImageAsync);
            SetupConsumer(_memorialImageQueue, HandleMemorialUpdateImageAsync);
            SetupConsumer(_portfolioMemorialImageQueue, HandlePortfolioMemorialUpdateImageAsync);
            SetupConsumer(_materialImageQueue, HandleMaterialUpdateImageAsync);

            await Task.CompletedTask;
        }

        private void SetupConsumer(string queueName, Func<string, Task> handleMethod)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await handleMethod(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private async Task ProcessMessage<T>(string message, Func<ContentDbContext, T, Task> updateFunc) where T : class
        {
            using var scope = _serviceFactory.CreateScope();
            var contentDbContext = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
            var updateBody = JsonSerializer.Deserialize<T>(message);
            if (updateBody == null)
                return;

            await updateFunc(contentDbContext, updateBody);
            await contentDbContext.SaveChangesAsync();
        }

        private async Task HandleAdditionalServiceImageAsync(string message)
        {
            await ProcessMessage<AdditionalServiceUpdateImageBody>(message, async (db, body) =>
            {
                var additionalService = await db.AdditionalServices.FindAsync(body.AdditionalServiceId);
                if (additionalService != null)
                    additionalService.Image = body.FileName;
            });
        }

        private async Task HandlePortfolioMemorialUpdateImageAsync(string message)
        {
            await ProcessMessage<PortfolioMemorialUpdateImageBody>(message, async (db, body) =>
            {
                var portfolioMemorial = await db.PortfolioMemorials.FindAsync(body.PortfolioMemorialId);
                if (portfolioMemorial != null)
                    portfolioMemorial.Images = string.Join(';', portfolioMemorial.Images, body.FileName);
            });
        }

        private async Task HandleMaterialUpdateImageAsync(string message)
        {
            await ProcessMessage<MaterialImageBody>(message, async (db, body) =>
            {
                var material = await db.Materials.FindAsync(body.MaterialId);
                if (material != null)
                    material.Image = body.FileName;
            });
        }

        private async Task HandleMemorialUpdateImageAsync(string message)
        {
            await ProcessMessage<MemorialUpdateImageBody>(message, async (db, body) =>
            {
                var memorial = await db.Memorials.FindAsync(body.MemorialId);
                if (memorial != null)
                    memorial.Image = body.FileName;
            });
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
