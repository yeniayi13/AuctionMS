using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Infrastructure.Database.Context.Postgres;
using global::AuctionMS.Core.Database;
using global::AuctionMS.Domain.Entities.Auction;

namespace AuctionMS.Infrastructure.Repositories
    {
        public class ClaimPrizeRepository : IClaimPrizeRepository
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public ClaimPrizeRepository(IApplicationDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

          /*  public async Task AddAsync(ClaimPrizeAuction claim)
            {
                await _dbContext.PrizeClaims.AddAsync(claim);
                await _dbContext.SaveEfContextChanges("");
            }

            public async Task<List<ClaimPrizeAuction>> GetByUserIdAsync(Guid userId)
            {
                return await _dbContext.PrizeClaims
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
            }

            public async Task<ClaimPrizeAuction?> GetByAuctionIdAsync(Guid auctionId)
            {
                return await _dbContext.PrizeClaims
                    .FirstOrDefaultAsync(c => c.AuctionId == auctionId);
            }*/

           
        }
    

}
