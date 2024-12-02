using Domain.Domain;

namespace Applications.Interfaces.Repository
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByLoginAndPassword(string username, string password);

        Task<UserEntity?> GetByName(string username);
    }
}
