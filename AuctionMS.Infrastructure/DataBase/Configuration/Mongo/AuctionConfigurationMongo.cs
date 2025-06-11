using MongoDB.Driver;
using AuctionMS.Domain.Entities.Auction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Infrastructure.Database.Configuration.Mongo
{
    [ExcludeFromCodeCoverage]
    public class AuctionConfigurationMongo
    {
        public static void Configure(IMongoCollection<AuctionEntity> collection)
        {
      
            var indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionId.Value);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<AuctionEntity>(indexKeysDefinition, indexOptions);
            collection.Indexes.CreateOne(indexModel);

            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionName.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            
            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionPriceBase.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionPriceReserva.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);


            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionCondiciones.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);


            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionIncremento.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);
          
            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionFechaInicio.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);


            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionFechaFin.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(a => a.AuctionCantidadProducto.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en AuctionUserId y AuctionProductId para mejorar consultas por usuario propietario

            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(p => p.AuctionProductId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            indexKeysDefinition = Builders<AuctionEntity>.IndexKeys.Ascending(p => p.AuctionUserId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);
        }
    }
}
