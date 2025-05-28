using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using AuctionMS.Controllers;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using Xunit;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Application.Auctions.Queries;

namespace AuctionMS.Test.Controllers
{
    public class AuctionControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<AuctionController>> _mockLogger;
        private readonly AuctionController _controller;

        public AuctionControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<AuctionController>>();
            _controller = new AuctionController(_mockLogger.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task CreatedAuction_ShouldReturnOk_WhenAuctionIsCreated()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid(); 
            var createAuctionDto = new CreateAuctionDto { AuctionName = "Antique Vase" };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _controller.CreateAuction(createAuctionDto, userId, productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreatedAuction_ShouldReturnBadRequest_WhenInvalidDataProvided()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var createAuctionDto = new CreateAuctionDto();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NullAttributeException("Auction data is required"));

            var result = await _controller.CreateAuction(createAuctionDto, userId, productId);

            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Auction data is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAllAuctions_ShouldReturnOk_WhenAuctionsExist()
        {
            var userId = Guid.NewGuid();
            var productId  = Guid.NewGuid();
            var expectedAuctions = new List<GetAuctionDto>
            {
                new GetAuctionDto { AuctionId = Guid.NewGuid(), AuctionName = "Painting" }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllAuctionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAuctions);

            var result = await _controller.GetAllAuction(userId, productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllAuctions_ShouldReturnNotFound_WhenNoAuctionsExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllAuctionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetAuctionDto>());

            var result = await _controller.GetAllAuction(userId, productId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("No Auctions found", notFoundResult.Value);

        }



        [Fact]
        public async Task GetAuction_ShouldReturnOk_WhenAuctionExists()
        {
            var userId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedAuction = new GetAuctionDto { AuctionId = auctionId, AuctionName = "Rare Book" };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAuctionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAuction);

            var result = await _controller.GetAuction(auctionId, userId, productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAuction_ShouldReturnNotFound_WhenAuctionDoesNotExist()
        {
            var auctionId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAuctionQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AuctionNotFoundException("Auction not found"));

            var result = await _controller.GetAuction(auctionId, userId,productId);

            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, notFoundResult.StatusCode);
            Assert.Equal("An error occurred while trying to search an Auction", notFoundResult.Value);
        }

         [Fact]
        public async Task UpdateAuction_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateAuctionDto = new UpdateAuctionDto { AuctionName = "Updated Auction" };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateAuctionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(auctionId);

            var result = await _controller.UpdateAuction(auctionId, updateAuctionDto, userId, productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
