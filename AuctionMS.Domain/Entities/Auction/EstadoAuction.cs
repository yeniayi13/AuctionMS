using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace AuctionMS.Domain.Entities.Auction
{

    public class EstadoAuction : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        // Este es el estado actual de la saga 
        public string EstadoActual { get; set; } = null!;

        // Fecha de última actualización del estado
        public DateTime UltimaActualizacion { get; set; }
    }
}
