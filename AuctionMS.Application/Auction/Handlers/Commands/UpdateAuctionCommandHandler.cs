using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class UpdateAuctionCommandHandler : IRequestHandler<UpdateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IEventBus<UpdateAuctionDto> _eventBus;
        public UpdateAuctionCommandHandler(IAuctionRepository auctionRepository, IEventBus<UpdateAuctionDto> eventBus)
        {
            _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;

        }

        public async Task<Guid> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Actualizando subasta: {request.Id} para el usuario: {request.UserId}"); // Agregar log para depuración
                var oldAuction = await _auctionRepository.GetByIdAsync(AuctionId.Create(request.Id)!, AuctionUserId.Create(request.UserId)!);


                if (oldAuction == null) throw new AuctionNotFoundException("Auction not found");

               

                //Crear el objeto actualizado con los cambios
                var updatedAuction = AuctionEntity.Update(
                    oldAuction, // Se debe proporcionar la entidad base para la actualización
                    request.Auction.AuctionName != null ? AuctionName.Create(request.Auction.AuctionName) : oldAuction.AuctionName,
                    request.Auction.AuctionImage != null ? AuctionImage.Create(request.Auction.AuctionImage) : oldAuction.AuctionImage,
                    request.Auction.AuctionPriceBase != null ? AuctionPriceBase.Create(request.Auction.AuctionPriceBase) : oldAuction.AuctionPriceBase,
                    request.Auction.AuctionPriceReserva != null ? AuctionPriceReserva.Create(request.Auction.AuctionPriceReserva) : oldAuction.AuctionPriceReserva,
                    request.Auction.AuctionDescription != null ? AuctionDescription.Create(request.Auction.AuctionDescription) : oldAuction.AuctionDescription,
                    request.Auction.AuctionIncremento != null ? AuctionIncremento.Create(request.Auction.AuctionIncremento) : oldAuction.AuctionIncremento,
                    request.Auction.AuctionDuracion != null ? AuctionDuracion.Create(request.Auction.AuctionDuracion) : oldAuction.AuctionDuracion,
                    request.Auction.AuctionCondiciones != null ? AuctionCondiciones.Create(request.Auction.AuctionCondiciones) : oldAuction.AuctionCondiciones,
                    request.Auction.AuctionUserId != null ? AuctionUserId.Create(request.Auction.AuctionUserId) : oldAuction.AuctionUserId

                );

                Console.WriteLine($"Actualizando subasta: {oldAuction.AuctionUserId}");

                // Actualizar el producto en el repositorio
                await _auctionRepository.UpdateAsync(oldAuction);
                await _eventBus.PublishMessageAsync(request.Auction, "auctionQueue", "AUCTION_UPDATED");

                return oldAuction.AuctionId.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAuctionCommandHandler: {ex.Message}");
                throw;
            }
        }
    }
}
