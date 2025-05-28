using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Moq;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using Xunit;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Core.RabbitMQ;


namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{

    public class DeleteAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IEventBus<GetAuctionDto>> _eventBusMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DeleteAuctionCommandHandler _handler;

        public DeleteAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _eventBusMock = new Mock<IEventBus<GetAuctionDto>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new DeleteAuctionCommandHandler(
                _auctionRepositoryMock.Object,
                _eventBusMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAuctionAndPublishEvent()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auctionUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var deleteCommand = new DeleteAuctionCommand(auctionId, auctionUserId, productId);

            var auction = new AuctionEntity(); 

            _auctionRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync(auction);

            _auctionRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<AuctionId>()))
                .Returns(Task.CompletedTask);

            var auctionDto = new GetAuctionDto();
            _mapperMock.Setup(mapper => mapper.Map<GetAuctionDto>(auction))
                .Returns(auctionDto);

            _eventBusMock.Setup(bus => bus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_DELETED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(deleteCommand, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _auctionRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<AuctionId>()), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_DELETED"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAuctionNotFound()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auctionUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var deleteCommand = new DeleteAuctionCommand(auctionId, auctionUserId, productId);

            _auctionRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(deleteCommand, CancellationToken.None));
            Assert.Equal("Product not found.", exception.Message);

            _auctionRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<AuctionId>()), Times.Never);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<GetAuctionDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
