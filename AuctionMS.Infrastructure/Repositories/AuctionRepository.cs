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
            private readonly IMapper _mapper; // Agregar el Mapper

<<<<<<< HEAD
            public AuctionRepository(IApplicationDbContext dbContext, IMapper mapper)
=======
        public AuctionRepository(IApplicationDbContext dbContext, IMongoCollection<AuctionEntity> collection, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));// Inyectar el Mapper
        }
        public async Task AddAsync(AuctionEntity auction)
        {
            await _dbContext.Auctions.AddAsync(auction);
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
>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            }
            public async Task AddAsync(AuctionEntity auction)
            {
                await _dbContext.Auction.AddAsync(auction);
                await _dbContext.SaveEfContextChanges("");
            }


            public async Task DeleteAsync(AuctionId id)
            {
                var auction = await _dbContext.Auction.FindAsync(id);
              
                _dbContext.Auction.Remove(auction);
              
                await _dbContext.SaveEfContextChanges("");
            }

            public async Task<AuctionEntity?> UpdateAsync(AuctionEntity auction)
            {
                _dbContext.Auction.Update(auction);
                await _dbContext.SaveEfContextChanges("");
                return auction;
            }

<<<<<<< HEAD
        
=======
            var auctionEntities = _mapper.Map<List<AuctionEntity>>(auctionDto);

            return auctionEntities;
        }

        public async Task DeleteAsync(AuctionId id)
        {
            var auction = await _dbContext.Auctions.FirstOrDefaultAsync(x => x.AuctionId == id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Auctions.Remove(auction);
            //department.IsDeleted = true;
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<AuctionEntity?> UpdateAsync(AuctionEntity auction)
        {
            _dbContext.Auctions.Update(auction);
            await _dbContext.SaveEfContextChanges("");
            return auction;
        }
        public Task<bool> ExistsAsync(AuctionId id)
        {
            return _dbContext.Auctions.AnyAsync(x => x.AuctionId == id);
        }

        public async Task<AuctionEntity?> ObtenerSubastaActivaPorProductoAsync(AuctionProductId productId)
        {
            var ahora = DateTime.UtcNow;

            return await _dbContext.Auctions
                .Where(a =>
                    a.AuctionProductId.Value == productId.Value &&
                    a.AuctionFechaInicio.Value <= ahora &&
                    a.AuctionFechaFin.Value >= ahora)
                .FirstOrDefaultAsync();
        }

>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)


    }
}

