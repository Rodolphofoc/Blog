
namespace Domain.Domain
{
    public class PostEntity : Entity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid UserId { get; set; }

        public virtual UserEntity User { get; set; }


    }
}
