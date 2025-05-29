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
using AuctionMS.Core.Service.User;
using AuctionMS.Core.Service.Auction;
using AuctionMS.Domain.Entities.Auction;

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
                _productServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateAuction_AndUpdateStock_WhenValidRequest()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var fechaInicio = DateTime.UtcNow;

            var auctionDto = new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionName = "Test Auction",
                AuctionImage = "image_base64",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 200,
                AuctionDescription = "Description",
                AuctionCondiciones = "Condiciones",
                AuctionFechaInicio = fechaInicio,
                AuctionFechaFin = fechaInicio.AddDays(1),
                AuctionCantidadProducto = 10,
                AuctionIncremento = 5,
                AuctionUserId = userId,
                AuctionProductId = productId
            };

            var createCommand = new CreateAuctionCommand(auctionDto, userId, productId);

            _userServiceMock.Setup(x => x.AuctioneerExists(userId))
                .ReturnsAsync(new GetUser { UserId = userId });

            _productServiceMock.Setup(x => x.ProductExist(productId, userId))
                .ReturnsAsync(true);

            _productServiceMock.Setup(x => x.GetProductStock(productId, userId))
                .ReturnsAsync(20);

            _productServiceMock.Setup(x => x.UpdateProductStockAsync(productId, 10))
                .ReturnsAsync(true);

            _mapperMock.Setup(m => m.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(new GetAuctionDto { AuctionId = auctionId, AuctionCantidadProducto = 10, AuctionProductId = productId });

            _auctionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AuctionEntity>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(x => x.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(createCommand, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _productServiceMock.Verify(x => x.UpdateProductStockAsync(productId, 10), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenStockIsInsufficient()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var dto = new CreateAuctionDto
            {
                AuctionId = Guid.NewGuid(),
                AuctionUserId = userId,
                AuctionProductId = productId,
                AuctionCantidadProducto = 10,
                AuctionPriceBase = 100,
                AuctionPriceReserva = 150,
                AuctionIncremento = 10,
                AuctionName = "Auction",
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddHours(1),
            };

            _userServiceMock.Setup(x => x.AuctioneerExists(userId))
                .ReturnsAsync(new GetUser { UserId = userId });

            _productServiceMock.Setup(x => x.ProductExist(productId, userId))
                .ReturnsAsync(true);

            _productServiceMock.Setup(x => x.GetProductStock(productId, userId))
                .ReturnsAsync(5); // Stock insuficiente

            var command = new CreateAuctionCommand(dto, userId, productId);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenStockUpdateFails()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var dto = new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionUserId = userId,
                AuctionProductId = productId,
                AuctionCantidadProducto = 5,
                AuctionPriceBase = 100,
                AuctionPriceReserva = 150,
                AuctionIncremento = 10,
                AuctionName = "Auction",
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddHours(1),
            };

            _userServiceMock.Setup(x => x.AuctioneerExists(userId))
                .ReturnsAsync(new GetUser { UserId = userId });

            _productServiceMock.Setup(x => x.ProductExist(productId, userId))
                .ReturnsAsync(true);

            _productServiceMock.Setup(x => x.GetProductStock(productId, userId))
                .ReturnsAsync(10);

            _productServiceMock.Setup(x => x.UpdateProductStockAsync(productId, 5))
                .ReturnsAsync(false); // Simula error en actualización

            _mapperMock.Setup(m => m.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(new GetAuctionDto { AuctionId = auctionId, AuctionCantidadProducto = 5, AuctionProductId = productId });

            _auctionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AuctionEntity>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(x => x.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"))
                .Returns(Task.CompletedTask);

            var command = new CreateAuctionCommand(dto, userId, productId);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}