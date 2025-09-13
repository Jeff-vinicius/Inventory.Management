using Inventory.Management.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Identificador de pedido (VO).
    /// </summary>
    public sealed record OrderId
    {
        public string Value { get; }

        public OrderId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("OrderId não pode ser vazio.");
            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(OrderId o) => o.Value;
        public static implicit operator OrderId(string s) => new OrderId(s);
    }
}
