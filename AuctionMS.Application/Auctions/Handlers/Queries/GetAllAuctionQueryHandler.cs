using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Common.Exceptions;
using AuctionMS.Application.Auctions.Queries;
using AuctionMS.Infrastructure.Repositories;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAllAuctionQueryHandler : IRequestHandler<GetAllAuctionQuery, List<GetAuctionDto>>
    {
        public IAuctionRepository _auctionRepository;

        public GetAllAuctionQueryHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<List<GetAuctionDto>> Handle(GetAllAuctionQuery request, CancellationToken cancellationToken)
        {
            var auction = await _auctionRepository.GetAllAsync();

            if (auction == null) throw new AuctionNotFoundException("Auctions are empty");

            return auction.Where(a => !a.IsDeleted).Select(a =>
                new GetAuctionDto(
                    a.Id.Value,
                    a.Name.Value,
                    a.Image.Url,
                    a.PriceBase.Value,
                    a.PriceReserva.Value,
                    a.Description.Value,
                    a.Incremento.Value,
                    a.Duracion.Value,
                    a.Condiciones.Value,


                )
            ).ToList();
        }
    }
}


