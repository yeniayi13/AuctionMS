using MassTransit;
using AuctionMS.Domain.Entities;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Application.Saga.Events;

namespace AuctionMS.Application.Saga
{
    public class MaquinaEstadoAuction : MassTransitStateMachine<EstadoAuction>
    {

        public State Pending { get; set; }    
        public State Active { get; set; }    
        public State Ended { get; set; }     
        public State Canceled { get; set; }   
        public State Completed { get; set; }  


        public Event<AuctionCreatedEvent> AuctionCreated { get; set; } 
        public Event<AuctionStartedEvent> AuctionStarted { get; set; }
        public Event<AuctionEndedEvent> AuctionEnded { get; set; }
        public Event<AuctionCanceledEvent> AuctionCanceled { get; set; }
        public Event<AuctionCompletedEvent> AuctionCompleted { get; set; } 

        public MaquinaEstadoAuction()
        {
            InstanceState(x => x.EstadoActual);

            Event(() => AuctionCreated, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionStarted, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionEnded, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionCanceled, e => e.CorrelateById(m => m.Message.AuctionId));
            Event(() => AuctionCompleted, e => e.CorrelateById(m => m.Message.AuctionId));

         
            Initially(
                When(AuctionCreated) 
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.AuctionId;
                        Console.WriteLine($"Subasta {context.Message.AuctionId} creada. Estado: Pending.");
                    })
                    .TransitionTo(Pending)); 

      
            During(Pending,
                When(AuctionStarted)
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} ha iniciado. Estado: Active."))
                    .TransitionTo(Active),

                When(AuctionCanceled)
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} cancelada desde Pending. Estado: Canceled."))
                    .TransitionTo(Canceled));

     
            During(Active,
                When(AuctionEnded) 
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} ha finalizado. Estado: Ended."))
                    .TransitionTo(Ended),
                When(AuctionCanceled) 
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} cancelada desde Active. Estado: Canceled."))
                    .TransitionTo(Canceled));

            During(Ended,
                When(AuctionCompleted) 
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} completada. Estado: Completed."))
                    .TransitionTo(Completed),
                When(AuctionCanceled)
                    .Then(context => Console.WriteLine($"Subasta {context.Message.AuctionId} con Estado: Canceled."))
                    .TransitionTo(Canceled));


           
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
