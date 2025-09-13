using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Identificador da loja (VO imutável).
    /// </summary>
    public sealed record StoreId
    {
        public int Value { get; }

        public StoreId(int value)
        {
            Value = value;
        }
        public static implicit operator StoreId(string s) => new StoreId(s);
    }

}