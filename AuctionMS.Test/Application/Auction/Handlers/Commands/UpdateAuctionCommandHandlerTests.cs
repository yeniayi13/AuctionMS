using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;
using Xunit;
using AuctionMS.Core.RabbitMQ;
using Google.Api.Gax.ResourceNames;
using Firebase.Auth;

namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{

    public class UpdateAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IEventBus<UpdateAuctionDto>> _eventBusMock;
        private readonly UpdateAuctionCommandHandler _handler;

        public UpdateAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _eventBusMock = new Mock<IEventBus<UpdateAuctionDto>>();

            _handler = new UpdateAuctionCommandHandler(
                _auctionRepositoryMock.Object,
                _eventBusMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAuctionAndPublishEvent()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auctionUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var fechaInicioSubasta = DateTime.UtcNow;
            var updateCommand = new UpdateAuctionCommand(auctionId, new UpdateAuctionDto
            {
                AuctionName = "Updated Name",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 150,
                AuctionIncremento = 20, 
                AuctionCantidadProducto = 10,
                AuctionFechaInicio = fechaInicioSubasta,
                AuctionFechaFin = fechaInicioSubasta.AddHours(4),
            }, productId, auctionUserId);

           
            var oldAuction = new AuctionEntity(
                AuctionId.Create(auctionId), 
                AuctionName.Create("Old Name"),
                AuctionImage.Create("Old Image"),
                AuctionPriceBase.Create(80),
                AuctionPriceReserva.Create(100),
                AuctionDescription.Create("Old Description"),
                AuctionIncremento.Create(10),
                AuctionCantidadProducto.Create(20),
                AuctionFechaInicio.Create(fechaInicioSubasta),
                AuctionFechaFin.Create(fechaInicioSubasta.AddHours(2)),
                AuctionCondiciones.Create("hola"),
               
                AuctionUserId.Create(auctionUserId),
                AuctionProductId.Create(productId)
            );

            _auctionRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync(oldAuction);

            _auctionRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<AuctionEntity>()))
                .ReturnsAsync((AuctionEntity updatedProduct) => updatedProduct ?? new AuctionEntity());

            _eventBusMock.Setup(bus => bus.PublishMessageAsync(It.IsAny<UpdateAuctionDto>(), "auctionQueue", "AUCTION_UPDATED"))
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
            var fechaInicioSubasta = DateTime.UtcNow;
            // Arrange
            var auctionId = Guid.NewGuid();
            var auctionUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateCommand = new UpdateAuctionCommand(auctionId, new UpdateAuctionDto

            {
                AuctionName = "Valid Name",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 150,
                AuctionIncremento = 20,
                AuctionCantidadProducto = 10,
                AuctionFechaInicio = fechaInicioSubasta,
                AuctionFechaFin = fechaInicioSubasta.AddHours(4),
            }, productId, auctionUserId);

            _auctionRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
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

        
    }
}
