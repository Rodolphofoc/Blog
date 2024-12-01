using Applications.Interceptions.Model;
using Domain;
using MediatR;

namespace Applications.Post.Commands
{
    public class PostUpdateCommand : IRequest<Response>, IAuditLoggable
    {
        private Guid Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool Deleted { get; set; }
        public void SetId(Guid id) { Id = id; }
        public Guid GetId()
        {
            return Id;
        }

        #region Audit
        public string TableName => "Project";
        public string ActionType => "Update";
        public string EntityId => Id.ToString();
        public string GetChanges() => $" Name: {Title}, Description:{Description} ,  Deleted: {Deleted}";
        #endregion


    }
}
