using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Management.Domain.Aggregates;

namespace Inventory.Management.Infra.Data.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            // Chave composta
            builder.HasKey(i => new { i.StoreId, i.Sku });

            // Value Objects como Owned Types
            builder.OwnsOne(i => i.StoreId, storeId =>
            {
                storeId.Property(s => s.Value)
                    .HasColumnName("StoreId")
                    .IsRequired();
            });

            builder.OwnsOne(i => i.Sku, sku =>
            {
                sku.Property(s => s.Value)
                    .HasColumnName("Sku")
                    .IsRequired()
                    .HasMaxLength(20);
            });

            builder.OwnsOne(i => i.AvailableQuantity, qty =>
            {
                qty.Property(q => q.Value)
                    .HasColumnName("AvailableQuantity")
                    .IsRequired();
            });

            builder.OwnsOne(i => i.ReservedQuantity, qty =>
            {
                qty.Property(q => q.Value)
                    .HasColumnName("ReservedQuantity")
                    .IsRequired();
            });

            // Relacionamento com Reservations
            builder.HasMany(i => i.Reservations)
                .WithOne()
                .HasForeignKey("InventoryItemStoreId", "InventoryItemSku")
                .IsRequired();

            // Propriedades de auditoria
            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt);
        }
    }
}