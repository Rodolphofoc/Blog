using System.Diagnostics.CodeAnalysis;
using Applications.Interfaces.Repository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(BlogContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            var completeResult = await CompleteAsync(cancellationToken);

            await _transaction.CommitAsync(cancellationToken);

            return completeResult;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            return result;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }


    }
}
