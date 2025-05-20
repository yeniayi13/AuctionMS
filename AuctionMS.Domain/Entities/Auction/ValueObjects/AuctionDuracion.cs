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
            private AuctionDuracion(decimal value) => Value = value;

            public static AuctionDuracion Create(decimal value)
            {
                if (value <= 0)
                    throw new ArgumentException("La duración de la subasta debe ser mayor a cero.");

                return new AuctionDuracion(value);
            }

            public decimal Value { get; init; } // Representa duración en horas
        }
    }


