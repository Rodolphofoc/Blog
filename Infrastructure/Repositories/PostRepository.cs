using Applications.Interfaces.Repository;
using Domain.Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository : Repository<PostEntity>, IPostRepository
    {
        private readonly BlogContext _context;

        public PostRepository(BlogContext context) : base(context)
        { 
            _context = context;
        }


        public async Task<(List<PostEntity> entities, int totalPage, int totalRecords)> Filter(string name, bool? deleted , int pageSize, int pageNumber)
        {
            int skip = (pageNumber -1) * pageSize;

            var query = _context.Post.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Title.ToLower() == name.ToLower()).AsQueryable();

            if (deleted is not null)
                query = query.Where(x => x.Deleted == deleted).AsQueryable();

            query = query.Include(x => x.User);

            var totalPage = (int)Math.Ceiling((double)_context.Post.Count() /pageSize);

            var totalRecords = query.Count();

            return (entities: await query.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync(), totalPage: totalPage, totalRecords: totalRecords);
        }


        public async Task<PostEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Post.Where(x => x.Id == id)
                .Include(x => x.User).FirstOrDefaultAsync();
        }
    }
}
