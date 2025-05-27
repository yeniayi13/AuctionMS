

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentValidation;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AutoMapper;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Service.User;

namespace AuctionMS.Test.Application.Auction.Handlers.Commands
{

    public class CreateAuctionCommandHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IEventBus<GetAuctionDto>> _eventBusMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateAuctionCommandHandler _handler;

        public CreateAuctionCommandHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _eventBusMock = new Mock<IEventBus<GetAuctionDto>>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateAuctionCommandHandler(
                _mapperMock.Object,
                _userServiceMock.Object,
                _auctionRepositoryMock.Object,
                _eventBusMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateAuctionAndPublishEvent()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auctionUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var fechaInicioSubasta = DateTime.UtcNow;
            var auctionDto = new CreateAuctionDto
            {
                AuctionId = auctionId,
                AuctionName = "New Auction",
                AuctionImage = "c29tZSBkYXRhIGVuIGJhc2U2NA==",
                AuctionPriceBase = 150,
                AuctionPriceReserva = 350,
                AuctionDescription = "A new amazing product in promo",
                AuctionCondiciones = "Disponible",
                AuctionFechaInicio = fechaInicioSubasta,
                AuctionFechaFin = fechaInicioSubasta.AddHours(2),
                AuctionCantidadProducto = 5,
                AuctionIncremento = 20,
                AuctionProductId = productId,

                AuctionUserId = auctionUserId
            };

            var getAuctionDto = new GetAuctionDto
            {
                AuctionId = auctionId,
                AuctionName = "New Auction",
                AuctionImage = "c29tZSBkYXRhIGVuIGJhc2U2NA==",
                AuctionPriceBase = 150,
                AuctionPriceReserva = 350,
                AuctionDescription = "A new amazing product in promo",
                AuctionCondiciones = "Disponible",
                AuctionFechaInicio = fechaInicioSubasta,
                AuctionFechaFin = fechaInicioSubasta.AddHours(2),
                AuctionCantidadProducto = 5,
                AuctionIncremento = 20,
                AuctionProductId = productId,
                AuctionUserId = auctionUserId
            };
            var createCommand = new CreateAuctionCommand(auctionDto, auctionUserId, productId);


            var user = new GetUser
            {
                UserId = auctionUserId,
                UserName = "Test User"
            };


            _userServiceMock.Setup(service => service.AuctioneerExists(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            _auctionRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<AuctionEntity>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(bus =>
                    bus.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(mapper => mapper.Map<GetAuctionDto>(It.IsAny<AuctionEntity>()))
                .Returns(getAuctionDto);

            // Act
            var result = await _handler.Handle(createCommand, CancellationToken.None);

            // Assert
            Assert.Equal(auctionId, result);
            _auctionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<AuctionEntity>()), Times.Once);
            _eventBusMock.Verify(
                bus => bus.PublishMessageAsync(It.IsAny<GetAuctionDto>(), "auctionQueue", "AUCTION_CREATED"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var createCommand = new CreateAuctionCommand(new CreateAuctionDto
            {
                AuctionId = Guid.NewGuid(),
                AuctionName = "", // Nombre inválido
                AuctionPriceBase = -1, // Precio inválido
                AuctionPriceReserva = -1, // Precio inválido
                AuctionIncremento = -1, // Incremento inválido
                AuctionProductId = Guid.NewGuid(),
                AuctionUserId = Guid.NewGuid()

            }, Guid.NewGuid(), Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _handler.Handle(createCommand, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var createCommand = new CreateAuctionCommand(new CreateAuctionDto
            {
                AuctionId = Guid.NewGuid(),
                AuctionName = "Valid Auction",
                AuctionPriceBase = 100,
                AuctionPriceReserva = 100,
                AuctionIncremento = 100,
                AuctionProductId = Guid.NewGuid(),
                AuctionUserId = Guid.NewGuid(),

            }, Guid.NewGuid(), Guid.NewGuid());


            _userServiceMock.Setup(service => service.AuctioneerExists(It.IsAny<Guid>()))
                .ReturnsAsync((GetUser)null);



        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Contains("Object reference not set to an instance",
                exception.Message);
        }
    }
}
