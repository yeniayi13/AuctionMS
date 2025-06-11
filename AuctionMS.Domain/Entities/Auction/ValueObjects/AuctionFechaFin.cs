using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionFechaFin
    {
        public DateTime Value { get; }

        private AuctionFechaFin(DateTime value)
        {
            if (value == default)
                throw new ArgumentException("La fecha de fin no puede estar vacía.");
            Value = value;
        }

        public static AuctionFechaFin Create(DateTime value) => new AuctionFechaFin(value);
    }
}
