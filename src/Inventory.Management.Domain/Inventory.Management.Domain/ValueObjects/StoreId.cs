using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Identificador da loja (VO imutável).
    /// </summary>
    public sealed record StoreId
    {
        public string Value { get; }

        public StoreId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("StoreId não pode ser vazio.");

            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(StoreId s) => s.Value;
        public static implicit operator StoreId(string s) => new StoreId(s);
    }

    //public sealed record StoreId
    //{
    //    public string Value { get; }

    //    private StoreId(string value)
    //    {
    //        Value = value;
    //    }

    //    public static StoreId Create(string value)
    //    {
    //        if (string.IsNullOrWhiteSpace(value))
    //            throw new DomainException("Store ID cannot be empty.");

    //        return new StoreId(value);
    //    }

    //    public static implicit operator string(StoreId storeId) => storeId.Value;
    //}
}