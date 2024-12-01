using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Applications.Interfaces.Repository;
using Domain;
using Domain.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Applications.Login.Commands.Handler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response>
    {
        private readonly IResponse _response;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;


        public LoginCommandHandler(IResponse response, IUserRepository userRepository, IConfiguration configuration)
        {
            _response = response;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Response> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.User) || string.IsNullOrEmpty(request.Password))
                    return await _response.CreateErrorResponseAsync("Name or password not be empty", HttpStatusCode.BadRequest);

                var user = await _userRepository.GetByLoginAndPassword(request.User, request.Password);

                if (user is null)
                    return await _response.CreateErrorResponseAsync("Login or password wrong", HttpStatusCode.Unauthorized);

                var token = await GenerateJwt(user);

                return await _response.CreateSuccessResponseAsync(token, string.Empty);

            }
            catch (Exception)
            {
                return await _response.CreateErrorResponseAsync(null, HttpStatusCode.InternalServerError);
            }
        }


        public async Task<string> GenerateJwt(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];


            var claims = new[]
             {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Sid, user.Id.ToString())
             };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddHours(1),
               signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }



}
