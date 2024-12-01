using System.Diagnostics.CodeAnalysis;
using Applications.Interfaces.Repository;
using Domain.Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]

    public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : Entity
    {
        private readonly BlogContext _context;

        public Repository(BlogContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            var result = _context.Set<TEntity>().Add(entity);

            return result.Entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await _context.Set<TEntity>().AddAsync(entity)).Entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public async Task<TEntity> DeleteAsync(Guid Id)
        {
            var entity = await _context
                .Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == Id);

            return _context.Set<TEntity>().Remove(entity).Entity;
        }

        public TEntity Delete(Guid Id)
        {
            var entity = _context
                .Set<TEntity>()
                .FirstOrDefault(x => x.Id == Id);

            return _context
                .Set<TEntity>()
                .Remove(entity).Entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>()
            .AsQueryable();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>()
                .ToListAsync();
        }

        public TEntity FindById(Guid Id)
        {
            var result = _context
                .Set<TEntity>()
                .FirstOrDefault(x => x.Id == Id);

            return result;
        }

        public async Task<TEntity> FindByIdAsync(Guid Id)
        {
            var result = await _context
                .Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }

        public async Task<TEntity> FindByIdAsNoTrackingAsync(Guid Id)
        {
            var result = await _context
                .Set<TEntity>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }


        public TEntity Update(TEntity entity)
        {
            var e = _context.Set<TEntity>().FirstOrDefault(x => x.Id == entity.Id);

            entity.Id = e.Id;

            _context.DetachLocal<TEntity>(entity, e.Id);

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var e = await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == entity.Id);

            entity.Id = e.Id;

            _context.DetachLocal<TEntity>(entity, e.Id);

            return entity;
        }

        public IQueryable<TEntity> ListAsQueryable()
        {
            return _context.Set<TEntity>();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
