using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Infra.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Management.Infra.Data.UnitOfWork
{
    public class UnitOfWork(InventoryDbContext context) : IUnitOfWork
    {
        private readonly InventoryDbContext _context = context;
        private IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);

        public void Dispose() =>
            _transaction?.Dispose();
    }
}