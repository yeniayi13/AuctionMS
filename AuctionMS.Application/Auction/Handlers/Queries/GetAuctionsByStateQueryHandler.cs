using AutoMapper;
using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionsByStateQueryHandler : IRequestHandler<GetAuctionsByStateQuery, List<GetAuctionDto>>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository;
        private readonly IMapper _mapper;

        public GetAuctionsByStateQueryHandler(IAuctionRepositoryMongo auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GetAuctionDto>> Handle(GetAuctionsByStateQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Estado))
                throw new NullAttributeException("El estado de la subasta es requerido.");

            var estado = AuctionEstado.Create(request.Estado);
            var subastas = await _auctionRepository.GetByEstadoAsync(estado);

            var subastasDto = _mapper.Map<List<GetAuctionDto>>(subastas);

            return subastasDto;
        }
    }
}
