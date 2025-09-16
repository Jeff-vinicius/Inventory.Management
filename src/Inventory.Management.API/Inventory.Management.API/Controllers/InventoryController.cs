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
    /// Inventory management and product availability by store.
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
        /// Check the availability of a SKU in a store.
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="sku">Product code</param>
        /// <param name="cancellationToken"></param>
        /// <returns>SKU availability information</returns>
        /// <response code="200">SKU availability found</response>
        /// <response code="404">Store or SKU not found</response>
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
        /// Reserves units of a SKU for an order.
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="sku">Product code</param>
        /// <param name="request">Reservation details</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Reservation result</returns>
        /// <response code="200">Reservation made successfully</response>
        /// <response code="404">Store or SKU not found</response>
        /// <response code="409">Insufficient available stock</response>
        [HttpPost("{storeId}/sku/{sku}/reserve")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// Confirms (consumes) a previously made reservation.
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="sku">Product code</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Confirmation result</returns>
        /// <response code="200">Reservation confirmed successfully</response>
        /// <response code="404">Reservation not found</response>
        /// <response code="409">Reservation inactive</response>
        [HttpPost("{storeId}/sku/{sku}/commit")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> CommitReservation(int storeId, string sku, [FromBody] CommitRequest request, CancellationToken cancellationToken)
        {
            var command = new CommitReservationCommand(
                storeId, 
                sku, 
                request.ReservationId);

            Result<bool> result = await commitHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        /// <summary>
        /// Releases a previously made reservation.
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="sku">Product code</param>
        /// <param name="request">Release data</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Release result</returns>
        /// <response code="200">Reservation released successfully</response>
        /// <response code="404">Reservation not found</response>
        /// <response code="409">Reservation inactive</response>
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
        /// Performs stock replenishment of a SKU.
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="sku">Product code</param>
        /// <param name="request">Replacement data</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Replacement result</returns>
        /// <response code="200">Replacement completed successfully</response>
        /// <response code="404">Store or SKU not found</response>
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
