using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Replenish
{
    internal sealed class ReplenishStockCommandHandler() : ICommandHandler<ReplenishStockCommand, bool>
    {
        public async Task<Result<bool>> Handle(
            ReplenishStockCommand command,
            CancellationToken cancellationToken)
        {

            return Result.Success(true);

        }
    }
}
