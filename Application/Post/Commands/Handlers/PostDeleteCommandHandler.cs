using System.Net;
using Applications.Interfaces.Repository;
using Domain;
using MediatR;

namespace Applications.Post.Commands.Handlers
{
    public class PostDeleteCommandHandler :  IRequestHandler<PostDeleteCommand, Response>
    {

        private readonly IResponse _response;
        private readonly IPostRepository _repository;
        private readonly IUnitOfWork _unitOfWork;


        public PostDeleteCommandHandler(IResponse response, IPostRepository repository, IUnitOfWork unitOfWork)
        {
            _response = response;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(PostDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity =  await _repository.GetByIdAsync(request.Id);

                if (entity == null)
                    return await _response.CreateErrorResponseAsync(null, HttpStatusCode.NotFound);


                entity.Deleted = true;

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
