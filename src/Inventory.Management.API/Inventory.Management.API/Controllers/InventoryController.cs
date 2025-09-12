using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Inventory.Management.API.Models.DTOs;
using Inventory.Management.API.Models.Enums;

namespace Inventory.Management.API.Controllers
{
    [ApiController]
    [Route("")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class InventoryController : ControllerBase
    {
        /// <summary>
        /// Retorna a lista de produtos com quantidade no inventário da loja.
        /// </summary>
        /// <param name="storeId">ID da loja</param>
        /// <response code="200">Lista de produtos e suas quantidades no inventário</response>
        /// <response code="404">Loja não encontrada</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpGet("stores/{storeId}/inventory")]
        [ProducesResponseType(typeof(IEnumerable<ProductInventoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetStoreInventory(int storeId)
        {
            return Ok(new List<ProductInventoryDTO>());
        }

        /// <summary>
        /// Retorna o inventário de um produto específico em uma loja.
        /// </summary>
        /// <param name="storeId">ID da loja</param>
        /// <param name="productId">ID do produto</param>
        /// <response code="200">Detalhes do produto e quantidade</response>
        /// <response code="404">Loja ou produto não encontrado</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpGet("stores/{storeId}/inventory/{productId}")]
        [ProducesResponseType(typeof(ProductInventoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductInventory(int storeId, int productId)
        {
            return Ok(new ProductInventoryDTO());
        }

        /// <summary>
        /// Ajusta manualmente a quantidade de um produto no inventário.
        /// </summary>
        /// <param name="storeId">ID da loja</param>
        /// <param name="request">Dados do ajuste de inventário</param>
        /// <response code="200">Ajuste realizado com sucesso</response>
        /// <response code="400">Dados inválidos na requisição</response>
        /// <response code="404">Loja ou produto não encontrado</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpPost("stores/{storeId}/inventory/adjust")]
        [ProducesResponseType(typeof(InventoryAdjustmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AdjustInventory(int storeId, [FromBody] InventoryAdjustmentRequest request)
        {
            return Ok(new InventoryAdjustmentResponse());
        }

        /// <summary>
        /// Registra uma movimentação de inventário (INBOUND, OUTBOUND, RETURN, LOSS).
        /// </summary>
        /// <param name="storeId">ID da loja</param>
        /// <param name="request">Dados da movimentação de inventário</param>
        /// <response code="201">Movimentação registrada com sucesso</response>
        /// <response code="400">Dados inválidos na requisição</response>
        /// <response code="404">Loja ou produto não encontrado</response>
        /// <response code="409">Estoque insuficiente para a operação</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpPost("stores/{storeId}/inventory/movements")]
        [ProducesResponseType(typeof(InventoryMovementResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateInventoryMovement(int storeId, [FromBody] InventoryMovementRequest request)
        {
            var response = new InventoryMovementResponse();
            return CreatedAtAction(nameof(GetStoreInventory), new { storeId }, response);
        }

        /// <summary>
        /// Lista movimentações de inventário da loja.
        /// </summary>
        /// <param name="storeId">ID da loja</param>
        /// <param name="startDate">Data inicial do filtro (opcional)</param>
        /// <param name="endDate">Data final do filtro (opcional)</param>
        /// <param name="type">Tipo de movimentação (opcional)</param>
        /// <response code="200">Lista de movimentações</response>
        /// <response code="404">Loja não encontrada</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpGet("stores/{storeId}/inventory/movements")]
        [ProducesResponseType(typeof(IEnumerable<InventoryMovementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetInventoryMovements(
            int storeId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] InventoryMovementType? type)
        {
            return Ok(new List<InventoryMovementResponse>());
        }

        /// <summary>
        /// Cria uma transferência de inventário entre duas lojas.
        /// </summary>
        /// <param name="request">Dados da transferência de inventário</param>
        /// <response code="201">Transferência criada com sucesso</response>
        /// <response code="400">Dados inválidos na requisição</response>
        /// <response code="404">Loja ou produto não encontrado</response>
        /// <response code="409">Estoque insuficiente na loja de origem</response>
        /// <response code="500">Erro inesperado ao processar a requisição</response>
        [HttpPost("inventory/transfers")]
        [ProducesResponseType(typeof(InventoryMovementResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateInventoryTransfer([FromBody] InventoryTransferRequest request)
        {
            var response = new InventoryMovementResponse();
            return CreatedAtAction(nameof(GetStoreInventory), new { storeId = request.ToStoreId }, response);
        }
    }
}
