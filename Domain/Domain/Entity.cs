namespace Domain.Domain
{
    public class Entity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public bool Deleted { get; set; }

        public string? LastModifiedBy { get; set; }

        protected Entity()
        {
        }

    }
}
