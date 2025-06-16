using MassTransit;
using AuctionMS.Application.Auction.Saga.Events.Eventos;
using AuctionMS.Domain.Entities;

namespace AuctionMS.Application.Saga
{
    public class MaquinaEstadoAuction : MassTransitStateMachine<EstadoAuction>
    {
        public State Pending { get; set; }
        public State Active { get; set; }
        public State Ended { get; set; }
        public State Canceled { get; set; }
        public State Completed { get; set; }

        public Event<AuctionStartedEvent> AuctionStarted { get; set; }
        public Event<BidPlacedEvent> BidPlaced { get; set; }
        public Event<AuctionEndedEvent> AuctionEnded { get; set; }
        public Event<PaymentReceivedEvent> PaymentReceived { get; set; }
        public Event<AuctionCanceledEvent> AuctionCanceled { get; set; }

        public MaquinaEstadoAuction()
        {
            InstanceState(x => x.EstadoActual);

            Event(() => AuctionStarted, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => BidPlaced, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionEnded, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => PaymentReceived, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionCanceled, e => e.CorrelateById(m => m.Message.AuctionId));

            Initially(
                When(AuctionStarted)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.AuctionId;
                        context.Saga.UltimaActualizacion = DateTime.UtcNow;
                    })
                    .TransitionTo(Active));

            During(Active,
                When(BidPlaced)
                    .Then(context =>
                    {
                        context.Saga.UltimaActualizacion = DateTime.UtcNow;
                    }),
                When(AuctionEnded)
                    .Then(context =>
                    {
                        context.Saga.UltimaActualizacion = DateTime.UtcNow;
                    })
                    .TransitionTo(Ended),
                When(AuctionCanceled)
                    .Then(context =>
                    {
                        context.Saga.UltimaActualizacion = DateTime.UtcNow;
                    })
                    .TransitionTo(Canceled));

            During(Ended,
                When(AuctionCanceled)
                    .Then(context =>
                    {
                        Console.WriteLine("No se puede cancelar una subasta ya finalizada.");
                    }),
                When(PaymentReceived)
                    .Then(context =>
                    {
                        context.Saga.UltimaActualizacion = DateTime.UtcNow;
                    })
                    .TransitionTo(Completed));
        }
    }
}
