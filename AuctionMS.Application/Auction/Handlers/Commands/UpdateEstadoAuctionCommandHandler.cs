using System;
using System.Threading;
using System.Threading.Tasks;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Core.RabbitMQ;
using MediatR;
using AuctionMS.Application.Saga.Events;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class UpdateEstadoAuctionCommandHandler : IRequestHandler<UpdateEstadoAuctionCommand, Unit>
    {
        private readonly IEventBus<AuctionStateChangedEvent> _eventBus;

        public UpdateEstadoAuctionCommandHandler(IEventBus<AuctionStateChangedEvent> eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<Unit> Handle(UpdateEstadoAuctionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var auctionEstado = AuctionEstado.Create(request.NuevoEstado);

            var estadoEvent = new AuctionStateChangedEvent(
                request.AuctionId,
                auctionEstado.Value,
                DateTime.UtcNow
            );

            await _eventBus.PublishMessageAsync(
                estadoEvent,
                "auctionStateQueue",
                "AUCTION_STATE_UPDATED"
            );

            return Unit.Value;
        }
    }
}
