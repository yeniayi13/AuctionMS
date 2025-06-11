using MediatR;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Commands
{
    public class CreateAuctionCommand : IRequest<Guid>
    {
        public CreateAuctionDto Auction { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public CreateAuctionCommand(CreateAuctionDto auction, Guid userId, Guid productId)
        {
            Auction = auction;
            UserId = userId;
            ProductId = productId;
        }
    }
}
