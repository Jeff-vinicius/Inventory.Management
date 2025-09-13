namespace Inventory.Management.Domain.Common
{
    public abstract class Entity
    {
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected Entity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}