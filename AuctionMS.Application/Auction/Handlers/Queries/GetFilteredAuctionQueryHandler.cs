using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Application.Auctions.Queries;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetFilteredAuctionQueryHandler : IRequestHandler<GetFilteredAuctionQuery, List<GetAuctionDto>>
    {
        private readonly IAuctionRepository _auctionRepository;

        public GetFilteredAuctionQueryHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<List<GetAuctionDto>> Handle(GetFilteredAuctionQuery request, CancellationToken cancellationToken)
        {
            var auction = await _auctionRepository.GetFilteredAuctionAsync(

                request.PriceBase
                request.PriceReserva,


            );

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
