using MassTransit;
using AuctionMS.Domain.Entities;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Application.Saga.Events;

namespace AuctionMS.Application.Saga
{
    public class MaquinaEstadoAuction : MassTransitStateMachine<EstadoAuction>
    {
        // --- Estados de la Subasta ---
        public State Pending { get; set; }    // Creada, pero aún no activa.
        public State Active { get; set; }     // La fecha de inicio ha llegado, aceptando pujas.
        public State Ended { get; set; }      // El tiempo de la subasta ha finalizado.
        public State Canceled { get; set; }   // Cancelada antes de su fin natural.
        public State Completed { get; set; }  // Terminó normalmente y ha sido procesada/pagada.

        // --- Eventos que desencadenan cambios ---
        public Event<AuctionCreatedEvent> AuctionCreated { get; set; } // Nuevo evento para la creación inicial
        public Event<AuctionStartedEvent> AuctionStarted { get; set; }
        public Event<AuctionEndedEvent> AuctionEnded { get; set; }
        public Event<AuctionCanceledEvent> AuctionCanceled { get; set; }
        public Event<AuctionCompletedEvent> AuctionCompleted { get; set; } // Nuevo evento para la finalización normal y completa

        public MaquinaEstadoAuction()
        {
            InstanceState(x => x.EstadoActual);

            // Correlación de eventos por AuctionId
            Event(() => AuctionCreated, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionStarted, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionEnded, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionCanceled, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionCompleted, e => e.CorrelateById(m => m.Message.AuctionId));

            // --- Definición de Transiciones ---

            // 1. Estado Inicial: Cuando una subasta es creada por primera vez
            Initially(
                When(AuctionCreated) // Al recibir el evento de creación
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.AuctionId;
                        Console.WriteLine($"Subasta {context.Message.AuctionId} creada. Estado: Pending.");
                    })
                    .TransitionTo(Pending)); // La subasta inicia en estado Pending

            // 2. Comportamiento en el estado 'Pending'
            During(Pending,
                When(AuctionStarted) // Cuando la fecha de inicio llega (evento disparado por un scheduler, por ejemplo)
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} ha iniciado. Estado: Active."))
                    .TransitionTo(Active),

                When(AuctionCanceled) // Si se cancela mientras está pendiente
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} cancelada desde Pending. Estado: Canceled."))
                    .TransitionTo(Canceled));

            // 3. Comportamiento en el estado 'Active'
            During(Active,
                When(AuctionEnded) // Cuando el tiempo de la subasta finaliza
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} ha finalizado. Estado: Ended."))
                    .TransitionTo(Ended),
                When(AuctionCanceled) // Si se cancela mientras está activa
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} cancelada desde Active. Estado: Canceled."))
                    .TransitionTo(Canceled));

            // 4. Comportamiento en el estado 'Ended'
            During(Ended,
                When(AuctionCompleted) // Cuando la subasta se completa (ej. pago procesado, ganador notificado)
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} completada. Estado: Completed."))
                    .TransitionTo(Completed),
                When(AuctionCanceled) // Podrías permitir la cancelación si está en Ended pero no completada
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} con Estado: Canceled."))
                    .TransitionTo(Canceled));


            // 5. Comportamiento en los estados finales 'Canceled' y 'Completed'
            // Normalmente, desde aquí no hay más transiciones.
            During(Canceled,
                Ignore(AuctionCreated),
                Ignore(AuctionStarted),
                Ignore(AuctionEnded),
                Ignore(AuctionCompleted));

            During(Completed,
                Ignore(AuctionCreated),
                Ignore(AuctionStarted),
                Ignore(AuctionEnded),
                Ignore(AuctionCanceled));
        }
    }
}
