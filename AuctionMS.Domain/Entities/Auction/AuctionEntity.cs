
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Primitives;


namespace AuctionMS.Domain.Entities.Auction
{
    public sealed class AuctionEntity : AggregateRoot
    {
        public AuctionId AuctionId { get; private set; }
        public AuctionName AuctiontName { get; private set; }
        public AuctionImage AuctionImage { get; private set; }
        public AuctionPriceBase AuctionPriceBase { get; private set; }
        public AuctionPriceReserva AuctionPriceReserva { get; private set; }
        public AuctionDescription AuctionDescription { get; private set; }
        public AuctionIncremento AuctionIncremento { get; private set; }

        public AuctionCondiciones AuctionCondiciones { get; private set; }
        public AuctionDuracion AuctionDuracion{ get; private set; }

        public AuctionUserId AuctionUserId { get; private set; } //FK


        public AuctionEntity (AuctionId auctionId, AuctionName auctionName, AuctionImage auctionImage, AuctionPriceBase auctionPriceBase,
            AuctionPriceReserva auctionPriceReserva, AuctionDescription auctionDescription, AuctionIncremento auctionIncremento,
            AuctionCondiciones auctionCondiciones, AuctionDuracion auctionDuracion, AuctionUserId auctionUserId)
        {
            AuctionId = auctionId;
            AuctionName = auctionName;
            AuctionImage = auctionImage;
            AuctionPriceBase = auctionPriceBase;
            AuctionPriceReserva = auctionPriceReserva;
            AuctionDescription = auctionDescription;
            AuctionIncremento = auctionIncremento;
            AuctionCondiciones = auctionCondiciones;
            AuctionDuracion = auctionDuracion;
            AuctionUserId = auctionUserId; 

        }

        public AuctionEntity() { }


        //actualiza las propiedades de un objeto
        public static AuctionEntity Update(AuctionEntity product, AuctionName name, AuctionImage image, 
            AuctionPriceBase priceBase, AuctionPriceReserva priceReserva, AuctionDescription description, 
            AuctionIncremento incremento, AuctionCondiciones condiciones, AuctionDuracion duracion, AuctionUserId auctionUserId)
        {

            var updates = new List<Action>()
                {
                    () => { if (name != null)  auction.AuctionName = name; },
                    () => { if (image != null) auction.AuctionImage = image; },
                    () => { if (priceBase != null) auction.AuctionPriceBase = priceBase; },
                    () => { if (priceReserva != null) auction.AuctionPriceReserva = priceReserva; },
                    () => { if (description != null) auction.AuctionDescription = description; },

                    () => { if (incremento != null) auction.AuctionIncremento = incremento; },
                    () => { if (condiciones != null) auction.AuctionCondiciones = condiciones; },
                    () => { if (duracion != null) auction.AuctionDuracion = duracion; },

                    () => { if (auction.AuctionUserId != null) auction.AuctionUserId = auctionUserId; }
                };

            updates.ForEach(update => update());
            return auction;
        }

    }
}
