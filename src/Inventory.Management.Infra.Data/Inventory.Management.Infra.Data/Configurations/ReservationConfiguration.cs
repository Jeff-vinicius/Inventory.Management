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

            builder.Property(r => r.ReservationId)
                .IsRequired();

            builder.Property(r => r.OrderId)
                .HasConversion(
                    v => v.Value,
                    v => new Domain.ValueObjects.OrderId(v))
                .IsRequired();

            builder.Property(r => r.Quantity)
                .HasConversion(
                    v => v.Value,
                    v => new Domain.ValueObjects.Quantity(v))
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt);
        }
    }
}