using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Repository;
using AutoMapper;
using MediatR;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionFilteredQueryHandler : IRequestHandler<GetAuctionFilteredQuery, List<GetAuctionDto>>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository;
        private readonly IMapper _mapper;

        public GetAuctionFilteredQueryHandler(IAuctionRepositoryMongo auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAuctionDto>> Handle(GetAuctionFilteredQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var auctions = await _auctionRepository.GetAuctionFilteredAsync(
                    request.StartDate,
                    request.EndDate
                );

                if (auctions == null)
                {
                    return new List<GetAuctionDto>();
                }

                var auctionDtoList = _mapper.Map<List<GetAuctionDto>>(auctions);

                return auctionDtoList;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Parámetro inválido en el filtro de subastas.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al obtener las subastas filtradas.", ex);
            }

        }
    }
}



