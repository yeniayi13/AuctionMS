


using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Application.Saga.Events
{

    public class AuctionStartedEvent
    {
        public Guid AuctionId { get; set; }
        public DateTime AuctionFechaInicio { get; set; }

        public AuctionStartedEvent(Guid auctionId, DateTime fechaInicio)
        {
            AuctionId = auctionId;
            AuctionFechaInicio = fechaInicio;
        }
        public AuctionStartedEvent() { }
    }

    public record ActivateAuctionEvent
    {
        public Guid AuctionId { get; set; }
        public DateTime FechaActivacion { get; set; }
    }

    public class AuctionEndedEvent
    {
        public Guid AuctionId { get; set; }
        public DateTime FechaFin {  get; set; }

        public AuctionEndedEvent(Guid auctionId, DateTime fechaFin)
        {
            AuctionId = auctionId;
            FechaFin = fechaFin;
        }
    }

    public class AuctionCanceledEvent
    {
        public Guid AuctionId { get; set; }

        public AuctionCanceledEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class PaymentReceivedEvent
    {
        public Guid AuctionId { get; set; }
        public DateTime FechaPayment { get; set; }

        public PaymentReceivedEvent(Guid auctionId, DateTime fechaPayment)
        {
            AuctionId = auctionId;
            FechaPayment = fechaPayment;
        }
    }


    public class BidPlacedEvent
    {
        public Guid AuctionId { get; set; }
        public Guid BidUserId { get; set; }
        public DateTime FechaBid { get; set; }


        public BidPlacedEvent(Guid auctionId , Guid bidUserId, DateTime fechaBid)
        {
            AuctionId = auctionId;
            BidUserId = bidUserId;
            FechaBid = fechaBid;
        }
    }

    public class AuctionStateChangedEvent
    {
        public Guid AuctionId { get;  set; }
        public string NuevoEstado { get; set; }

        public DateTime FechaActualizacion { get; set; }
    

     public AuctionStateChangedEvent(Guid auctionId, string nuevoEstado, DateTime fechaActualizacion)
        {
            AuctionId = auctionId;
            NuevoEstado = nuevoEstado;
            FechaActualizacion = fechaActualizacion;
        }

    }

}
    