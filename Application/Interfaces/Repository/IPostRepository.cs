using Domain.Domain;

namespace Applications.Interfaces.Repository
{
    public interface IPostRepository : IRepository<PostEntity>
    {
        Task<(List<PostEntity> entities, int totalPage, int totalRecords)> Filter(string name, bool? deleted, int pageSize, int pageNumber);

        Task<PostEntity?> GetByIdAsync(Guid id);
    }
}
