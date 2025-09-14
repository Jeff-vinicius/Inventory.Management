using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record OrderId
    {
        public string Value { get; }

        public OrderId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("OrderId cannot be empty!");
            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(OrderId o) => o.Value;
        public static implicit operator OrderId(string s) => new OrderId(s);
    }
}
