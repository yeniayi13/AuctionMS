using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionCondiciones
    {
        private AuctionCondiciones(string value) => Value = value;

        public static AuctionCondiciones Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Las condiciones de la subasta son requeridas.");

            return new AuctionCondiciones(value.Trim());
        }

        public string Value { get; init; } // Solo asignable desde el constructor
    }
}

