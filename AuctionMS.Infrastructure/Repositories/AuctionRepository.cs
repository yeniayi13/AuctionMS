using AutoMapper;
using Firebase.Auth;
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

        public async Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId)
        {
            Console.WriteLine($"Buscando una subasta con ID: {id} y usuario: {userId.Value}");

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

        public async Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId)
        {
            Console.WriteLine($"Buscando subasta con nombre: {name} y usuario: {userId.Value}");

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

        public async Task DeleteAsync(AuctionId id)
        {
            var product = await _dbContext.Auction.FirstOrDefaultAsync(x => x.AuctionId == id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Auction.Remove(product);
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



    }
}
