using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionCantidadProducto
    {
            private AuctionCantidadProducto(int cantidad)
            {
                Cantidad = cantidad;
            }

            public static AuctionCantidadProducto Create(int cantidad)
            {
                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad a subastar debe ser mayor a cero.");

                return new AuctionCantidadProducto(cantidad);
            }

            public int Cantidad { get; init; }

            // Valor expuesto para facilitar búsquedas e indexaciones
            public int Value => Cantidad;
        }
    }
