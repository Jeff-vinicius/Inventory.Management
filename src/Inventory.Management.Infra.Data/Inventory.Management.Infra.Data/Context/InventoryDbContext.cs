using Microsoft.EntityFrameworkCore;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Entities;

namespace Inventory.Management.Infra.Data.Context
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}