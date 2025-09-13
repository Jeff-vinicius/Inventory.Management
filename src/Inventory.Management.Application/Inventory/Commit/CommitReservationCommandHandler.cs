using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Commit
{
    internal sealed class CommitReservationCommandHandler() : ICommandHandler<CommitReservationCommand, bool>
    {
        public async Task<Result<bool>> Handle(
            CommitReservationCommand command,
            CancellationToken cancellationToken)
        {

            return Result.Success(true);

        }
    }
}
