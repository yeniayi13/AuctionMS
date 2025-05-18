
using MediatR;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction.ValueObjects;



namespace AuctionMS.Application.Auctions.Queries
{
    public class GetAuctionQuery : IRequest<GetAuctionDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public GetAuctionQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}


        

    



