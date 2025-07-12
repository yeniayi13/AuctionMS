using System;
using MediatR;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetBidAuctionQuery : IRequest<GetAuctionDto>
    {
        public Guid AuctionBidId { get; }
        public Guid AuctionId { get; }

        public GetBidAuctionQuery(Guid auctionBidId, Guid auctionId)
        {
            AuctionBidId = auctionBidId;
            AuctionId = auctionId;
        }
    }
}
