using Applications.Interceptions.Model;
using Domain;
using MediatR;

namespace Applications.Login.Commands
{
    public class LoginCommand : IRequest<Response>, IAuditLoggable
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string TableName => "Project";
        public string ActionType => "login";
        public string EntityId => string.Empty;
        public string GetChanges() => $"Description: login, User {User}";

    }
}
