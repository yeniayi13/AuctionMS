using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using FluentValidation;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Repository;
using AuctionMS.Core.Service.Auction;
using AuctionMS.Core.Service.User;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{
    public class CreateAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IEventBus<GetAuctionDto>> _eventBusMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateAuctionCommandHandler _handler;

        public CreateAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _eventBusMock = new Mock<IEventBus<GetAuctionDto>>();
            _userServiceMock = new Mock<IUserService>();
            _productServiceMock = new Mock<IProductService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateAuctionCommandHandler(
                _mapperMock.Object,
                _userServiceMock.Object,
                _auctionRepositoryMock.Object,
                _eventBusMock.Object,
                _productServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateAuctionAndPublishEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();

            var auctionDto = new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionName = "Test Auction",
                AuctionImage = "image",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 200,
                AuctionDescription = "Test Description",
                AuctionIncremento = 10,
                AuctionCantidadProducto = 2,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(1),
                AuctionCondiciones = "Condiciones",
                AuctionUserId = userId,
                AuctionProductId = productId
            };

            var command = new CreateAuctionCommand(auctionDto, userId, productId);

            _userServiceMock.Setup(x => x.AuctioneerExists(userId))
                .ReturnsAsync(new GetUser { UserId = userId });

            _productServiceMock.Setup(x => x.ProductExist(productId, userId))
                .ReturnsAsync(true);

            _productServiceMock.Setup(x => x.GetProductStock(productId, userId))
                .ReturnsAsync(5m); // decimal

            _productServiceMock.Setup(x => x.UpdateProductStockAsync(productId, It.IsAny<decimal>(), userId))
                .ReturnsAsync(true);

            _mapperMock.Setup(x => x.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(new GetAuctionDto
                {
                    AuctionId = auctionId,
                    AuctionCantidadProducto = 2
                });

            _auctionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AuctionEntity>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(x => x.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _auctionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AuctionEntity>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var command = new CreateAuctionCommand(new CreateAuctionDto
            {
                AuctionName = "", // inválido
                AuctionCantidadProducto = 0,
                AuctionFechaInicio = DateTime.MinValue,
                AuctionFechaFin = DateTime.MinValue
            }, Guid.NewGuid(), Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();

            var validAuctionDto = new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionUserId = userId,
                AuctionProductId = productId,
                AuctionName = "Subasta válida",
                AuctionImage = "imagen.png",
                AuctionPriceBase = 100m,
                AuctionPriceReserva = 150m,
                AuctionDescription = "Una descripción válida",
                AuctionIncremento = 10m,
                AuctionCantidadProducto = 1,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(1),
                AuctionCondiciones = "Condiciones válidas"
            };

            var command = new CreateAuctionCommand(validAuctionDto, userId, productId);

            // Mock para que no falle antes de llegar a la validación del usuario
            _productServiceMock.Setup(x => x.GetProductStock(productId, userId))
                .ReturnsAsync(10);  // Stock suficiente
            _productServiceMock.Setup(x => x.ProductExist(productId, userId))
                .ReturnsAsync(true);

            // Aquí simulamos que el usuario no existe
            _userServiceMock.Setup(x => x.AuctioneerExists(userId))
                .ReturnsAsync((GetUser)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"user with id {userId} not found", ex.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenStockIsInsufficient()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new CreateAuctionCommand(new CreateAuctionDto
            {
                AuctionUserId = userId,
                AuctionProductId = productId,
                AuctionCantidadProducto = 10,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(1),
                AuctionName = "Valid Auction",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 200,
                AuctionDescription = "Test",
                AuctionIncremento = 10,
                AuctionCondiciones = "Condiciones",
                AuctionId = Guid.NewGuid()
            }, userId, productId);

            _userServiceMock.Setup(x => x.AuctioneerExists(userId)).ReturnsAsync(new GetUser());
            _productServiceMock.Setup(x => x.ProductExist(productId, userId)).ReturnsAsync(true);
            _productServiceMock.Setup(x => x.GetProductStock(productId, userId)).ReturnsAsync(5m); // stock insuficiente

            _mapperMock.Setup(x => x.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(new GetAuctionDto { AuctionCantidadProducto = 10 });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenStockUpdateFails()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();

            var command = new CreateAuctionCommand(new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionUserId = userId,
                AuctionProductId = productId,
                AuctionCantidadProducto = 2,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(1),
                AuctionName = "Valid Auction",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 200,
                AuctionDescription = "Test",
                AuctionIncremento = 10,
                AuctionCondiciones = "Condiciones"
            }, userId, productId);

            _userServiceMock.Setup(x => x.AuctioneerExists(userId)).ReturnsAsync(new GetUser());
            _productServiceMock.Setup(x => x.ProductExist(productId, userId)).ReturnsAsync(true);
            _productServiceMock.Setup(x => x.GetProductStock(productId, userId)).ReturnsAsync(5m);

            _mapperMock.Setup(x => x.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(new GetAuctionDto { AuctionCantidadProducto = 2 });

            _productServiceMock.Setup(x => x.UpdateProductStockAsync(productId, It.IsAny<decimal>(), userId))
                .ReturnsAsync(false); // simulamos que falla

            await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
