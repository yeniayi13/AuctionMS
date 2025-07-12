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
using FluentValidation;
using AuctionMS.Application.Auction.Validators.Auctions;

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
            try
            {
                var allowedStates = new[] { "Pending", "Canceled", "Active", "Completed", "Ended" };
                if (!allowedStates.Contains(request.Estado, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidStateException($"El Estado '{request.Estado}' es invalido");


                //  Crear estado del dominio
                var auctionState = AuctionEstado.Create(request.Estado);

                //  Obtener subastas según estado
                var auctions = await _auctionRepository.GetByEstadoAsync(auctionState);

                //  Mapear a DTOs
                var auctionDtos = _mapper.Map<List<GetAuctionDto>>(auctions);

                return auctionDtos;
            }
            catch (ValidationException ex)
            {

                throw;
            }
            catch (InvalidStateException ex)
            {
                // 🔥 Estado inválido
                throw ;
            }
            catch (Exception ex)
            {
                // ⚠️ Falla inesperada
                throw;
            }
        }
    }
}
