using Inventory.Management.Domain.Common;
using System.Text.RegularExpressions;

namespace Inventory.Management.Domain.ValueObjects
{
    public sealed record Sku
    {
        private static readonly Regex Allowed = new(@"^[A-Za-z0-9\-_]+$", RegexOptions.Compiled);

        public string Value { get; }

        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("SKU cannot be empty!");

            if (!Allowed.IsMatch(value))
                throw new DomainException("SKU in invalid format!");

            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(Sku s) => s.Value;
        public static implicit operator Sku(string s) => new Sku(s);
    }
}