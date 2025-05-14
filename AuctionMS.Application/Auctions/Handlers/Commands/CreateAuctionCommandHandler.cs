using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Enum;
using AuctionMS.Infrastructure.Repositories;



namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;


        public CreateAuctionCommandHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;

        }

        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo AuctionId
                var auctionId = AuctionId.Create(Guid.NewGuid());



                // Crear la entidad Subasta
                var auction = new AuctionEntity(
                    auctionId,
                    AuctionName.Create(request.Auction.Name),
                    AuctionImage.Create(request.Auction.Image),
                    AuctionPriceBase.Create(request.Auction.PriceBase),
                    AuctionPriceReserva.Create(request.Auction.PriceReserva),
                    AuctionDescription.Create(request.Auction.Description),
                    AuctionIncremento.Create(request.Auction.Incremento),
                    AuctionDuracion.Create(request.Auction.Duracion),
                    AuctionCondiciones.Create(request.Auction.Condiciones),




                );

                // Guardar la subasta en el repositorio
                await _auctionRepository.AddAsync(auction);

                // Retornar el ID de la subasta registrada
                return auction.Id.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                throw new Exception("An error occurred while registering the auction", ex);
            }
        }
    }
}