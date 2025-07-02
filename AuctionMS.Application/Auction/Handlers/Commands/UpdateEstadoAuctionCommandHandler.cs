using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Validators.Auctions;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Application.Auction.Commands;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
        public class UpdateEstadoAuctionCommandHandler : IRequestHandler<UpdateEstadoAuctionCommand, Guid>
        {
            private readonly IEventBus<UpdateEstadoAuctionDto> _eventBus;


            public UpdateEstadoAuctionCommandHandler(IEventBus<UpdateEstadoAuctionDto> eventBus)
            {
                _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            }

            public async Task<Guid> Handle(UpdateEstadoAuctionCommand request, CancellationToken cancellationToken)
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var validator = new UpdateEstadoAuctionValidator();
                var validationResult = await validator.ValidateAsync(request.EstadoDto, cancellationToken);
                if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);

            }


            // Validar que el estado sea uno de los permitidos (por ejemplo)
            var estadosValidos = new[] { "Pending", "Active", "Ended", "Canceled", "Completed" };
                if (!estadosValidos.Contains(request.EstadoDto.NuevoEstado))
                    throw new InvalidOperationException($"Estado no válido: {request.EstadoDto.NuevoEstado}");

                // Publicar evento en bus, el consumer se encargará de procesar y actualizar la saga
                await _eventBus.PublishMessageAsync(request.EstadoDto, "auctionStateQueue", "AUCTION_STATE_UPDATED");

                return request.EstadoDto.CorrelationId;
            }
        }
    }


