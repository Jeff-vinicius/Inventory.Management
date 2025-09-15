using Inventory.Management.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Management.Infra.Data.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(i => new { i.StoreId, i.Sku });

            builder.Property(i => i.StoreId)
                .HasConversion(
                    v => v.Value,
                    v => new Domain.ValueObjects.StoreId(v))
                .IsRequired();

            builder.Property(i => i.Sku)
                .HasConversion(
                    v => v.Value,
                    v => new Domain.ValueObjects.Sku(v))
                .IsRequired();

            builder.Property(i => i.AvailableQuantity)
                .IsRequired();

            builder.Property(i => i.ReservedQuantity)
                .IsRequired();

            builder.Property(i => i.Version)
                .IsConcurrencyToken()
                .IsRequired();

            builder.Property(i => i.LastUpdatedAt)
                .IsRequired();

            builder.HasMany(i => i.Reservations) 
                .WithOne()
                .HasForeignKey("InventoryStoreId", "InventorySku")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata
                .FindNavigation(nameof(InventoryItem.Reservations))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(i => i.Events);
        }
    }
}