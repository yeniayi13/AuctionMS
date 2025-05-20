using MediatR;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Commands
{
    public class CreateAuctionCommand : IRequest<Guid>
    {
        public CreateAuctionDto Auction { get; set; }

        public CreateAuctionCommand(CreateAuctionDto auction)
        {
            Auction = auction;
        }
    }
}
