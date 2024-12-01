using Applications.Post.Commands;
using Applications.Project.Model;
using Domain.Domain;

namespace Applications.Mappers.Interface
{
    public interface IPostMappers
    {
        PostEntity Map(PostAddCommand command);

        PostEntity Map(PostUpdateCommand command);

        List<PostModel> Map(List<PostEntity> command);

    }
}
