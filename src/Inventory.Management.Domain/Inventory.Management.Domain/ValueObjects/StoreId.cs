namespace Inventory.Management.Domain.ValueObjects
{
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