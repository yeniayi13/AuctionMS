using MassTransit;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Application.Saga.Events;

namespace AuctionMS.Application.Saga
{
    public class MaquinaEstadoAuction : MassTransitStateMachine<EstadoAuction>
    {
        public State Pending { get; private set; } = null!;
        public State Active { get; private set; } = null!;
        public State Ended { get; private set; } = null!;
        public State Canceled { get; private set; } = null!;
        public State Completed { get; private set; } = null!;

        public Event<AuctionStartedEvent> AuctionStarted { get; private set; } = null!;
        public Event<BidPlacedEvent> BidPlaced { get; private set; } = null!;
        public Event<AuctionEndedEvent> AuctionEnded { get; private set; } = null!;
        public Event<PaymentReceivedEvent> PaymentReceived { get; private set; } = null!;
        public Event<AuctionCanceledEvent> AuctionCanceled { get; private set; } = null!;

        public MaquinaEstadoAuction()
        {
            InstanceState(x => x.CurrentState);

            // Correlación por AuctionId
            Event(() => AuctionStarted, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
            Event(() => BidPlaced, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
            Event(() => AuctionEnded, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
            Event(() => PaymentReceived, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
            Event(() => AuctionCanceled, x => x.CorrelateById(ctx => ctx.Message.AuctionId));

            Initially(
                When(AuctionStarted)
                    .Then(ctx =>
                    {
                        ctx.Saga.CorrelationId = ctx.Message.AuctionId;
                        ctx.Saga.AuctionId = ctx.Message.AuctionId;
                        ctx.Saga.FechaInicio = ctx.Message.FechaInicio;
                        Console.WriteLine($"[INIT] Saga creada para AuctionId: {ctx.Message.AuctionId}");
                    })
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Pending),
                            DateTime.UtcNow
                        ));
                    })
                    .TransitionTo(Pending)
            );

            During(Pending,
                When(BidPlaced)
                    .Then(ctx => Console.WriteLine($"[BID] Primera puja recibida. Subasta {ctx.Saga.AuctionId} activa"))
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Active),
                            ctx.Message.FechaBid
                        ));
                    })
                    .TransitionTo(Active)
            );

            During(Active,
                When(BidPlaced)
                    .Then(ctx =>
                    {
                        Console.WriteLine($"[BID] Nueva puja recibida en subasta {ctx.Saga.AuctionId}");
                        // Aquí no cambia estado
                    }),

                When(AuctionEnded)
                    .Then(ctx => Console.WriteLine($"[END] Subasta finalizada. Id: {ctx.Saga.AuctionId}"))
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Ended),
                            ctx.Message.FechaFin
                        ));
                    })
                    .TransitionTo(Ended),

                When(AuctionCanceled)
                    .Then(ctx => Console.WriteLine($"[CANCEL] Subasta cancelada desde Active. Id: {ctx.Saga.AuctionId}"))
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Canceled),
                            DateTime.UtcNow
                        ));
                    })
                    .TransitionTo(Canceled)
            );

            During(Ended,
                When(PaymentReceived)
                    .Then(ctx => Console.WriteLine($"[PAYMENT] Pago recibido. Id: {ctx.Saga.AuctionId}"))
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Completed),
                            ctx.Message.FechaPayment
                        ));
                    })
                    .TransitionTo(Completed),

                When(AuctionCanceled)
                    .Then(ctx => Console.WriteLine($"[CANCEL] Subasta cancelada desde Ended. Id: {ctx.Saga.AuctionId}"))
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Publish(new AuctionStateChangedEvent(
                            ctx.Saga.AuctionId,
                            nameof(Canceled),
                            DateTime.UtcNow
                        ));
                    })
                    .TransitionTo(Canceled)
            );

            During(Canceled,
                Ignore(AuctionStarted),
                Ignore(BidPlaced),
                Ignore(AuctionEnded),
                Ignore(PaymentReceived)
            );

            During(Completed,
                Ignore(AuctionStarted),
                Ignore(BidPlaced),
                Ignore(AuctionEnded),
                Ignore(AuctionCanceled),
                Ignore(PaymentReceived)
            );
        }
    }
}
