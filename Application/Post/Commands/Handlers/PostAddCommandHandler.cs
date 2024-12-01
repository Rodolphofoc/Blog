using System.Net;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Applications.Socket;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Applications.Post.Commands.Handlers
{
    public class PostAddCommandHandler : IRequestHandler<PostAddCommand, Response>
    {
        private readonly IResponse _response;
        private readonly IPostRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostMappers _taskManagerMappers;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<PostHub> _hubContext;



        public PostAddCommandHandler(IResponse response,  IPostRepository repository, IUnitOfWork unitOfWork, IPostMappers taskManagerMappers, IHttpContextAccessor httpContextAccessor, IHubContext<PostHub> hubContext)
        {
            _response = response;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _taskManagerMappers = taskManagerMappers;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;

        }

        public async Task<Response> Handle(PostAddCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sid = _httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value ?? "Unknown";
                var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

                if (string.IsNullOrEmpty(request.Description) || string.IsNullOrEmpty(request.Title))
                    return await _response.CreateErrorResponseAsync("Name or description be empty", HttpStatusCode.BadRequest);

                var entity = _taskManagerMappers.Map(request);
                entity.UserId = Guid.Parse(sid);
                entity.LastModifiedBy = user;

                await _repository.AddAsync(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                await _hubContext.Clients.All.SendAsync("ReceivePostAdded", $"Post {entity.Title} added by {user}");


                return await _response.CreateSuccessResponseAsync(null, string.Empty);

            }
            catch (Exception ex)
            {
                return await _response.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError);
            }
        }
    }
}
