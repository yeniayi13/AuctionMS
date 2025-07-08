
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Application.Auctions.Queries;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Controllers
{
    [ApiController]
    [Route("auction")]
    public class AuctionController : ControllerBase
    {
        private readonly ILogger<AuctionController> _logger;
        private readonly IMediator _mediator;

        public AuctionController(ILogger<AuctionController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Authorize(Policy = "SubastadorPolicy")]

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

        //buscar 1 subasta por su estado

        [Authorize(Policy = "SubastadorPolicy")]
        [Authorize(Policy = "PostorPolicy")]

        [HttpGet("{auctionId}/state")]
        public async Task<IActionResult> GetAuctionState(Guid auctionId)
        {
            var estado = await _mediator.Send(new GetAuctionStateQuery(auctionId));
            if (estado == null)
                return NotFound($"Estado para subasta {auctionId} no encontrado");

            return Ok(new { AuctionId = auctionId, Estado = estado });
        }

        //Buscar todas las subastas por estado
        [Authorize(Policy = "SubastadorPolicy")]
        [Authorize(Policy = "PostorPolicy")]

        [HttpGet("state/{estado}")]
        public async Task<IActionResult> GetAuctionsByState(string estado)
        {
            var subastas = await _mediator.Send(new GetAuctionsByStateQuery(estado));
            if (subastas == null )
                return NotFound($"No se encontraron subastas con el estado '{estado}'");

            return Ok(subastas);
        }



        
          [Authorize(Policy = "PostorPolicy")]
          [Authorize(Policy = "SubastadorPolicy")]
          [Authorize(Policy = "AdministradorPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllAuction([FromQuery] Guid userId)
        {
            try
            {
                var query = new GetAllAuctionQuery(userId);
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


       
        [Authorize(Policy = "SubastadorPolicy")]
        [Authorize(Policy = "PostorPolicy")]
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

        [Authorize(Policy = "SubastadorPolicy")]
        [Authorize(Policy = "PostorPolicy")]

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAuction( [FromQuery] Guid userId, [FromRoute] Guid productId)
        {
            try
            {
                var command = new GetAuctionQuery( userId, productId);
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


        [Authorize(Policy = "SubastadorPolicy")]
       
        //Cancelar subasta

        [HttpPost("{auctionId:guid}/cancel")]
        public async Task<IActionResult> CancelAuction(Guid auctionId, [FromQuery] Guid userId)
        {
            try
            {
                var command = new CancelAuctionCommand(auctionId, userId);
                await _mediator.Send(command);
                return Ok(new { message = $"Subasta {auctionId} cancelada correctamente." });
            }
           
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al cancelar la subasta: {ex.Message}" });
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        [Authorize(Policy = "PostorPolicy")]
        //BUSCAR SUBASTA SOLO POR ID

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("El ID de la subasta es requerido.");
                }

                var query = new GetAuctionByIdQuery(id); 
                var Auction = await _mediator.Send(query);

                if (Auction == null)
                {
                    return NotFound("No se encontró la subasta.");
                }

                return Ok(Auction);
            }
            catch (Exception e)
            {
                _logger.LogError("Error al buscar la subasta por ID: {Message}", e.Message);
                return StatusCode(500, "Ocurrió un error al buscar la subasta.");
            }
        }


         [Authorize(Policy = "SubastadorPolicy")]
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

      


     

        [Authorize(Policy = "SubastadorPolicy")]
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

       [Authorize(Policy = "SubastadorPolicy")]
       [Authorize(Policy = "PostorPolicy")]
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
                    return Ok(null); // Esto resulta en 200 OK y un body nulo


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