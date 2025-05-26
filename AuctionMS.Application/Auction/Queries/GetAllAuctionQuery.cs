using MediatR;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetAllAuctionQuery : IRequest<List<GetAuctionDto>>
    {

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public GetAllAuctionQuery(Guid userId, Guid productId)
        {
            UserId = userId;
            ProductId = productId;
        }
    }
}
