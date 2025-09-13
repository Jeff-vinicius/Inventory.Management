using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Management.Domain.Entities;

namespace Inventory.Management.Infra.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.ReservationId);

            builder.Property(r => r.OrderId)
                .IsRequired();

            builder.OwnsOne(r => r.Quantity, qty =>
            {
                qty.Property(q => q.Value)
                    .HasColumnName("Quantity")
                    .IsRequired();
            });

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt);
        }
    }
}