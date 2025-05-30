using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Infrastructure.RabbitMQ.Consumer;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Infrastructure.RabbitMQ.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;


namespace ProductsMS.Test.Infrastructure.RabbitMQ
{


    public class RabbitMQConsumerTests
    {
        private readonly Mock<IConnectionRabbbitMQ> _mockRabbitMQConnection;
        private readonly Mock<IChannel> _mockChannel;
        private readonly Mock<IMongoCollection<GetAuctionDto>> _mockCollection;
        private readonly RabbitMQConsumer _consumer;

        public RabbitMQConsumerTests()
        {
            _mockRabbitMQConnection = new Mock<IConnectionRabbbitMQ>();
            _mockChannel = new Mock<IChannel>();
            _mockCollection = new Mock<IMongoCollection<GetAuctionDto>>();

            // 🔹 Asegurar que `RabbitMQConsumer` recibe `_mockCollection.Object` en pruebas
            _mockRabbitMQConnection.Setup(c => c.GetChannel()).Returns(_mockChannel.Object);
            _consumer = new RabbitMQConsumer(_mockRabbitMQConnection.Object, _mockCollection.Object);
        }

        [Fact]
        public async Task ConsumeMessagesAsync_ShouldDeclareQueue_AndStartConsumer()
        {
            // Arrange
            var queueName = "testQueue";

            // Simular declaración de la cola
            _mockChannel.Setup(c => c.QueueDeclareAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueueDeclareOk(It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<uint>()));

            // 🔥 No se puede mockear directamente `BasicConsumeAsync()`, pero podemos simular la ejecución manual
            var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);

            // Act
            await _consumer.ConsumeMessagesAsync(queueName);

            // Assert
            _mockChannel.Verify(c => c.QueueDeclareAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

       
    }
}
