using Domain.Domain;

namespace Applications.Interfaces.Repository
{
    public interface IUserRepository : IRepository<PostEntity>
    {
        Task<UserEntity?> GetByLoginAndPassword(string username, string password);
    }
}
