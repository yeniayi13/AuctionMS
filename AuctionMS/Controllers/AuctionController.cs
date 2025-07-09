
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
using FluentValidation;

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
                _logger.LogError("Ocurrió un error al intentar crear una subasta: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Ocurrió un error al intentar crear una subasta: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Ocurrió un error al intentar crear una subasta: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (ValidatorException e)
            {
                _logger.LogError("Error de validación al intentar crear una subasta: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Ocurrió un error inesperado al intentar crear una subasta: {Message}", e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al intentar crear una subasta.");
            }
        }

        //buscar 1 subasta por su estado

        // [Authorize(Policy = "SubastadorPolicy")]
        // [Authorize(Policy = "PostorPolicy")]

        [HttpGet("{auctionId}/state")]
        public async Task<IActionResult> GetAuctionState(Guid auctionId)
        {
            try
            {
                var estado = await _mediator.Send(new GetAuctionStateQuery(auctionId));

                if (estado == null)
                {
                    _logger.LogWarning("No se encontró el estado para la subasta con ID {AuctionId}", auctionId);
                    return NotFound($"Estado para la subasta con ID {auctionId} no encontrado.");
                }

                return Ok(new { AuctionId = auctionId, Estado = estado });
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al consultar estado de subasta {AuctionId}: {Message}", auctionId, e.Message);
                return BadRequest(e.Message);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("Subasta no encontrada al consultar estado {AuctionId}: {Message}", auctionId, e.Message);
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al consultar estado de subasta {AuctionId}: {Message}", auctionId, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al consultar el estado de la subasta.");
            }
        }

        //Buscar todas las subastas por estado

       // [Authorize(Policy = "SubastadorOPostorPolicy")]
        [HttpGet("state/{estado}")]
        public async Task<IActionResult> GetAuctionsByState(string estado)
        {
            try
            {
                var result = await _mediator.Send(new GetAuctionsByStateQuery(estado));

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No se encontraron subastas con estado '{Estado}'.", estado);
                    return Ok(new List<GetAuctionDto>());
                }

                return Ok(result);
            }
            catch (ValidationException e)
            {
                _logger.LogError("Error de validación al buscar subastas por estado '{Estado}': {Message}", estado, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidStateException e)
            {
                _logger.LogError("Estado inválido '{Estado}' al buscar subastas: {Message}", estado, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al buscar subastas por estado '{Estado}': {Message}", estado, e.Message);
                return StatusCode(500, "Ocurrió un error al buscar las subastas por estado.");
            }
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAuction([FromQuery] Guid userId)
        {
            try
            {
                var query = new GetAllAuctionQuery(userId);
                var auctions = await _mediator.Send(query);

                if (auctions == null || !auctions.Any())
                {
                    _logger.LogWarning("No se encontraron subastas para el usuario con ID {UserId}.", userId);
                    return Ok(null);
                }

                return Ok(auctions);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("Subasta no encontrada para el usuario {UserId}: {Message}", userId, e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al consultar subastas para el usuario {UserId}: {Message}", userId, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al consultar subastas para el usuario {UserId}: {Message}", userId, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al consultar subastas para el usuario {UserId}: {Message}", userId, e.Message);
                return StatusCode(500, "Ocurrió un error al buscar las subastas.");
            }
        }


        [Authorize(Policy = "SubastadorOPostorPolicy")]
        [HttpGet("name/auction/{name}")]
        public async Task<IActionResult> GetAllNameAuction([FromRoute] string name, [FromQuery] Guid userId)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogWarning("El nombre de la subasta es requerido.");
                    return BadRequest("El nombre de la subasta es requerido.");
                }

                var query = new GetNameAuctionQuery(name, userId);
                var auction = await _mediator.Send(query);

                return Ok(auction);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("No se encontró la subasta con nombre '{Name}' para el usuario {UserId}: {Message}", name, userId, e.Message);
                return NotFound(e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al buscar la subasta por nombre '{Name}': {Message}", name, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al buscar la subasta por nombre '{Name}': {Message}", name, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al buscar la subasta por nombre '{Name}': {Message}", name, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al buscar la subasta por nombre.");
            }
        }

      /*  [Authorize(Policy = "SubastadorOPostorPolicy")]
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAuction([FromQuery] Guid userId, [FromRoute] Guid productId)
        {
            try
            {
                var command = new GetAuctionQuery(userId, productId);
                var auction = await _mediator.Send(command);

                return Ok(auction);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al buscar la subasta del producto {ProductId} para el usuario {UserId}: {Message}", productId, userId, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al buscar la subasta del producto {ProductId} para el usuario {UserId}: {Message}", productId, userId, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al buscar la subasta del producto {ProductId} para el usuario {UserId}: {Message}", productId, userId, e.Message);
                return StatusCode(500, "Ocurrió un error al buscar la subasta.");
           }
        }*/

        [Authorize(Policy = "SubastadorPolicy")]
        [HttpPost("{auctionId:guid}/cancel")]
        public async Task<IActionResult> CancelAuction(Guid auctionId, [FromQuery] Guid userId)
        {
            try
            {
                var command = new CancelAuctionCommand(auctionId, userId);
                await _mediator.Send(command);

                _logger.LogInformation("Subasta {AuctionId} cancelada exitosamente por el usuario {UserId}.", auctionId, userId);
                return Ok(null);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("No se pudo cancelar la subasta {AuctionId}: subasta no encontrada. Detalle: {Message}", auctionId, e.Message);
                return NotFound($"Subasta no encontrada con ID {auctionId}.");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error de atributo nulo al cancelar la subasta {AuctionId}: {Message}", auctionId, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al cancelar la subasta {AuctionId}: {Message}", auctionId, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inesperado al cancelar la subasta {AuctionId}: {Message}", auctionId, ex.Message);
                return StatusCode(500, new { error = "Ocurrió un error inesperado al cancelar la subasta." });
            }
        }
       // [Authorize(Policy = "SubastadorOPostorPolicy")]
        [HttpGet("id/{id}")] // Buscar subasta solo por ID
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("ID de subasta vacío recibido.");
                    return BadRequest("El ID de la subasta es requerido.");
                }

                var query = new GetAuctionByIdQuery(id);
                var auction = await _mediator.Send(query);

                if (auction == null)
                {
                    _logger.LogInformation("No se encontró ninguna subasta con ID {AuctionId}.", id);
                    return Ok(null);
                }

                _logger.LogInformation("Subasta con ID {AuctionId} recuperada exitosamente.", id);
                return Ok(auction);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("Subasta no encontrada al buscar por ID {AuctionId}: {Message}", id, e.Message);
                return NotFound(e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al buscar subasta por ID {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al buscar subasta por ID {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al buscar subasta por ID {AuctionId}: {Message}", id, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al buscar la subasta.");
            }
        }


        //[Authorize(Policy = "SubastadorPolicy")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto updateAuctionDto, [FromQuery] Guid userId)
        {
            try
            {
                var command = new UpdateAuctionCommand(id, updateAuctionDto, userId);
                var auctionId = await _mediator.Send(command);

                _logger.LogInformation("Subasta {AuctionId} actualizada exitosamente por el usuario {UserId}.", auctionId, userId);
                return Ok(auctionId);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al actualizar la subasta {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al actualizar la subasta {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al actualizar la subasta {AuctionId}: {Message}", id, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al actualizar la subasta.");
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
                var auctionId = await _mediator.Send(command);

                _logger.LogInformation("Subasta {AuctionId} eliminada exitosamente por el usuario {UserId}.", auctionId, userId);
                return Ok(auctionId);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al intentar eliminar la subasta {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al intentar eliminar la subasta {AuctionId}: {Message}", id, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al intentar eliminar la subasta {AuctionId}: {Message}", id, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al intentar eliminar la subasta.");
            }
        }

        [Authorize(Policy = "SubastadorOPostorPolicy")]
        [HttpGet("producto-activo/{productId}")] // Trae los productos activos de una subasta
        public async Task<IActionResult> GetProductoActivo(
      [FromRoute] Guid productId,
      [FromQuery] Guid userId)
        {
            try
            {
                if (productId == Guid.Empty)
                {
                    _logger.LogWarning("Se recibió un ID de producto vacío al buscar subasta activa.");
                    return BadRequest("El ID del producto es requerido.");
                }

                var query = new GetProductAuctionQuery(productId, userId);
                var auctionDto = await _mediator.Send(query);

                if (auctionDto == null)
                {
                    _logger.LogInformation("No se encontró subasta activa para el producto con ID {ProductId}.", productId);
                    return Ok(null);
                }

                return Ok(auctionDto);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Atributo nulo al consultar subasta activa para producto {ProductId}: {Message}", productId, e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Atributo inválido al consultar subasta activa para producto {ProductId}: {Message}", productId, e.Message);
                return BadRequest(e.Message);
            }
            catch (AuctionNotFoundException e)
            {
                _logger.LogError("No se encontró la subasta activa del producto {ProductId} para el usuario {UserId}: {Message}", productId, userId, e.Message);
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado al obtener subasta activa del producto {ProductId}: {Message}", productId, e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al consultar la subasta activa del producto.");
            }
        }






    }
}