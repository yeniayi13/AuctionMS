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
        public Event<ActivateAuctionEvent> ActivateAuction { get; private set; } = null!;
        public Event<BidPlacedEvent> BidPlaced { get; private set; } = null!;
        public Event<AuctionEndedEvent> AuctionEnded { get; private set; } = null!;
        public Event<PaymentReceivedEvent> PaymentReceived { get; private set; } = null!;
        public Event<AuctionCanceledEvent> AuctionCanceled { get; private set; } = null!;

        public MaquinaEstadoAuction()
        {
            InstanceState(x => x.CurrentState);

            Event(() => AuctionStarted, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
            Event(() => ActivateAuction, x => x.CorrelateById(ctx => ctx.Message.AuctionId));
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
                        ctx.Saga.FechaInicio = ctx.Message.AuctionFechaInicio;
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
              When(ActivateAuction)
                .If(
                    ctx => ctx.Message.FechaActivacion >= ctx.Saga.FechaInicio,
                    binder => binder
                        .Then(ctx => Console.WriteLine($"[ACTIVATE] Activando subasta programada. AuctionId: {ctx.Saga.AuctionId}"))
                        .ThenAsync(async ctx =>
                        {
                            await ctx.Publish(new AuctionStateChangedEvent(
                                ctx.Saga.AuctionId,
                                nameof(Active),
                                ctx.Message.FechaActivacion
                            ));
                        })
                        .TransitionTo(Active)
                ),

                    When(AuctionCanceled)
                        .Then(ctx => Console.WriteLine("Subasta cancelada"))
                        .TransitionTo(Canceled)
              );

            During(Active,
                When(BidPlaced)
                    .Then(ctx => Console.WriteLine($"[BID] Puja recibida en subasta activa. AuctionId: {ctx.Saga.AuctionId}")),

                When(AuctionEnded)
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
