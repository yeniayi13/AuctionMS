
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Primitives;


namespace AuctionMS.Domain.Entities.Auction
{

    public sealed class AuctionEntity : AggregateRoot
    {
        public enum AuctionState
        {
            // 0: Estado inicial: la subasta ha sido creada pero aún no está lista para pujas.
            Pending = 0,

            // 1: La subasta está activa y los usuarios pueden pujar.
            Active = 1,

            // 2: El período de pujas ha terminado
            Ended = 2,

            // 3: La subasta ha finalizado completamente, el producto ha sido asignado y/o pagado.
            Completed = 3,

            // 4: La subasta ha sido cancelada
            Canceled = 4
        }

    
        public AuctionId AuctionId { get; private set; }
        public AuctionName AuctionName { get; private set; }
        public AuctionImage AuctionImage { get; private set; }
        public AuctionPriceBase AuctionPriceBase { get; private set; }
        public AuctionPriceReserva AuctionPriceReserva { get; private set; }
        public AuctionDescription AuctionDescription { get; private set; }
        public AuctionIncremento AuctionIncremento { get; private set; }

        public AuctionFechaInicio AuctionFechaInicio { get; private set; }
        public AuctionFechaFin AuctionFechaFin { get; private set; }

        public AuctionCondiciones AuctionCondiciones { get; private set; }

        public AuctionCantidadProducto AuctionCantidadProducto { get; private set; }

        
        public AuctionState CurrentState { get; private set; }

        public AuctionUserId AuctionUserId { get; private set; } //FK

        public AuctionProductId AuctionProductId { get; private set; } //FK

        public AuctionBidId AuctionBidId { get; private set; } //FK




        public AuctionEntity (AuctionId auctionId, AuctionName auctionName, AuctionImage auctionImage, AuctionPriceBase auctionPriceBase,
            AuctionPriceReserva auctionPriceReserva, AuctionDescription auctionDescription, AuctionIncremento auctionIncremento,
           AuctionCantidadProducto auctionCantidadProducto, AuctionFechaInicio auctionFechaInicio, AuctionFechaFin auctionFechaFin, AuctionCondiciones auctionCondiciones, AuctionUserId auctionUserId,
           AuctionProductId auctionProductId, AuctionBidId auctionBidId)
        {
            AuctionId = auctionId;
            AuctionName = auctionName;
            AuctionImage = auctionImage;
            AuctionPriceBase = auctionPriceBase;
            AuctionPriceReserva = auctionPriceReserva;
            AuctionDescription = auctionDescription;
            AuctionIncremento = auctionIncremento;
            AuctionCantidadProducto = auctionCantidadProducto;
            AuctionFechaInicio = auctionFechaInicio;
            AuctionFechaFin = auctionFechaFin;
            AuctionCondiciones = auctionCondiciones;
            AuctionUserId = auctionUserId;
            AuctionProductId = auctionProductId;
            AuctionBidId = auctionBidId;

            // --- Inicializa el estado de la subasta al crearla ---
            CurrentState = AuctionState.Pending;
        


        }

        public AuctionEntity() { }


        //actualiza las propiedades de un objeto
        public static AuctionEntity Update(AuctionEntity auction, AuctionName name, AuctionImage image, 
            AuctionPriceBase priceBase, AuctionPriceReserva priceReserva, AuctionDescription description, 
            AuctionIncremento incremento, AuctionCantidadProducto auctionCantidadProducto, AuctionFechaInicio auctionFechaInicio, AuctionFechaFin auctionFechaFin, AuctionCondiciones condiciones, AuctionUserId auctionUserId, AuctionProductId  auctionProductId)
        {

            var updates = new List<Action>()
                {
                    () => { if (name != null)  auction.AuctionName = name; },
                    () => { if (image != null) auction.AuctionImage = image; },
                    () => { if (priceBase != null) auction.AuctionPriceBase = priceBase; },
                    () => { if (priceReserva != null) auction.AuctionPriceReserva = priceReserva; },
                    () => { if (description != null) auction.AuctionDescription = description; },

                    () => { if (incremento != null) auction.AuctionIncremento = incremento; },
                     () => { if (auctionCantidadProducto != null) auction.AuctionCantidadProducto = auctionCantidadProducto; }, 

                    () => { if (auctionFechaInicio != null) auction.AuctionFechaInicio = auctionFechaInicio; },
                    () => { if (auctionFechaFin != null) auction.AuctionFechaFin = auctionFechaFin; },


                    () => { if (condiciones != null) auction.AuctionCondiciones = condiciones; },
                  

                    () => { if (auction.AuctionUserId != null) auction.AuctionUserId = auctionUserId; },

                    () => { if (auction.AuctionProductId != null) auction.AuctionProductId = auctionProductId; }
                };


            updates.ForEach(update => update());
            return auction;
        }

        // --- Método para cambiar el estado de la subasta (será llamado por el consumidor de la Saga) ---
        public void SetState(AuctionState newState)
        {
            // Puedes añadir lógica de validación para transiciones de estado válidas aquí
            // Por ejemplo:
            // if (CurrentState == AuctionState.Completed && newState != AuctionState.Completed)
            // {
            //     throw new InvalidOperationException($"Cannot change state from {CurrentState} once completed.");
            // }

            CurrentState = newState;
        }

    }
}
