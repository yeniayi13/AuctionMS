
using MediatR;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction.ValueObjects;



namespace AuctionMS.Application.Auctions.Queries
{
    public class GetAuctionQuery : IRequest<GetAuctionDto>
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public GetAuctionQuery( Guid userId, Guid id)
        {
            
            UserId = userId;
            Id = id;
        }
    }
}


        

    



