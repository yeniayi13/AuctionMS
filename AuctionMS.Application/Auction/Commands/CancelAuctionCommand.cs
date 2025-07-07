using MediatR;
using System;

namespace AuctionMS.Application.Auction.Commands
{
    public class CancelAuctionCommand : IRequest<Unit>
    {
        public Guid AuctionId { get; set; }
        public Guid UserId { get; set; }

        public CancelAuctionCommand(Guid auctionId, Guid userId)
        {
            AuctionId = auctionId;
            UserId = userId;
        }
    }
}
