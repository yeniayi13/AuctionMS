using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Core.RabbitMQ;




namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IEventBus<CreateAuctionDto> _eventBus;


        public CreateAuctionCommandHandler(IAuctionRepository auctionRepository, IEventBus<CreateAuctionDto> eventBus)
        {
            _auctionRepository = auctionRepository;
            _eventBus = eventBus;

        }

        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo AuctionId
               // var auctionId = AuctionId.Create(Guid.NewGuid());



                // Crear la entidad Subasta
                var auction = new AuctionEntity(
                    AuctionId.Create(request.Auction.AuctionId),
                    AuctionName.Create(request.Auction.AuctionName),
                    AuctionImage.Create(request.Auction.AuctionImage),
                    AuctionPriceBase.Create(request.Auction.AuctionPriceBase),
                    AuctionPriceReserva.Create(request.Auction.AuctionPriceReserva),
                    AuctionDescription.Create(request.Auction.AuctionDescription),
                    AuctionIncremento.Create(request.Auction.AuctionIncremento),
                    AuctionDuracion.Create(request.Auction.AuctionDuracion),
                    AuctionCondiciones.Create(request.Auction.AuctionCondiciones),
                    AuctionUserId.Create(request.Auction.AuctionUserId), // Asignar el ID del usuario
                     AuctionProductId.Create(request.Auction.AuctionProductId)



                );

                // Guardar la subasta en el repositorio
                await _auctionRepository.AddAsync(auction);

                // Retornar el ID de la subasta registrada
                return auction.AuctionId.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                Console.WriteLine($"An error occurred while registering the auction, {ex.Message}");
                throw;

            }
        }
    }
}