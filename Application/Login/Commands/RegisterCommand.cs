using Applications.Interceptions.Model;
using Domain;
using MediatR;

namespace Applications.Login.Commands
{
    public class RegisterCommand : IRequest<Response>, IAuditLoggable
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string TableName => "Project";
        public string ActionType => "Register";
        public string EntityId => string.Empty;
        public string GetChanges() => $"Description: login, User {Login}";
    }
}
