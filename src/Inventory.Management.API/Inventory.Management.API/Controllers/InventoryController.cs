using Inventory.Management.API.Extensions;
using Inventory.Management.API.Infrastructure;
using Inventory.Management.API.Models.Inventory;
using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Application.Inventory.Commit;
using Inventory.Management.Application.Inventory.GetAvailability;
using Inventory.Management.Application.Inventory.ReleaseReservation;
using Inventory.Management.Application.Inventory.Replenish;
using Inventory.Management.Application.Inventory.Reserve;
using Inventory.Management.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Inventory.Management.API.Controllers
{
    /// <summary>
    /// Gerenciamento de inventário e disponibilidade de produtos por loja.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/inventory")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class InventoryController(
        IQueryHandler<GetSkuAvailabilityQuery, SkuAvailabilityResponse> getSkuHandler,
        ICommandHandler<ReserveStockCommand, ReservationResponse> reserveHandler,
        ICommandHandler<CommitReservationCommand, bool> commitHandler,
        ICommandHandler<ReleaseReservationCommand, bool> releaseHandler,
        ICommandHandler<ReplenishStockCommand, bool> replenishHandler
            ) : ControllerBase
    {
        /// <summary>
        /// Consulta a disponibilidade de um SKU em uma loja.
        /// </summary>
        /// <param name="storeId">Identificador da loja</param>
        /// <param name="sku">Código do produto</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Informações de disponibilidade do SKU</returns>
        /// <response code="200">Disponibilidade do SKU encontrada</response>
        /// <response code="404">Loja ou SKU não encontrado</response>
        /// <example>
        /// {
        ///    "storeId": 1,
        ///    "sku": "SKU123",
        ///    "available": 50,
        ///    "reserved": 5
        /// }
        /// </example>
        [HttpGet("{storeId}/sku/{sku}")]
        [ProducesResponseType(typeof(SkuAvailabilityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetSkuAvailability(int storeId, string sku, CancellationToken cancellationToken)
        {
            var query = new GetSkuAvailabilityQuery(storeId, sku);

            Result<SkuAvailabilityResponse> result = await getSkuHandler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        /// <summary>
        /// Reserva unidades de um SKU para um pedido.
        /// </summary>
        /// <param name="storeId">Identificador da loja</param>
        /// <param name="sku">Código do produto</param>
        /// <param name="request">Dados da reserva</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Resultado da reserva ou conflito</returns>
        /// <response code="200">Reserva realizada com sucesso</response>
        /// <response code="404">Loja ou SKU não encontrado</response>
        /// <response code="409">Quantidade insuficiente em estoque</response>
        [HttpPost("{storeId}/sku/{sku}/reserve")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)] //TODO: avaliar como lidar com conflitos
        public async Task<IResult> ReserveStock(int storeId, string sku, [FromBody] ReservationRequest request, CancellationToken cancellationToken)
        {
            var command = new ReserveStockCommand(
                storeId, 
                sku, 
                request.Quantity, 
                request.OrderId);

            Result<ReservationResponse> result = await reserveHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        /// <summary>
        /// Confirma (consome) uma reserva previamente realizada.
        /// </summary>
        /// <param name="storeId">Identificador da loja</param>
        /// <param name="sku">Código do produto</param>
        /// <param name="request">Dados da confirmação</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Resultado da confirmação</returns>
        /// <response code="200">Reserva confirmada com sucesso</response>
        /// <response code="404">Reserva não encontrada</response>
        [HttpPost("{storeId}/sku/{sku}/commit")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> CommitReservation(int storeId, string sku, [FromBody] CommitRequest request, CancellationToken cancellationToken)
        {
            var command = new CommitReservationCommand(
                storeId, 
                sku, 
                request.ReservationId, 
                request.Quantity);

            Result<bool> result = await commitHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        /// <summary>
        /// Libera uma reserva previamente realizada.
        /// </summary>
        /// <param name="storeId">Identificador da loja</param>
        /// <param name="sku">Código do produto</param>
        /// <param name="request">Dados da liberação</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Resultado da liberação</returns>
        /// <response code="200">Reserva liberada com sucesso</response>
        /// <response code="404">Reserva não encontrada</response>
        [HttpPost("{storeId}/sku/{sku}/release")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> ReleaseReservation(int storeId, string sku, [FromBody] ReleaseRequest request, CancellationToken cancellationToken)
        {
            var command = new ReleaseReservationCommand(
                storeId, 
                sku, 
                request.ReservationId);

            Result<bool> result = await releaseHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        /// <summary>
        /// Realiza reposição de estoque de um SKU.
        /// </summary>
        /// <param name="storeId">Identificador da loja</param>
        /// <param name="sku">Código do produto</param>
        /// <param name="request">Dados da reposição</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Resultado da reposição</returns>
        /// <response code="200">Reposição realizada com sucesso</response>
        /// <response code="404">Loja ou SKU não encontrado</response>
        [HttpPost("{storeId}/sku/{sku}/replenish")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> ReplenishStock(int storeId, string sku, [FromBody] ReplenishRequest request, CancellationToken cancellationToken)
        {
            var command = new ReplenishStockCommand(
                storeId, 
                sku, 
                request.Quantity, 
                request.BatchId);

            Result<bool> result = await replenishHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }
    }
}
