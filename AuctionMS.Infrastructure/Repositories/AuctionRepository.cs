using AutoMapper;
//using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Database.Context.Postgres;

namespace AuctionMS.Infrastructure.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMongoCollection<AuctionEntity> _collection;
        private readonly IMapper _mapper; // Agregar el Mapper

        public AuctionRepository(IApplicationDbContext dbContext, IMongoCollection<AuctionEntity> collection, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));// Inyectar el Mapper
        }
        public async Task AddAsync(AuctionEntity auction)
        {
            await _dbContext.Auction.AddAsync(auction);
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId, AuctionProductId productId)
        {
            Console.WriteLine($"Buscando una subasta con ID: {id} y usuario: {userId.Value} y producto : {productId.Value}");

            var filters = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionId", id.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value) ,
                  Builders<AuctionEntity>.Filter.Eq("AuctionProductId", productId.Value)
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

        public async Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId, AuctionProductId productId)
        {
            Console.WriteLine($"Buscando subasta con nombre: {name} usuario: {userId.Value} y producto: {productId.Value}");

            var filters = Builders<AuctionEntity>.Filter.And(
                Builders<AuctionEntity>.Filter.Eq("AuctionName", name.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionUserId", userId.Value),
                Builders<AuctionEntity>.Filter.Eq("AuctionProductId", productId.Value)
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

        public async Task DeleteAsync(AuctionId id)
        {
            var auction = await _dbContext.Auction.FirstOrDefaultAsync(x => x.AuctionId == id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Auction.Remove(auction);
            //department.IsDeleted = true;
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<AuctionEntity?> UpdateAsync(AuctionEntity auction)
        {
            _dbContext.Auction.Update(auction);
            await _dbContext.SaveEfContextChanges("");
            return auction;
        }
        public Task<bool> ExistsAsync(AuctionId id)
        {
            return _dbContext.Auction.AnyAsync(x => x.AuctionId == id);
        }

        public async Task<AuctionEntity?> ObtenerSubastaActivaPorProductoAsync(AuctionProductId productId)
        {
            var ahora = DateTime.UtcNow;

            return await _dbContext.Auction
                .Where(a =>
                    a.AuctionProductId.Value == productId.Value &&
                    a.AuctionFechaInicio.Value <= ahora &&
                    a.AuctionFechaFin.Value >= ahora)
                .FirstOrDefaultAsync();
        }



    }
}
