using AutoMapper;
using MongoDB.Driver;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Database;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Core.Repository;
using MongoDB.Bson;

namespace AuctionMS.Infrastructure.Repositories
{
    public class AuctionRepositoryMongo : IAuctionRepositoryMongo
    {
        private readonly IMongoCollection<AuctionEntity> _collection;
        private readonly IMapper _mapper; // Agregar el Mapper

        public AuctionRepositoryMongo(IMongoCollection<AuctionEntity> collection, IMapper mapper)
        {

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));// Inyectar el Mapper
        }


        public async Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId)
        {
            Console.WriteLine($"Buscando una subasta con ID: {id} y usuario: {userId.Value} ");

            var filters = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionId", id.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value)
            );

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filters)
                .Project<GetAuctionDto>(projection) // Convertir el resultado al DTO
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (auctionDto == null)
            {
                Console.WriteLine("Subasta no encontrada para este usuario.");
                return null;
            }

            var auctionEntity = _mapper.Map<AuctionEntity>(auctionDto);
            return auctionEntity;
        }

        public async Task<AuctionEntity?> GetByIdAsync(AuctionId id)
        {
            Console.WriteLine($"Buscando una subasta con ID: {id} ");

            var filters = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionId", id.Value)
               
            );

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filters)
                .Project<GetAuctionDto>(projection) // Convertir el resultado al DTO
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

           
            var auctionEntity = _mapper.Map<AuctionEntity>(auctionDto);
            return auctionEntity;
        }


        public async Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId)
        {
            Console.WriteLine($"Buscando subasta con nombre: {name} usuario: {userId.Value} ");

            var filters = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionName", name.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value)
            );

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filters)
                .Project<GetAuctionDto>(projection) // Convertir el resultado al DTO
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (auctionDto == null)
            {
                Console.WriteLine("Subasta no encontrada para este usuario.");
                return null;
            }

            var auctionEntity = _mapper.Map<AuctionEntity>(auctionDto);
            return auctionEntity;
        }

        public async Task<List<AuctionEntity>> GetAllAsync(AuctionUserId userId)
        {
            Console.WriteLine($"Consulta de subastas en proceso para el usuario: {userId.Value}");

            var filter = Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value); // Filtrar por usuario propietario

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filter) // Aplicamos el filtro
                .Project<GetAuctionDto>(projection) // Convertir los datos al DTO
                .ToListAsync()
                .ConfigureAwait(false);

            if (auctionDto == null || auctionDto.Count == 0)
            {
                Console.WriteLine("No se encontraron subastas para este usuario.");
                return new List<AuctionEntity>(); // Retorna una lista vacía en lugar de `null` para evitar errores
            }

            var auctionEntities = _mapper.Map<List<AuctionEntity>>(auctionDto);

            return auctionEntities;
        }

        public async Task<AuctionEntity?> ObtenerSubastaActivaPorProductoAsync(AuctionProductId productId, AuctionUserId userId)
        {
            Console.WriteLine($"Buscando subasta activa para el producto con ID: {productId.Value}");

            var ahora = DateTime.UtcNow;

            var filter = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionProductId", productId.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value)
                //Builders<AuctionEntity>.Filter.Lte("AuctionFechaInicio", ahora),
                //Builders<AuctionEntity>.Filter.Gte("AuctionFechaFin", ahora)
            );
            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var subastaActiva = await _collection
                .Find(filter)
                .Project<GetAuctionDto>(projection)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (subastaActiva == null)
            {
                Console.WriteLine("No se encontró ninguna subasta activa para este producto.");
            }
            else
            {
                Console.WriteLine($"Subasta activa encontrada con ID: {subastaActiva.AuctionId}");
            }
            var auctionEntity = _mapper.Map<AuctionEntity>(subastaActiva);
            return auctionEntity;
        }

        //PUJA
        public async Task<AuctionEntity?> GetBidByIdAndAuctionIdAsync(AuctionBidId auctionBidId)
        {
            Console.WriteLine($"Buscando subasta que contiene la puja con ID: {auctionBidId.Value}");

            var filter = Builders<AuctionEntity>.Filter.And(
               Builders<AuctionEntity>.Filter.Eq("AuctionBidId", auctionBidId.Value));

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var subasta = await _collection
                .Find(filter)
                .Project<GetAuctionDto>(projection)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (subasta == null)
            {
                Console.WriteLine("No se encontró ninguna subasta que contenga esa puja.");
            }
            else
            {
                Console.WriteLine($"Subasta encontrada con ID: {subasta.AuctionId}");
            }

            var auctionEntity = _mapper.Map<AuctionEntity>(subasta);
            return auctionEntity;
        }

        public async Task<List<AuctionEntity>> GetAllByEstadoAsync(string estado)
        {
            var filter = Builders<AuctionEntity>.Filter.Regex(
                "AuctionEstado",
                new BsonRegularExpression($"^{estado}", "i") // 🎯 Permite búsqueda insensible a mayúsculas
            );

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filter)
                .Project<GetAuctionDto>(projection)
                .ToListAsync()
                .ConfigureAwait(false);

            if (auctionDto == null || auctionDto.Count == 0)
            {
                Console.WriteLine($"[BUSQUEDA] No se encontraron subastas con estado: {estado}");
                return new List<AuctionEntity>();
            }

            return _mapper.Map<List<AuctionEntity>>(auctionDto);
        }

        public async Task<List<AuctionEntity>> GetByEstadoAsync(AuctionEstado estado)
        {
            var filter = Builders<AuctionEntity>.Filter.Eq("AuctionEstado", estado.Value);

            var projection = Builders<AuctionEntity>.Projection.Exclude("_id");

            var auctionDto = await _collection
                .Find(filter)
                .Project<GetAuctionDto>(projection)
                .ToListAsync()
                .ConfigureAwait(false);

            if (auctionDto == null || auctionDto.Count == 0)
            {
                Console.WriteLine($"[BUSQUEDA] No se encontraron subastas con estado: {estado.Value}");
                return new List<AuctionEntity>();
            }

            return _mapper.Map<List<AuctionEntity>>(auctionDto);
        }






    }
}