using AutoMapper;
using FluentValidation;
using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Validators.Auctions;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Service.User;
//using AuctionMS.Core.Service.Product;



namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IEventBus<GetAuctionDto> _eventBus;
        private readonly IUserService _userService;
       // private readonly IProductService productService;
        private readonly IMapper _mapper;


        public CreateAuctionCommandHandler(IMapper mapper, IUserService userService, IAuctionRepository auctionRepository, IEventBus<GetAuctionDto> eventBus)
        {
            _auctionRepository = auctionRepository;
            _eventBus = eventBus;
            _userService = userService;
            _mapper = mapper;
           // _productService = productService;

        }

        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Valido los datos de entrada
                var validator = new CreateAuctionEntityValidator();
                var validationResult = await validator.ValidateAsync(request.Auction, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors); // No lo capturamos en un Exception genérico
                }

                var user = await _userService.AuctioneerExists(request.UserId);


                if (user == null) throw new NullReferenceException($"user with id {request.UserId} not found");


                // Crear la entidad Subasta
                var auction = new AuctionEntity(
                    AuctionId.Create(request.Auction.AuctionId),
                    AuctionName.Create(request.Auction.AuctionName),
                    AuctionImage.Create(request.Auction.AuctionImage),
                    AuctionPriceBase.Create(request.Auction.AuctionPriceBase),
                    AuctionPriceReserva.Create(request.Auction.AuctionPriceReserva),
                    AuctionDescription.Create(request.Auction.AuctionDescription),
                    AuctionIncremento.Create(request.Auction.AuctionIncremento),
                    AuctionCantidadProducto.Create(request.Auction.AuctionCantidadProducto),            
                    AuctionFechaInicio.Create(request.Auction.AuctionFechaInicio),
                    AuctionFechaFin.Create(request.Auction.AuctionFechaFin),
                    AuctionCondiciones.Create(request.Auction.AuctionCondiciones),
                    AuctionUserId.Create(request.Auction.AuctionUserId), // Asignar el ID del usuario
                    AuctionProductId.Create(request.Auction.AuctionProductId)



                );

                var auctionDto = _mapper.Map<GetAuctionDto>(auction);

                // Guardar la subasta en el repositorio
                await _auctionRepository.AddAsync(auction);
                await _eventBus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_CREATED");

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