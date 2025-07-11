using AutoMapper;
using FluentValidation;
using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Service.Auction;
using AuctionMS.Core.Service.User;
using MassTransit;
using AuctionMS.Application.Saga.Events;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using AuctionMS.Infrastructure.Repositories;
using Firebase.Auth;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class CancelAuctionCommandHandler : IRequestHandler<CancelAuctionCommand, Unit>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionRepositoryMongo _auctionRepositoryMongo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventBus<GetAuctionDto> _eventBus;

        public CancelAuctionCommandHandler(
            IMapper mapper,
            IUserService userService,
            IAuctionRepository auctionRepository,
              IAuctionRepositoryMongo auctionRepositoryMongo,
                IEventBus<GetAuctionDto> eventBus,
            IPublishEndpoint publishEndpoint)
         
        {
            _auctionRepository = auctionRepository;
            _userService = userService;
            _mapper = mapper;
            _eventBus = eventBus;
            _publishEndpoint = publishEndpoint;
            _auctionRepositoryMongo = auctionRepositoryMongo;
        }

        public async Task<Unit> Handle(CancelAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.AuctioneerExists(request.UserId);
                if (user == null)
                    throw new ValidationException($"El usuario con ID {request.UserId} no existe.");

                var auction = await _auctionRepositoryMongo.GetByIdAsync(AuctionId.Create(request.AuctionId)!);
                if (auction == null)
                    throw new ValidationException($"La subasta con ID {request.AuctionId} no fue encontrada.");

                if (auction.AuctionUserId.Value != request.UserId)
                    throw new ValidationException("No tienes permisos para cancelar esta subasta.");

               auction.AuctionEstado = AuctionEstado.Create("Canceled");


                await _auctionRepository.UpdateAsync(auction);

                var auctionDto = _mapper.Map<GetAuctionDto>(auction);

                await _eventBus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_UPDATED");

                await _publishEndpoint.Publish(new AuctionCanceledEvent
                (
                    auction.AuctionId.Value,
                    DateTime.UtcNow
                ));

                return Unit.Value;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Cancelación de subasta fallida: {ex.Message}");
                throw new ApplicationException("No se pudo cancelar la subasta.", ex);
            }
        }
    }
}
