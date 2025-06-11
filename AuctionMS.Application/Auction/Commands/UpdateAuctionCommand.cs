using MediatR;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Commands
{
    public class UpdateAuctionCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public UpdateAuctionDto Auction;

        public UpdateAuctionCommand(Guid id, UpdateAuctionDto auction, Guid userId)
        {
            Id = id;
            Auction = auction;
            UserId = userId;
           
        }
    }
}
