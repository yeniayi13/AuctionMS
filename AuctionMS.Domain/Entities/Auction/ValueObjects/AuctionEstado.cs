using System;
using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public partial class AuctionEstado
    {
        private static readonly string[] EstadosValidos = { "Pending", "Active", "Ended", "Canceled", "Completed" };

        private AuctionEstado(string value) => Value = value;

        public static AuctionEstado Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El estado de la subasta es obligatorio.", nameof(value));

            if (!IsValid(value))
                throw new ArgumentException($"Estado inválido: {value}. Estados válidos: {string.Join(", ", EstadosValidos)}");

            return new AuctionEstado(value);
        }

        public string Value { get; init; }

        public static bool IsValid(string value)
        {
            return Array.Exists(EstadosValidos, estado => estado.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        // Métodos de acceso rápidos (opcional)
        public static AuctionEstado Pending() => new("Pending");
        public static AuctionEstado Active() => new("Active");
        public static AuctionEstado Ended() => new("Ended");
        public static AuctionEstado Canceled() => new("Canceled");
        public static AuctionEstado Completed() => new("Completed");

        public override string ToString() => Value;
    }
}
