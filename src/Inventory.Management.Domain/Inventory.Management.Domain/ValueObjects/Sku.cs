using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record Sku
    {
        public string Value { get; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Sku Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("SKU cannot be empty.");

            if (!IsValidFormat(value))
                throw new DomainException("Invalid SKU format.");

            return new Sku(value.ToUpperInvariant());
        }

        private static bool IsValidFormat(string value)
        {
            return value.Length >= 4 && value.Length <= 20 
                && value.All(c => char.IsLetterOrDigit(c) || c == '-');
        }

        public static implicit operator string(Sku sku) => sku.Value;
    }
}