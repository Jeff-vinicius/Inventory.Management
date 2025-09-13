namespace Inventory.Management.Domain.Interfaces
{
    public interface IUnitOfWork //: IDisposable TODO - revisar se faz sentido implementar IDisposable
    {
        //Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        //Task CommitAsync(CancellationToken cancellationToken = default);
        //Task RollbackAsync(CancellationToken cancellationToken = default);

        //TODO - revisar se faz sentido somente usar o SaveChangesAsync do DbContext
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
