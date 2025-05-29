
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Application.Auctions.Queries;
using AuctionMS.Infrastructure.Repositories;

namespace AuctionMS.Controllers
{
    [ApiController]
    [Route("auctioneer/auction")]
    public class AuctionController : ControllerBase
    {
        private readonly ILogger<AuctionController> _logger;
        private readonly IMediator _mediator;

        public AuctionController(ILogger<AuctionController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("addAuction/{userId}/{productId}")]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto, [FromRoute] Guid userId, [FromRoute] Guid productId)
        {
            try
            {
                
                var command = new CreateAuctionCommand(createAuctionDto, userId, productId );
                var auctionId = await _mediator.Send(command);

                return Ok(auctionId);
            
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (ValidatorException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to create an Auction");
            }
        }


        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllAuction([FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            try
            {
                var query = new GetAllAuctionQuery(userId, productId);
                var Auction = await _mediator.Send(query);
                
                if (Auction == null || !Auction.Any()) {
                    return NotFound("No Auctions found");
                }

                return Ok(Auction);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search Auction: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search Auction");
            }
        }



        [HttpGet("name/auction/{name}")]
        public async Task<IActionResult> GetAllNameAuction([FromRoute] string name, [FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("El nombre de la subasta es requerido.");
                }

                var query = new GetNameAuctionQuery(name, userId, productId);
                var auction = await _mediator.Send(query);

                return Ok(auction);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to search Auction: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to search Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to search Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An unexpected error occurred: {Message}", e.Message);
                return StatusCode(500, "An unexpected error occurred while trying to search the auction.");
            }
        }

        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuction([FromRoute] Guid id, [FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            try
            {
                var command = new GetAuctionQuery(id, userId, productId);
                var Auction = await _mediator.Send(command);
                return Ok(Auction);
            }
            
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search an Auction: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search an Auction");
            }
        }



        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto updateAuctionDto, [FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            try
            
            {
                var command = new UpdateAuctionCommand(id, updateAuctionDto, userId, productId);
                var AuctionId = await _mediator.Send(command);
                return Ok(AuctionId);
            }
            
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to update an Auction: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to update an Auction");
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id, [FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            try
            {
                var command = new DeleteAuctionCommand(id, userId, productId);
                var AuctionId = await _mediator.Send(command);
                return Ok(AuctionId);
            }
           
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Auction: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                //TODO: Colocar validaciones HTTP
                _logger.LogError("An error occurred while trying to delete an Auction: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to delete an Auction");
            }
        }

        [HttpGet("producto-activo/{productId}")] //Trae los productos activos de una subasta
        public async Task<IActionResult> GetProductoActivo(
     [FromRoute] Guid productId,
     [FromQuery] Guid userId)
        {
            try
            {
                if (productId == Guid.Empty)
                    return BadRequest("El ID del producto es requerido.");

                var query = new GetProductAuctionQuery(productId, userId);
                var auctionDto = await _mediator.Send(query);

                if (auctionDto == null)
                    return NotFound("No hay una subasta activa para este producto.");

                return Ok(auctionDto);
            }
            catch (Exception e)
            {
                _logger.LogError("Error al obtener subasta activa del producto: {Message}", e.Message);
                return StatusCode(500, "Error inesperado.");
            }
        }







    }
}