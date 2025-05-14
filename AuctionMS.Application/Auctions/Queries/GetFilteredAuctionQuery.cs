using MediatR;

using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Common.Enum;


namespace AuctionMS.Application.Auctions.Queries
{


    public class GetFilteredAuctionQuery : IRequest<List<GetAuctionDto>>
    {

        public AuctionPriceBase? PriceBase { get; set; }
        public AuctionPriceReserva? PriceReserva { get; set; }

    }

}
