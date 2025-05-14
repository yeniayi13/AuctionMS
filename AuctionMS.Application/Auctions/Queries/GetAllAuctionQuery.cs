using MediatR;
using AuctionMs.Common.Dtos.Auction.Response;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetAllAuctionQuery : IRequest<List<GetAuctionDto>>
    {
        public GetAllAuctionQuery() { }
    }
}