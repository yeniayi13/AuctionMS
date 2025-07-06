using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Application.Auctions.Queries;

namespace AuctionMS.Test.Application.Auction.Handlers.Queries
{
    public class GetAuctionQueryHandlerTests
    {
        private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMongoMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAuctionQueryHandler _handler;

        public GetAuctionQueryHandlerTests()
        {
            _auctionRepositoryMongoMock = new Mock<IAuctionRepositoryMongo>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAuctionQueryHandler(
                _auctionRepositoryMongoMock.Object,
                _dbContextMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedAuctionDto_WhenAuctionExists()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var auctionEntity = new AuctionEntity(
                AuctionId.Create(auctionId),
                AuctionName.Create("Auction Test"),
                AuctionImage.Create("img"),
                AuctionPriceBase.Create(100),
                AuctionPriceReserva.Create(150),
                AuctionDescription.Create("Desc"),
                AuctionIncremento.Create(10),
                AuctionCantidadProducto.Create(1),
                AuctionEstado.Create("Active"),
                AuctionFechaInicio.Create(DateTime.UtcNow),
                AuctionFechaFin.Create(DateTime.UtcNow.AddDays(5)),
                AuctionCondiciones.Create("Conditions"),
                AuctionUserId.Create(userId),
                AuctionProductId.Create(Guid.NewGuid()),
                 AuctionBidId.Create(Guid.NewGuid())
            );

            var auctionDto = new GetAuctionDto
            {
                AuctionName = "Auction Test"
                // Otros campos si quieres
            };

            // Cambié para que acepte cualquier AuctionId y AuctionUserId para evitar mismatch en mock
            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync(auctionEntity);

            _mapperMock
                .Setup(m => m.Map<GetAuctionDto>(auctionEntity))
                .Returns(auctionDto);

            var query = new GetAuctionQuery(auctionId, userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Auction Test", result.AuctionName);

            _auctionRepositoryMongoMock.Verify(r => r.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<GetAuctionDto>(auctionEntity), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetAuctionQuery(auctionId, userId);

            _auctionRepositoryMongoMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
                .ReturnsAsync((AuctionEntity)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AuctionNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Auction not found.", ex.Message);

            _auctionRepositoryMongoMock.Verify(r => r.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()), Times.Never);
        }
    }
}
