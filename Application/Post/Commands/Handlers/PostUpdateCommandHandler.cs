using System.Net;
using Applications.Interfaces.Repository;
using Applications.Mappers.Interface;
using Domain;
using MediatR;

namespace Applications.Post.Commands.Handlers
{
    public class PostUpdateCommandHandler : IRequestHandler<PostUpdateCommand, Response>
    {
        private readonly IResponse _response;
        private readonly IPostRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostMappers _taskManagerMappers;


        public PostUpdateCommandHandler(IResponse response, IPostRepository repository, IPostMappers taskManagerMappers, IUnitOfWork unitOfWork)
        {
            _response = response;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _taskManagerMappers = taskManagerMappers;
        }

        public async Task<Response> Handle(PostUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.FindByIdAsNoTrackingAsync(request.GetId());

                if (entity == null)
                    return await _response.CreateErrorResponseAsync(null, HttpStatusCode.NotFound);


                entity.Description = request.Description;
                entity.Title = request.Title;
                entity.Deleted = request.Deleted;

                await _repository.UpdateAsync(entity);  

                await _unitOfWork.CompleteAsync();

                return await _response.CreateSuccessResponseAsync(null, string.Empty);

            }
            catch (Exception)
            {
                return await _response.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError);
            }
        }
    }
}
