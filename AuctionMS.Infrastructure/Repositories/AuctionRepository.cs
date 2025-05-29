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


        public AuctionRepository(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task AddAsync(AuctionEntity auction)
        {
            await _dbContext.Auctions.AddAsync(auction);
            await _dbContext.SaveEfContextChanges("");
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
     


    }
}

