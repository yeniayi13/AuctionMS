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
using AuctionMS.Core.Service.Auction;
using AuctionMS.Core.Service.User;
using MassTransit;
using AuctionMS.Application.Saga.Events;




namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IEventBus<GetAuctionDto> _eventBus;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;






        public CreateAuctionCommandHandler(IMapper mapper, IUserService userService, IAuctionRepository auctionRepository, IEventBus<GetAuctionDto> eventBus, IProductService productService, IPublishEndpoint publishEndpoint)

        {
            _auctionRepository = auctionRepository;
            _eventBus = eventBus;
            _userService = userService;
            _publishEndpoint = publishEndpoint;

            _mapper = mapper;
            _productService = productService;

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

                var product = await _productService.ProductExist(request.ProductId, request.UserId);

                var stockDisponible = await _productService.GetProductStock(request.ProductId, request.UserId);
                if (stockDisponible == null)
                    throw new InvalidOperationException($"No se pudo obtener el stock del producto con ID: {request.ProductId}");

                if (stockDisponible < request.Auction.AuctionCantidadProducto)
                    throw new InvalidOperationException($"Stock insuficiente para el producto con ID: {request.ProductId}. " +
                        $"Stock disponible: {stockDisponible}, cantidad solicitada: {request.Auction.AuctionCantidadProducto}");



                if (user == null) throw new NullReferenceException($"user with id {request.UserId} not found");


                // Crear la entidad Subasta
                var auction = new AuctionEntity(
                    AuctionId.Create(Guid.NewGuid()),
                    AuctionName.Create(request.Auction.AuctionName),
                    AuctionImage.Create(request.Auction.AuctionImage),
                    AuctionPriceBase.Create(request.Auction.AuctionPriceBase),
                    AuctionPriceReserva.Create(request.Auction.AuctionPriceReserva),
                    AuctionDescription.Create(request.Auction.AuctionDescription),
                    AuctionIncremento.Create(request.Auction.AuctionIncremento),
                    AuctionCantidadProducto.Create(request.Auction.AuctionCantidadProducto),  
                    AuctionEstado.Create(request.Auction.AuctionEstado),
                    AuctionFechaInicio.Create(request.Auction.AuctionFechaInicio),
                    AuctionFechaFin.Create(request.Auction.AuctionFechaFin),
                    AuctionCondiciones.Create(request.Auction.AuctionCondiciones),
                    AuctionUserId.Create(request.Auction.AuctionUserId), // Asignar el ID del usuario
                    AuctionProductId.Create(request.Auction.AuctionProductId),
                     AuctionBidId.Create(request.Auction.AuctionBidId),
                     AuctionPaymentId.Create(request.Auction.AuctionPaymentId)

                );

                var auctionDto = _mapper.Map<GetAuctionDto>(auction);
                var stock = await _productService.GetProductStock(request.Auction.AuctionProductId, request.UserId);

                //Cambia esto y maneja un aexcepción específica si para stock insuficiente
                if (stock < auctionDto.AuctionCantidadProducto)
                {
                    throw new InvalidOperationException($" Error: Stock insuficiente para el producto {request.Auction.AuctionProductId}. Disponible: {stock}, requerido: {auctionDto.AuctionCantidadProducto}");
                }
                // Guardar la subasta en el repositorio
               

                var nuevoStock = stock.Value - auctionDto.AuctionCantidadProducto;

              var actualizado = await _productService.UpdateProductStockAsync(request.Auction.AuctionProductId, nuevoStock,request.UserId);


                if (!actualizado)
                {
                    throw new ApplicationException("No se pudo actualizar el stock del producto luego de crear la subasta.");
                }
                await _auctionRepository.AddAsync(auction);
                await _eventBus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_CREATED");

                await _publishEndpoint.Publish(new AuctionStartedEvent
                (
                   auction.AuctionId.Value,
                   auction.AuctionFechaInicio.Value
                ));



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