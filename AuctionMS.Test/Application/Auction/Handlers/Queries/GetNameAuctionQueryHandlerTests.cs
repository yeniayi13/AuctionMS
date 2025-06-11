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
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using Xunit;

namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{
    public class GetNameAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetNameAuctionQueryHandler _handler;

        public GetNameAuctionQueryHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepositoryMongo>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetNameAuctionQueryHandler(
                _auctionRepositoryMock.Object,
                _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAuctionDto_WhenAuctionExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetNameAuctionQuery("Existing Auction", userId);

            var auctionEntity = new AuctionEntity();
            var auctionDto = new GetAuctionDto();

            _auctionRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync(auctionEntity);

            _mapperMock.Setup(mapper => mapper.Map<GetAuctionDto>(auctionEntity))
                .Returns(auctionDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionDto, result);
            _auctionRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<GetAuctionDto>(auctionEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetNameAuctionQuery("Nonexistent Auction", userId);

            // Configuramos el mock para que devuelva null simulando que no encontró la subasta
            _auctionRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AuctionNotFoundException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal("Auction not found.", exception.Message);

            _auctionRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<AuctionName>(), It.IsAny<AuctionUserId>()), Times.Once);
        }

    }
}
