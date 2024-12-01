using Applications.Interceptions.Model;
using Domain;
using MediatR;

namespace Applications.Post.Commands
{
    public class PostAddCommand : IRequest<Response>, IAuditLoggable
    {

        public string? Title { get; set; }

        public string Description { get; set; }

        #region Audit
        public string TableName => "Project";
        public string ActionType => "Insert";
        public string EntityId => string.Empty;
        public string GetChanges() => $"Title: {Title}, Description: {Description}";
        #endregion

    }
}
