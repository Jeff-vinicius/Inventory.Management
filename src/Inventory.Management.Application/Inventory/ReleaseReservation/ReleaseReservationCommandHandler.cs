using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Application.Inventory.Release;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.ReleaseReservation
{
    internal sealed class ReleaseReservationCommandHandler() : ICommandHandler<ReleaseReservationCommand, bool>
    {
        public async Task<Result<bool>> Handle(
            ReleaseReservationCommand command,
            CancellationToken cancellationToken)
        {

            return Result.Success(true);

        }
    }
}
