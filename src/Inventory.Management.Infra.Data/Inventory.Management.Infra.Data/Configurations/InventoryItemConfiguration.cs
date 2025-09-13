using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Management.Infra.Data.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            // Chave composta: StoreId + Sku
            builder.HasKey(i => new { i.StoreId, i.Sku });

            builder.Property(i => i.StoreId)
                .HasConversion(
                    v => v.Value, // StoreId -> string
                    v => new Domain.ValueObjects.StoreId(v))
                .IsRequired();

            builder.Property(i => i.Sku)
                .HasConversion(
                    v => v.Value, // Sku -> string
                    v => new Domain.ValueObjects.Sku(v))
                .IsRequired();

            builder.Property(i => i.AvailableQuantity)
                .IsRequired();

            builder.Property(i => i.ReservedQuantity)
                .IsRequired();

            builder.Property(i => i.Version)
                .IsConcurrencyToken() // usado para optimistic concurrency
                .IsRequired();

            builder.Property(i => i.LastUpdatedAt)
                .IsRequired();

            // Relação 1:N com Reservations
            builder.HasMany<Reservation>("_reservations") // campo privado
                .WithOne()
                .HasForeignKey("InventoryStoreId", "InventorySku"); // FK shadow properties

            // Ignora lista de eventos
            builder.Ignore(i => i.Events);
        }
    }

    //public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    //{
    //    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    //    {
    //        builder.ToTable("InventoryItems");

    //        // Chave composta
    //        builder.HasKey(i => new { i.StoreId, i.Sku });

    //        // Value Objects como Owned Types
    //        builder.OwnsOne(i => i.StoreId, storeId =>
    //        {
    //            storeId.Property(s => s.Value)
    //                .HasColumnName("StoreId")
    //                .IsRequired();
    //        });

    //        builder.OwnsOne(i => i.Sku, sku =>
    //        {
    //            sku.Property(s => s.Value)
    //                .HasColumnName("Sku")
    //                .IsRequired()
    //                .HasMaxLength(20);
    //        });

    //        builder.OwnsOne(i => i.AvailableQuantity, qty =>
    //        {
    //            qty.Property(q => q.Value)
    //                .HasColumnName("AvailableQuantity")
    //                .IsRequired();
    //        });

    //        builder.OwnsOne(i => i.ReservedQuantity, qty =>
    //        {
    //            qty.Property(q => q.Value)
    //                .HasColumnName("ReservedQuantity")
    //                .IsRequired();
    //        });

    //        // Relacionamento com Reservations
    //        builder.HasMany(i => i.Reservations)
    //            .WithOne()
    //            .HasForeignKey("InventoryItemStoreId", "InventoryItemSku")
    //            .IsRequired();

    //        // Propriedades de auditoria
    //        builder.Property(i => i.CreatedAt)
    //            .IsRequired();

    //        builder.Property(i => i.UpdatedAt);
    //    }
    //}
}