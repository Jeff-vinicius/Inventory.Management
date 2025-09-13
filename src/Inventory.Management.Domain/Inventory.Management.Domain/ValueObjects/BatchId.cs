using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record BatchId
    {
        public string Value { get; }

        private BatchId(string value)
        {
            Value = value;
        }

        public static BatchId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Batch ID cannot be empty.");

            return new BatchId(value);
        }

        public static implicit operator string(BatchId batchId) => batchId.Value;
    }
}