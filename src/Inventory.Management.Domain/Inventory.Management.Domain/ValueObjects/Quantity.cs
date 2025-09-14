using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (value <= 0) throw new DomainException("Quantity must be greater than zero!");
            Value = value;
        }

        public static Quantity FromInt(int q) => new Quantity(q);
        public override string ToString() => Value.ToString();
        public static implicit operator int(Quantity q) => q.Value;
        public static implicit operator Quantity(int v) => new Quantity(v);
    }
}