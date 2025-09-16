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
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace Inventory.Management.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/inventory")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerTag("Inventory management and product availability by store.")]
    public class InventoryController(
        IQueryHandler<GetSkuAvailabilityQuery, SkuAvailabilityResponse> getSkuHandler,
        ICommandHandler<ReserveStockCommand, ReservationResponse> reserveHandler,
        ICommandHandler<CommitReservationCommand, bool> commitHandler,
        ICommandHandler<ReleaseReservationCommand, bool> releaseHandler,
        ICommandHandler<ReplenishStockCommand, bool> replenishHandler
            ) : ControllerBase
    {

        [HttpGet("{storeId}/{sku}")]
        [SwaggerOperation(
            Summary = "Check the availability of a SKU in a store.",
            Description = "Returns information about SKU availability, including reserved quantity."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "SKU availability found", typeof(SkuAvailabilityResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Store or SKU not found")]
        public async Task<IResult> GetSkuAvailability(int storeId, string sku, CancellationToken cancellationToken)
        {
            var query = new GetSkuAvailabilityQuery(storeId, sku);

            Result<SkuAvailabilityResponse> result = await getSkuHandler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        [HttpPost("{storeId}/sku/{sku}/reserve")]
        [SwaggerOperation(
            Summary = "Reserves units of a SKU for an order.",
            Description = "Reserves stock units of a given SKU in a store for a specific order. Returns the reservation result including quantity reserved and any relevant metadata."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Reservation made successfully", typeof(ReservationResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Store or SKU not found")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Insufficient available stock")]
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

        [HttpPost("{storeId}/sku/{sku}/commit")]
        [SwaggerOperation(
            Summary = "Confirms (consumes) a previously made reservation.",
            Description = "Confirms a stock reservation previously made for a given SKU in a store. Returns a boolean indicating whether the confirmation was successful."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Reservation confirmed successfully", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Reservation not found")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Reservation inactive")]
        public async Task<IResult> CommitReservation(int storeId, string sku, [FromBody] CommitRequest request, CancellationToken cancellationToken)
        {
            var command = new CommitReservationCommand(
                storeId, 
                sku, 
                request.ReservationId);

            Result<bool> result = await commitHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        [HttpPost("{storeId}/sku/{sku}/release")]
        [SwaggerOperation(
            Summary = "Releases a previously made reservation.",
            Description = "Releases a stock reservation previously made for a given SKU in a store. Returns a boolean indicating whether the release was successful."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Reservation released successfully", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Reservation not found")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Reservation inactive")]
        public async Task<IResult> ReleaseReservation(int storeId, string sku, [FromBody] ReleaseRequest request, CancellationToken cancellationToken)
        {
            var command = new ReleaseReservationCommand(
                storeId, 
                sku, 
                request.ReservationId);

            Result<bool> result = await releaseHandler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }

        [HttpPost("{storeId}/sku/{sku}/replenish")]
        [SwaggerOperation(
            Summary = "Performs stock replenishment of a SKU.",
            Description = "Adds stock units to a given SKU in a store. Returns a boolean indicating whether the replenishment was successful."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Replenishment completed successfully", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Store or SKU not found")]
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
