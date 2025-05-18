
using Microsoft.Extensions.Hosting;
using AuctionMS.Infrastructure.RabbitMQ.Consumer;

namespace AuctionMS.Infrastructure.RabbitMQ.Connection
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly RabbitMQConsumer _rabbitMQConsumer;

        public RabbitMQBackgroundService(RabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(" Esperando la inicialización de RabbitMQ...");

            await Task.Delay(3000); // Pequeño retraso para asegurar la inicialización
            await _rabbitMQConsumer.ConsumeMessagesAsync("productQueue");

            Console.WriteLine(" Consumidor de RabbitMQ iniciado.");
        }
    }
}
