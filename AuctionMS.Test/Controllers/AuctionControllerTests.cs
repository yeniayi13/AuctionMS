using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using AuctionMS.Controllers;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace AuctionMS.Tests.Controllers
{
    public class AuctionControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<AuctionController>> _mockLogger;
        private readonly AuctionController _controller;
        private readonly Mock<IMapper> _mockMapper;

       /* public AuctionControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<AuctionController>>();
            _controller = new AuctionController(_mockLogger.Object, _mockMediator.Object, _mockMapper.Object);
        }*/

        [Fact]
        public async Task CreateAuction_ShouldReturnOk_WhenAuctionCreatedSuccessfully()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedAuctionId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default)).ReturnsAsync(expectedAuctionId);

            var result = await _controller.CreateAuction(dto, userId, productId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedAuctionId, result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturn404_WhenAuctionNotFound()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ThrowsAsync(new AuctionNotFoundException("Subasta no encontrada"));

            var result = await _controller.CreateAuction(dto, userId, productId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Subasta no encontrada", result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturn400_WhenNullAttributes()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ThrowsAsync(new NullAttributeException("Atributos nulos"));

            var result = await _controller.CreateAuction(dto, userId, productId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Atributos nulos", result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturn400_WhenInvalidAttributes()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ThrowsAsync(new InvalidAttributeException("Atributos inválidos"));

            var result = await _controller.CreateAuction(dto, userId, productId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Atributos inválidos", result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturn400_WhenValidatorFails()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ThrowsAsync(new ValidatorException("Error de validación"));

            var result = await _controller.CreateAuction(dto, userId, productId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Error de validación", result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturn500_WhenUnexpectedError()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.CreateAuction(dto, userId, productId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("An error occurred while trying to create an Auction", result.Value);
        }

        [Fact]
        public async Task CreateAuction_ShouldReturnOk_WhenSuccess()
        {
            var dto = new CreateAuctionDto();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), default))
                .ReturnsAsync(auctionId);

            var result = await _controller.CreateAuction(dto, userId, productId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(auctionId, result.Value);
        }

        [Fact]
        public async Task GetProductoActivo_ShouldReturnBadRequest_WhenProductIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _controller.GetProductoActivo(Guid.Empty, userId) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del producto es requerido", result.Value.ToString());
        }

        [Fact]
        public async Task GetProductoActivo_ShouldReturnNotFound_WhenAuctionNotExists()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductAuctionQuery>(), default)).ReturnsAsync((GetAuctionDto)null);

            var result = await _controller.GetProductoActivo(productId, userId) as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("No hay una subasta activa", result.Value.ToString());
        }

        [Fact]
        public async Task GetProductoActivo_ShouldReturnOk_WhenAuctionExists()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionDto = new GetAuctionDto();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductAuctionQuery>(), default)).ReturnsAsync(auctionDto);

            var result = await _controller.GetProductoActivo(productId, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(auctionDto, result.Value);
        }

        [Fact]
        public async Task GetProductoActivo_ShouldHandleConcurrentRequests()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionDto = new GetAuctionDto();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductAuctionQuery>(), default)).ReturnsAsync(auctionDto);

            var tasks = Enumerable.Range(0, 10).Select(_ => _controller.GetProductoActivo(productId, userId)).ToArray();

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task GetProductoActivo_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var auctionDto = new GetAuctionDto();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductAuctionQuery>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(3000).Wait();
                    return auctionDto;
                });

            var result = await _controller.GetProductoActivo(productId, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(auctionDto, result.Value);
        }

        [Fact]
        public async Task GetProductoActivo_ShouldReturnInternalServerError_WhenUnhandledExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductAuctionQuery>(), default))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetProductoActivo(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Error inesperado", result.Value.ToString());
        }
    }
}
