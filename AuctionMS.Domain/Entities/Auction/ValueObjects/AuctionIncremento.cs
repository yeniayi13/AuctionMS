using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionIncremento
    {
        private AuctionIncremento(decimal value) => Value = value;

        public static AuctionIncremento Create(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("El incremento mínimo debe ser mayor a cero.");

            return new AuctionIncremento(value);
        }

        public decimal Value { get; init; } // Solo se puede setear desde el constructor
    }
}
