using MongoDB.Driver;
using AuctionMS.Domain.Entities.Auction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Infrastructure.Database.Configuration.Mongo
{
    public class AuctionConfigurationMongo
    {
        public static void Configure(IMongoCollection<AuctionEntity> collection)
        {
            // Índice único en AuctionId para evitar duplicados
            var indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionId.Value);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<AuctionEntity>(indexKeysDefinition, indexOptions);
            collection.Indexes.CreateOne(indexModel);

            // Índice en AuctionName para optimizar búsqueda por nombre
            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionName.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductPrice para consultas de rango de precios
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductPrice.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductAvilability (considerando que es un ENUM)
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductAvilability.ToString());
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductStock para optimizar búsquedas por disponibilidad en inventario
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductStock.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en CategoryId para mejorar las relaciones entre categorías y productos
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.CategoryId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductUserId para mejorar consultas por usuario propietario
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductUserId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);
        }
    }
}
