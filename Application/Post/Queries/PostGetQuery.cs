using Domain;
using MediatR;

namespace Applications.Project.Queries
{
    public class PostGetQuery : IRequest<Response>
    {
        public  Guid Id { get; set; }
    }
}
