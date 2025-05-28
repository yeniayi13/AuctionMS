using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;
using Xunit;

namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{

    public class GetAllAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllAuctionQueryHandler _handler;

        public GetAllAuctionQueryHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllAuctionQueryHandler(
                _auctionRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAuctionList_WhenAuctionExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetAllAuctionQuery(userId, productId);

            var auctionList = new List<AuctionEntity> { new AuctionEntity(), new AuctionEntity() };
            var auctionDtoList = new List<GetAuctionDto> { new GetAuctionDto(), new GetAuctionDto() };

            _auctionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()))
                .ReturnsAsync(auctionList);

            _mapperMock.Setup(mapper => mapper.Map<List<GetAuctionDto>>(auctionList))
                .Returns(auctionDtoList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionDtoList.Count, result.Count);
            _auctionRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<GetAuctionDto>>(auctionList), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAuctionAreEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var request = new GetAllAuctionQuery(userId, productId);

            _auctionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()))
                .ReturnsAsync((List<AuctionEntity>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AuctionNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Auctions are empty", exception.Message);
        }

        //[Fact]

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMapperIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                Task.Run(() => new GetAllAuctionQueryHandler(_auctionRepositoryMock.Object, null)));

            Assert.Contains("mapper", exception.Message);
        }
    }
}
