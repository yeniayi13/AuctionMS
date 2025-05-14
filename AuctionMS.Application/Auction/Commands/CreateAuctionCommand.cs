using MediatR;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Commands
{
    public class CreateAuctionCommand : IRequest<Guid>
    {
        public CreateAuctionDto Product { get; set; }

        public CreateAuctionMSCommand(CreateAuctionMSDto auction)
        {
            Auction = auction;
        }
    }
}
