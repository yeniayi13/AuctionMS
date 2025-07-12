using System;
using MediatR;

namespace AuctionMS.Application.Auction.Commands
{
    public class UpdateEstadoAuctionCommand : IRequest<Unit>
    {
        public Guid AuctionId { get; }
        public string NuevoEstado { get; }

        public UpdateEstadoAuctionCommand(Guid auctionId, string nuevoEstado)
        {
            AuctionId = auctionId;
            NuevoEstado = nuevoEstado ?? throw new ArgumentNullException(nameof(nuevoEstado));
        }
    }
}
