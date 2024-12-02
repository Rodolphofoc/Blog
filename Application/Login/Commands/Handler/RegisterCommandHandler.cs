using System.Net;
using Applications.Interfaces.Repository;
using Domain;
using Domain.Domain;
using MediatR;

namespace Applications.Login.Commands.Handler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response>
    {
        private readonly IResponse _response;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IResponse response, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _response = response;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password))
                    return await _response.CreateErrorResponseAsync("Name or password not be empty", HttpStatusCode.BadRequest);

                var user = await _userRepository.GetByName(request.Login);

                if (user is not null)
                    return await _response.CreateErrorResponseAsync("Login in use", HttpStatusCode.Unauthorized);


                await _userRepository.AddAsync(new UserEntity()
                {
                    Name = request.Login,
                    Password = request.Password,
                });

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
