using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionFechaInicio
    {
        public DateTime Value { get; }

        private AuctionFechaInicio(DateTime value)
        {
            if (value == default)
                throw new ArgumentException("La fecha de inicio no puede estar vacía.");
            Value = value;
        }

        public static AuctionFechaInicio Create(DateTime value) => new AuctionFechaInicio(value);
    }
}
