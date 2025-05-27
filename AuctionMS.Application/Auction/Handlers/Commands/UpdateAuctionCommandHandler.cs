using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Validators.Auctions;
using AuctionMS.Core.RabbitMQ;
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
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                if (request.Auction == null)
                {
                    throw new ArgumentNullException(nameof(request.Auction), "Auction cannot be null.");
                }


     
                var oldAuction = await _auctionRepository.GetByIdAsync(AuctionId.Create(request.Id)!, AuctionUserId.Create(request.UserId)!, AuctionProductId.Create(request.ProductId)!);

                //Valido los datos de entrada
                var validator = new UpdateAuctionEntityValidator();
                var validationResult = await validator.ValidateAsync(request.Auction, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new FluentValidation.ValidationException(validationResult.Errors);
                }
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
                    request.Auction.AuctionCantidadProducto != null ? AuctionCantidadProducto.Create(request.Auction.AuctionCantidadProducto) : oldAuction.AuctionCantidadProducto,

                    request.Auction.AuctionFechaInicio != null ? AuctionFechaInicio.Create(request.Auction.AuctionFechaInicio) : oldAuction.AuctionFechaInicio,
                     request.Auction.AuctionFechaFin != null ? AuctionFechaFin.Create(request.Auction.AuctionFechaFin) : oldAuction.AuctionFechaFin,
                    request.Auction.AuctionCondiciones != null ? AuctionCondiciones.Create(request.Auction.AuctionCondiciones) : oldAuction.AuctionCondiciones,
                    request.Auction.AuctionUserId != null ? AuctionUserId.Create(request.Auction.AuctionUserId) : oldAuction.AuctionUserId,
                     request.Auction.AuctionProductId != null ? AuctionProductId.Create(request.Auction.AuctionProductId) : oldAuction.AuctionProductId

                );

                Console.WriteLine($"Actualizando subasta: {oldAuction.AuctionUserId}");

                // Actualizar el producto en el repositorio
                await _auctionRepository.UpdateAsync(oldAuction);
                await _eventBus.PublishMessageAsync(request.Auction, "auctionQueue", "AUCTION_UPDATED");

                return oldAuction.AuctionId.Value;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
