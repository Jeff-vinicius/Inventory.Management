namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record Sku
    {
        public string Value { get; }

        public Sku(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(Sku s) => s.Value;
        public static implicit operator Sku(string s) => new Sku(s);
    }
}