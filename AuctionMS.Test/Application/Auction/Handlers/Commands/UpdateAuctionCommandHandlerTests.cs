using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;

namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{
    public class UpdateAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMongoMock;
        private readonly Mock<IEventBus<UpdateAuctionDto>> _eventBusMock;
        private readonly UpdateAuctionCommandHandler _handler;

        public UpdateAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _eventBusMock = new Mock<IEventBus<UpdateAuctionDto>>();
            _auctionRepositoryMongoMock = new Mock<IAuctionRepositoryMongo>();

            _handler = new UpdateAuctionCommandHandler(
                _auctionRepositoryMongoMock.Object,
                _auctionRepositoryMock.Object,
                _eventBusMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAuctionAndPublishEvent()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var updateDto = new UpdateAuctionDto
            {
                AuctionName = "Updated Auction",
                AuctionImage = "updatedImageBase64",
                AuctionPriceBase = 200,
                AuctionPriceReserva = 300,
                AuctionDescription = "Updated Description",
                AuctionIncremento = 10,
                AuctionCantidadProducto = 5,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(7),
                AuctionCondiciones = "Updated conditions",
                AuctionUserId = userId,
                AuctionProductId = Guid.NewGuid()
            };

            var updateCommand = new UpdateAuctionCommand(auctionId, updateDto, userId);

            var oldAuction = new AuctionEntity(
                AuctionId.Create(auctionId),
                AuctionName.Create("Old Auction"),
                AuctionImage.Create("oldImageBase64"),
                AuctionPriceBase.Create(150),
                AuctionPriceReserva.Create(250),
                AuctionDescription.Create("Old Description"),
                AuctionIncremento.Create(5),
                AuctionCantidadProducto.Create(3),
                AuctionEstado.Create("Pending"),
                AuctionFechaInicio.Create(DateTime.UtcNow.AddDays(-10)),
                AuctionFechaFin.Create(DateTime.UtcNow.AddDays(-3)),
                AuctionCondiciones.Create("Old conditions"),
                AuctionUserId.Create(userId),
                AuctionProductId.Create(Guid.NewGuid()),
                 AuctionBidId.Create(Guid.NewGuid()),
                 AuctionPaymentId.Create(Guid.NewGuid())
            );

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync(oldAuction);

            _auctionRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<AuctionEntity>()))
                .ReturnsAsync((AuctionEntity updatedAuction) => updatedAuction);

            _eventBusMock
                .Setup(bus => bus.PublishMessageAsync(It.IsAny<UpdateAuctionDto>(), "auctionQueue", "AUCTION_UPDATED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _auctionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AuctionEntity>()), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<UpdateAuctionDto>(), "auctionQueue", "AUCTION_UPDATED"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAuctionNotFound()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var validUpdateDto = new UpdateAuctionDto
            {
                AuctionName = "Valid Auction",
                AuctionPriceBase = 100m,
                AuctionPriceReserva = 150m,
                AuctionIncremento = 10m,
                AuctionCantidadProducto = 1,
                AuctionFechaInicio = DateTime.UtcNow,
                AuctionFechaFin = DateTime.UtcNow.AddDays(1),
                AuctionCondiciones = "Valid conditions",
                AuctionUserId = userId,
                AuctionProductId = Guid.NewGuid()
            };

            var updateCommand = new UpdateAuctionCommand(auctionId, validUpdateDto, userId);

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AuctionNotFoundException>(() => _handler.Handle(updateCommand, CancellationToken.None));
            Assert.Equal("Auction not found", exception.Message);

            _auctionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AuctionEntity>()), Times.Never);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<UpdateAuctionDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Contains("Request cannot be null.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAuctionDtoIsNull()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var updateCommand = new UpdateAuctionCommand(auctionId, null, userId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(updateCommand, CancellationToken.None));
            Assert.Contains("Auction cannot be null.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenAuctionDtoIsInvalid()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // DTO con valores inválidos para pasar la validación (ejemplo, nombre vacío)
            var invalidUpdateDto = new UpdateAuctionDto
            {
                AuctionName = "", // nombre inválido que debe ser validado
                AuctionUserId = userId
            };

            var updateCommand = new UpdateAuctionCommand(auctionId, invalidUpdateDto, userId);

            var oldAuction = new AuctionEntity(); // se puede devolver un objeto válido aunque vacío para la prueba

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync(oldAuction);

            // Act & Assert
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _handler.Handle(updateCommand, CancellationToken.None));
        }
    }
}
