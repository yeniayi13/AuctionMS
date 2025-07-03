using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Request
{
    public class UpdateEstadoAuctionDto
    {
        public Guid CorrelationId { get; set; }
        public string NuevoEstado { get; set; } = null!;
        public DateTime FechaActualizacion { get; set; }
    

   
        public UpdateEstadoAuctionDto(Guid id, string nuevoEstado, DateTime fechaActualizacion)
        {
            CorrelationId = id;
            NuevoEstado = nuevoEstado;
            FechaActualizacion = fechaActualizacion;

        }
    }

}