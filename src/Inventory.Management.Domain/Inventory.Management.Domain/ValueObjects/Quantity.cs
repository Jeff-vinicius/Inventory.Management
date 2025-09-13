using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Quantidade (VO). Garante valor inteiro positivo.
    /// </summary>
    public sealed record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (value <= 0) throw new DomainException("Quantity deve ser maior que zero.");
            Value = value;
        }

        public static Quantity FromInt(int q) => new Quantity(q);
        public override string ToString() => Value.ToString();
        public static implicit operator int(Quantity q) => q.Value;
        public static implicit operator Quantity(int v) => new Quantity(v);
    }

    //public sealed record Quantity
    //{
    //    public int Value { get; }

    //    private Quantity(int value)
    //    {
    //        Value = value;
    //    }

    //    public static Quantity Create(int value)
    //    {
    //        if (value < 0)
    //            throw new DomainException("Quantity cannot be negative.");

    //        return new Quantity(value);
    //    }

    //    public static Quantity Zero => new(0);

    //    public static implicit operator int(Quantity quantity) => quantity.Value;

    //    public Quantity Add(Quantity other) => new(Value + other.Value);
    //    public Quantity Subtract(Quantity other)
    //    {
    //        if (Value < other.Value)
    //            throw new DomainException("Cannot subtract to a negative quantity.");

    //        return new(Value - other.Value);
    //    }
    //}
}