using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record StoreId
    {
        public string Value { get; }

        private StoreId(string value)
        {
            Value = value;
        }

        public static StoreId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Store ID cannot be empty.");

            return new StoreId(value);
        }

        public static implicit operator string(StoreId storeId) => storeId.Value;
    }
}