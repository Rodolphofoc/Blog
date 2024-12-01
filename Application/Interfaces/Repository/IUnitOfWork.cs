namespace Applications.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
