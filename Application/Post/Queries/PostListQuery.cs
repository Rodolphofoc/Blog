using Domain;
using MediatR;

namespace Applications.Project.Queries
{
    public class PostListQuery : IRequest<Response>
    {
        public string? Name { get; set; }
        public int? PageSize { get; set; }

        public int? Page { get; set; }

        public bool? Deleted { get; set; }

        public PostListQuery()
        {
            PageSize = 10;
            Page = 1;
        }

    }
}
