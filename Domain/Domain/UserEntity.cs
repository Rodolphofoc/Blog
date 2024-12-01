namespace Domain.Domain
{
    public class UserEntity : Entity
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public virtual ICollection<PostEntity> Post { get; set; }
        
    }
}
