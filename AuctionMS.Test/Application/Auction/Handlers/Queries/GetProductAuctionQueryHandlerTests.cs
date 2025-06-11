using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{
    public class GetProductAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProductAuctionQueryHandler _handler;

        public GetProductAuctionQueryHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepositoryMongo>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetProductAuctionQueryHandler(
                _auctionRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAuctionDto_WhenAuctionExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetProductAuctionQuery(productId, userId);

            var auctionEntity = new AuctionEntity(); // Supón que este es un objeto válido
            var auctionDto = new GetAuctionDto();    // También uno simulado

            _auctionRepositoryMock
                .Setup(repo => repo.ObtenerSubastaActivaPorProductoAsync(
                    It.IsAny<AuctionProductId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync(auctionEntity);

            _mapperMock
                .Setup(m => m.Map<GetAuctionDto>(auctionEntity))
                .Returns(auctionDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionDto, result);
            _auctionRepositoryMock.Verify(repo => repo.ObtenerSubastaActivaPorProductoAsync(
                It.IsAny<AuctionProductId>(), It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<GetAuctionDto>(auctionEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenAuctionDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetProductAuctionQuery(productId, userId);

            _auctionRepositoryMock
                .Setup(repo => repo.ObtenerSubastaActivaPorProductoAsync(
                    It.IsAny<AuctionProductId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _auctionRepositoryMock.Verify(repo => repo.ObtenerSubastaActivaPorProductoAsync(
                It.IsAny<AuctionProductId>(), It.IsAny<AuctionUserId>()), Times.Once);
        }
    }
}
