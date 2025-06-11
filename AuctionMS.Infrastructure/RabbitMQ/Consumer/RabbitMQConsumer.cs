//using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using MongoDB.Driver;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Infrastructure.RabbitMQ.Consumer;
using AuctionMS.Core.RabbitMQ;

namespace AuctionMS.Infrastructure.RabbitMQ.Consumer
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IConnectionRabbbitMQ _rabbitMQConnection;
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<GetAuctionDto> _collection;
        public RabbitMQConsumer(IConnectionRabbbitMQ rabbitMQConnection, IMongoCollection<GetAuctionDto> collection)
        {
            _rabbitMQConnection = rabbitMQConnection;

            // 🔹 Conexión a MongoDB Atlas
            _mongoClient = new MongoClient("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            _database = _mongoClient.GetDatabase("AuctionMS");
            _collection = collection;

        }

        public RabbitMQConsumer() { }

        public async Task ConsumeMessagesAsync(string queueName)
        {
            var channel = _rabbitMQConnection.GetChannel();
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Mensaje recibido: {message}");

                try
                {
                    var eventMessageD = JsonConvert.DeserializeObject<EventMessage<GetAuctionDto>>(message);
                    if (eventMessageD?.EventType == "AUCTION_CREATED")
                    {
                        await _collection.InsertOneAsync(eventMessageD.Data);
                        Console.WriteLine($"Subasta insertada en MongoDB: {JsonConvert.SerializeObject(eventMessageD.Data)}");
                    }
                    else if (eventMessageD?.EventType == "AUCTION_UPDATED")
                    {
                        var filter = Builders<GetAuctionDto>.Filter.Eq(a => a.AuctionId, eventMessageD.Data.AuctionId);
                        var update = Builders<GetAuctionDto>.Update
                            .Set(a => a.AuctionName, eventMessageD.Data.AuctionName)
                            .Set(a => a.AuctionImage, eventMessageD.Data.AuctionImage)
                            .Set(a => a.AuctionPriceBase, eventMessageD.Data.AuctionPriceBase)
                            .Set(a => a.AuctionPriceReserva, eventMessageD.Data.AuctionPriceReserva)

                            .Set(a => a.AuctionDescription, eventMessageD.Data.AuctionDescription)
                            .Set(a => a.AuctionCondiciones, eventMessageD.Data.AuctionCondiciones)
                            .Set(a => a.AuctionFechaInicio, eventMessageD.Data.AuctionFechaInicio)
                            .Set(a => a.AuctionFechaFin, eventMessageD.Data.AuctionFechaFin)
                            .Set(a => a.AuctionIncremento, eventMessageD.Data.AuctionIncremento)
                             .Set(a => a.AuctionCantidadProducto, eventMessageD.Data.AuctionCantidadProducto)
                            .Set(a => a.AuctionUserId, eventMessageD.Data.AuctionUserId)
                            .Set(a => a.AuctionProductId, eventMessageD.Data.AuctionProductId);





                        await _collection.UpdateOneAsync(filter, update);
                        Console.WriteLine($"Subasta actualizado en MongoDB: {JsonConvert.SerializeObject(eventMessageD.Data)}");
                    }
                    else if (eventMessageD?.EventType == "AUCTION_DELETED")
                    {
                        var filter = Builders<GetAuctionDto>.Filter.Eq("AuctionId", eventMessageD.Data.AuctionId);
                        await _collection.DeleteOneAsync(filter);
                        Console.WriteLine($"Subasta eliminado en MongoDB con ID: {eventMessageD.Data.AuctionId}");
                    }
                   


                    await Task.Run(() => channel.BasicAckAsync(ea.DeliveryTag, false));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando el mensaje: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine("Consumidor de RabbitMQ escuchando mensajes...");
        }
    }
}