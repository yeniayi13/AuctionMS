using System;
using System.Collections.Generic;
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
using AuctionMS.Infrastructure.Exceptions;

namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{
    public class GetAllAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMongoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllAuctionQueryHandler _handler;

        public GetAllAuctionQueryHandlerTests()
        {
            _auctionRepositoryMongoMock = new Mock<IAuctionRepositoryMongo>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllAuctionQueryHandler(_auctionRepositoryMongoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedAuctionDtoList_WhenAuctionsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var auctions = new List<AuctionEntity>
            {
                new AuctionEntity(
                    AuctionId.Create(Guid.NewGuid()),
                    AuctionName.Create("Auction 1"),
                    AuctionImage.Create("image1"),
                    AuctionPriceBase.Create(100),
                    AuctionPriceReserva.Create(150),
                    AuctionDescription.Create("Desc 1"),
                    AuctionIncremento.Create(10),
                    AuctionCantidadProducto.Create(2),
                    AuctionFechaInicio.Create(DateTime.UtcNow),
                    AuctionFechaFin.Create(DateTime.UtcNow.AddDays(5)),
                    AuctionCondiciones.Create("Cond 1"),
                    AuctionUserId.Create(userId),
                    AuctionProductId.Create(Guid.NewGuid()),
                    AuctionBidId.Create(Guid.NewGuid())
                ),
                new AuctionEntity(
                    AuctionId.Create(Guid.NewGuid()),
                    AuctionName.Create("Auction 2"),
                    AuctionImage.Create("image2"),
                    AuctionPriceBase.Create(200),
                    AuctionPriceReserva.Create(250),
                    AuctionDescription.Create("Desc 2"),
                    AuctionIncremento.Create(20),
                    AuctionCantidadProducto.Create(1),
                    AuctionFechaInicio.Create(DateTime.UtcNow),
                    AuctionFechaFin.Create(DateTime.UtcNow.AddDays(10)),
                    AuctionCondiciones.Create("Cond 2"),
                    AuctionUserId.Create(userId),
                    AuctionProductId.Create(Guid.NewGuid()),
                     AuctionBidId.Create(Guid.NewGuid())
                )
            };

            var auctionDtos = new List<GetAuctionDto>
            {
                new GetAuctionDto { AuctionName = "Auction 1" },
                new GetAuctionDto { AuctionName = "Auction 2" }
            };

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetAllAsync(It.Is<AuctionUserId>(id => id.Value == userId)))
                .ReturnsAsync(auctions);

            _mapperMock
                .Setup(m => m.Map<List<GetAuctionDto>>(It.IsAny<List<AuctionEntity>>()))
                .Returns(auctionDtos);

            var query = new GetAllAuctionQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Collection(result,
                item => Assert.Equal("Auction 1", item.AuctionName),
                item => Assert.Equal("Auction 2", item.AuctionName)
            );

            _auctionRepositoryMongoMock.Verify(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<GetAuctionDto>>(auctions), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionsAreNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()))
                .ReturnsAsync((List<AuctionEntity>)null);

            var query = new GetAllAuctionQuery(userId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AuctionNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Auctions are empty", exception.Message);

            _auctionRepositoryMongoMock.Verify(repo => repo.GetAllAsync(It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<GetAuctionDto>>(It.IsAny<List<AuctionEntity>>()), Times.Never);
        }
    }
}
