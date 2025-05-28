using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;
using Xunit;
using AuctionMS.Core.Database;
using Google.Api.Gax.ResourceNames;


namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{

    public class GetNameAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetNameAuctionQueryHandler _handler;

        public GetNameAuctionQueryHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetNameAuctionQueryHandler(
                _auctionRepositoryMock.Object,
                _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAuction_WhenAuctionExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetNameAuctionQuery("Existing Auction", userId, productId);

            var auction = new AuctionEntity();
            var auctionDto = new GetAuctionDto();

            _auctionRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync(auction);

            _mapperMock.Setup(mapper => mapper.Map<GetAuctionDto>(auction))
                .Returns(auctionDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionDto, result);
            _auctionRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<GetAuctionDto>(auction), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenAuctionDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetNameAuctionQuery("Non existent Auction", userId, productId);

            _auctionRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _auctionRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>(), It.IsAny<AuctionProductId>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMapperIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => new GetAllAuctionQueryHandler(_auctionRepositoryMock.Object, null)));
            Assert.Contains("mapper", exception.Message);
        }
    }
}
