using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AuctionMS.Core.Repository;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;

public class DeleteAuctionCommandHandlerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock = new();
    private readonly Mock<IAuctionRepositoryMongo> _auctionRepositoryMongoMock = new();
    private readonly Mock<IEventBus<GetAuctionDto>> _eventBusMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly DeleteAuctionCommandHandler _handler;


    public DeleteAuctionCommandHandlerTests()
    {
        _handler = new DeleteAuctionCommandHandler(
            _auctionRepositoryMongoMock.Object,
            _auctionRepositoryMock.Object,
            _eventBusMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldDeleteAuction_WhenAuctionExists()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteAuctionCommand(auctionId, userId);

        var auction = new AuctionEntity(
            AuctionId.Create(auctionId),
            AuctionName.Create("Test Auction"),
            AuctionImage.Create("image.png"),
            AuctionPriceBase.Create(100),
            AuctionPriceReserva.Create(150),
            AuctionDescription.Create("Test Description"),
            AuctionIncremento.Create(10),
            AuctionCantidadProducto.Create(1),
            AuctionFechaInicio.Create(DateTime.UtcNow),
            AuctionFechaFin.Create(DateTime.UtcNow.AddHours(1)),
            AuctionCondiciones.Create("Condiciones"),
            AuctionUserId.Create(userId),
            AuctionProductId.Create(Guid.NewGuid())
        );

        _auctionRepositoryMongoMock.Setup(x => x.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
            .ReturnsAsync(auction);

        _mapperMock.Setup(m => m.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
            .Returns(new GetAuctionDto());

        _auctionRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<AuctionId>()))
            .Returns(Task.CompletedTask);

        _eventBusMock.Setup(x => x.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_DELETED"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(auctionId, result);
        _auctionRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<AuctionId>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        Assert.Equal("request", ex.ParamName);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenAuctionNotFound()
    {
        // Arrange
        var command = new DeleteAuctionCommand(Guid.NewGuid(), Guid.NewGuid());

        _auctionRepositoryMongoMock.Setup(x => x.GetByIdAsync(It.IsAny<AuctionId>(), It.IsAny<AuctionUserId>()))
            .ReturnsAsync((AuctionEntity)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Product not found.", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DeleteAuctionCommandHandler(
                _auctionRepositoryMongoMock.Object,
                null,
                _eventBusMock.Object,
                _mapperMock.Object));
    }

    }

