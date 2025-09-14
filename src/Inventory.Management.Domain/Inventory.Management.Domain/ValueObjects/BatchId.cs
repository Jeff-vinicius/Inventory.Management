using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record BatchId
    {
        public string Value { get; }

        public BatchId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("BatchId cannot be empty!");
            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(BatchId b) => b.Value;
        public static implicit operator BatchId(string s) => new BatchId(s);
    }
}