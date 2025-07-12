using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AutoMapper;
using MongoDB.Driver;

namespace AuctionMS.Infrastructure.Repositories
{
    public class ClaimPrizeRepositoryMongo : IClaimPrizeRepository
        {
            private readonly IMongoCollection<ClaimPrizeAuction> _collection;
            private readonly IMapper _mapper;

            public ClaimPrizeRepositoryMongo(IMongoCollection<ClaimPrizeAuction> collection, IMapper mapper)
            {
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<List<ClaimPrizeAuction>> GetByUserIdAsync(Guid userId)
            {
                Console.WriteLine($"Buscando reclamos de premio del usuario: {userId}");

                var filter = Builders<ClaimPrizeAuction>.Filter.Eq("UserId", userId);
                var projection = Builders<ClaimPrizeAuction>.Projection.Exclude("_id");

                var claimDtos = await _collection
                    .Find(filter)
                    .Project<GetClaimPrizeDto>(projection)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return _mapper.Map<List<ClaimPrizeAuction>>(claimDtos);
            }

            public async Task<ClaimPrizeAuction?> GetByAuctionIdAsync(Guid auctionId)
            {
                Console.WriteLine($"Buscando reclamo de premio para subasta: {auctionId}");

                var filter = Builders<ClaimPrizeAuction>.Filter.Eq("AuctionId", auctionId);
                var projection = Builders<ClaimPrizeAuction>.Projection.Exclude("_id");

                var dto = await _collection
                    .Find(filter)
                    .Project<ClaimPrizeAuction>(projection)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                return dto == null ? null : _mapper.Map<ClaimPrizeAuction>(dto);
            }

            public async Task<List<ClaimPrizeAuction>> GetAllAsync()
            {
                Console.WriteLine($"Buscando todos los reclamos registrados");

                var projection = Builders<ClaimPrizeAuction>.Projection.Exclude("_id");

                var claimDtos = await _collection
                    .Find(Builders<ClaimPrizeAuction>.Filter.Empty)
                    .Project<GetClaimPrizeDto>(projection)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return _mapper.Map<List<ClaimPrizeAuction>>(claimDtos);
            }

            public async Task AddAsync(ClaimPrizeAuction claim)
            {
                Console.WriteLine($"Registrando nuevo reclamo de premio para subasta: {claim.AuctionId}");
                await _collection.InsertOneAsync(claim);
            }
        }
    }



