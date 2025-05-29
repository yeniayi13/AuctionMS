using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{
    public class DeleteAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMongoMock;
        private readonly Mock<IEventBus<GetAuctionDto>> _eventBusMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DeleteAuctionCommandHandler _handler;

        public DeleteAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _auctionRepositoryMongoMock = new Mock<IAuctionRepositoryMongo>();
            _eventBusMock = new Mock<IEventBus<GetAuctionDto>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new DeleteAuctionCommandHandler(
                _auctionRepositoryMongoMock.Object,
                _auctionRepositoryMock.Object,
                _eventBusMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAuctionSuccessfully()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var auctionEntity = new AuctionEntity(
                AuctionId.Create(auctionId),
                AuctionName.Create("Test Auction"),
                AuctionImage.Create("base64image"),
                AuctionPriceBase.Create(100),
                AuctionPriceReserva.Create(200),
                AuctionDescription.Create("Desc"),
                AuctionIncremento.Create(10),
                AuctionCantidadProducto.Create(1),
                AuctionFechaInicio.Create(DateTime.UtcNow),
                AuctionFechaFin.Create(DateTime.UtcNow.AddHours(1)),
                AuctionCondiciones.Create("Condiciones"),
                AuctionUserId.Create(userId),
                AuctionProductId.Create(productId)
            );

            var getAuctionDto = new GetAuctionDto { AuctionId = auctionId };

            _auctionRepositoryMongoMock.Setup(repo => repo.GetByIdAsync(
                It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync(auctionEntity);

            _mapperMock.Setup(mapper => mapper.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(getAuctionDto);

            _auctionRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<AuctionId>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(bus => bus.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_DELETED"))
                .Returns(Task.CompletedTask);

            var command = new DeleteAuctionCommand(auctionId, userId, productId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _auctionRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<AuctionId>(id => id.Value == auctionId)), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(getAuctionDto, "auctionQueue", "AUCTION_DELETED"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAuctionNotFound()
        {
            // Arrange
            var command = new DeleteAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _auctionRepositoryMongoMock.Setup(repo => repo.GetByIdAsync(
                It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Equal("request", exception.ParamName);
        }
    }
}