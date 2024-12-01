using Applications.Interceptions.Model;
using Domain;
using MediatR;

namespace Applications.Post.Commands
{
    public class PostDeleteCommand : IRequest<Response>, IAuditLoggable
    {
        public Guid Id { get; set; }
        #region Audit
        public string TableName => "Project";
        public string ActionType => "Delete";
        public string EntityId => Id.ToString();
        public string GetChanges() => $" Description: Delete Post";
        #endregion

    }
}
