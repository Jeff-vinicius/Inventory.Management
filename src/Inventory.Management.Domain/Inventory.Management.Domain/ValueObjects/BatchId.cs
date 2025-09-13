using Inventory.Management.Domain.Common;

namespace Inventory.Management.Domain.ValueObjects
{
    /// <summary>
    /// Identificador de lote para reposição (VO).
    /// </summary>
    public sealed record BatchId
    {
        public string Value { get; }

        public BatchId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("BatchId não pode ser vazio.");
            Value = value;
        }

        public override string ToString() => Value;
        public static implicit operator string(BatchId b) => b.Value;
        public static implicit operator BatchId(string s) => new BatchId(s);
    }

    //public sealed record BatchId
    //{
    //    public string Value { get; }

    //    private BatchId(string value)
    //    {
    //        Value = value;
    //    }

    //    public static BatchId Create(string value)
    //    {
    //        if (string.IsNullOrWhiteSpace(value))
    //            throw new DomainException("Batch ID cannot be empty.");

    //        return new BatchId(value);
    //    }

    //    public static implicit operator string(BatchId batchId) => batchId.Value;
    //}
}