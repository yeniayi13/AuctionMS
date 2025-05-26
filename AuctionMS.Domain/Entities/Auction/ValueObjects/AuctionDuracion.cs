using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System;
using System.Text.RegularExpressions;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionDuracion
    {
        private AuctionDuracion(DateTime fechaInicio, DateTime fechaFin)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            DuracionEnHoras = (decimal)(fechaFin - fechaInicio).TotalHours;
        }

        public static AuctionDuracion Create(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaFin <= fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

            return new AuctionDuracion(fechaInicio, fechaFin);
        }

        public DateTime FechaInicio { get; init; }
        public DateTime FechaFin { get; init; }
        public decimal DuracionEnHoras { get; init; }

        // ✅ Esta propiedad permite usar `.Value` para indexar
        public DateTime Value => FechaInicio;
    }

}



