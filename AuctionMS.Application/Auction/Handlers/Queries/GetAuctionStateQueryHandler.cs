using MediatR;
using AuctionMS.Application.Auctions.Queries;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using AuctionMS.Application.Auction.Queries;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionStateQueryHandler : IRequestHandler<GetAuctionStateQuery, string>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository;

        public GetAuctionStateQueryHandler(IAuctionRepositoryMongo auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<string> Handle(GetAuctionStateQuery request, CancellationToken cancellationToken)
        {
            if (request.AuctionId == Guid.Empty)
                throw new NullAttributeException("Auction id is required");

            var auctionId = AuctionId.Create(request.AuctionId);

       
            var auction = await _auctionRepository.GetByIdAsync(auctionId);

            if (auction == null)
                throw new AuctionNotFoundException("Auction not found.");

            return auction.AuctionEstado?.Value ?? "Unknown";
        }
    }
}
