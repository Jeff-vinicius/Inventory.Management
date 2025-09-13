using Inventory.Management.Domain.Common;
using System.Text.RegularExpressions;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Código do produto (VO imutável). Valida formato simples alfanumérico com caracteres -/_ permitidos.
    /// </summary>
    public sealed record Sku
    {
        private static readonly Regex Allowed = new(@"^[A-Za-z0-9\-_]+$", RegexOptions.Compiled);

        public string Value { get; }

        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("SKU não pode ser vazio.");

            if (!Allowed.IsMatch(value))
                throw new DomainException("SKU em formato inválido.");

            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(Sku s) => s.Value;
        public static implicit operator Sku(string s) => new Sku(s);
    }

    //public sealed record Sku
    //{
    //    public string Value { get; }

    //    private Sku(string value)
    //    {
    //        Value = value;
    //    }

    //    public static Sku Create(string value)
    //    {
    //        if (string.IsNullOrWhiteSpace(value))
    //            throw new DomainException("SKU cannot be empty.");

    //        if (!IsValidFormat(value))
    //            throw new DomainException("Invalid SKU format.");

    //        return new Sku(value.ToUpperInvariant());
    //    }

    //    private static bool IsValidFormat(string value)
    //    {
    //        return value.Length >= 4 && value.Length <= 20 
    //            && value.All(c => char.IsLetterOrDigit(c) || c == '-');
    //    }

    //    public static implicit operator string(Sku sku) => sku.Value;
    //}
}