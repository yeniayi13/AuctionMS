using MediatR;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Commands
{
    public class UpdateAuctionCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public UpdateAuctionDto Auction;

        public UpdateAuctionCommand(Guid id, UpdateAuctionDto auction)
        {
            Id = id;
            Auction = auction;
        }
    }
}
