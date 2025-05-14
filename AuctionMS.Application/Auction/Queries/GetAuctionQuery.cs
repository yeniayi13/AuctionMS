
using MediatR;
using AuctionMS.Common.Dtos.Auction.Response;
using ProductsMS.Common.Dtos.Auction.Response;



namespace AuctionMS.Application.Auctions.Queries
{
    public class GetAuctionQuery : IRequest<GetAuctionDto>
    {
        public Guid Id { get; set; }

        public GetAuctionQuery(Guid id)
        {
            Id = id;
        }
    }
}




