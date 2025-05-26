using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionCantidadProducto
    {
        private AuctionCantidadProducto(int cantidad, int stockRestante)
        {
            Cantidad = cantidad;
            StockRestante = stockRestante;
        }

        public static AuctionCantidadProducto Create(int cantidad, int stockActual)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad a subastar debe ser mayor a cero.");

            if (cantidad > stockActual)
                throw new ArgumentException("La cantidad a subastar no puede ser mayor al stock disponible.");

            int stockRestante = stockActual - cantidad;

            return new AuctionCantidadProducto(cantidad, stockRestante);
        }



        public int Cantidad { get; init; }
        public int StockRestante { get; init; }

        // ✅ Agrega esta propiedad para indexar
        public int Value => Cantidad;
    }
}

