using Inventory.Management.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Management.Domain.Errors
{
    public static class InventoryErrors
    {
        public static Error NotFound() => Error.NotFound(
            "Inventory.NotFound",
            $"The inventory was not found"); //TODO - melhorar mensagem
    }
}
