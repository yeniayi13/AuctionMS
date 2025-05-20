
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Application.Auctions.Queries;

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

        
        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllAuction([FromQuery] Guid userId)
        {
            try
            {
                var query = new GetAllAuctionQuery(userId);
                var Auction = await _mediator.Send(query);
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
        public async Task<IActionResult> GetAllNameAuction([FromRoute] string name, [FromQuery] Guid userId)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("El nombre de la subasta es requerido.");
                }

                var query = new GetNameAuctionQuery(name, userId);
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
        public async Task<IActionResult> GetAuction([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            try
            {
                var command = new GetAuctionQuery(id, userId);
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
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto updateAuctionDto, [FromQuery] Guid userId)
        {
            try
            
            {
                var command = new UpdateAuctionCommand(id, updateAuctionDto, userId);
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
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            try
            {
                var command = new DeleteAuctionCommand(id, userId);
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


    }
}