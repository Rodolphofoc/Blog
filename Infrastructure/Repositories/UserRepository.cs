using Applications.Interfaces.Repository;
using Domain.Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<PostEntity>, IUserRepository
    {
        private readonly BlogContext _context;

        public UserRepository(BlogContext context) : base(context)
        {
            _context = context;
        }


        public async Task<UserEntity?> GetByLoginAndPassword(string username, string password)
        {
            return await _context.User.FirstOrDefaultAsync(x => x.Name.Equals(username) && x.Password.Equals(password));
        }
    }
}
