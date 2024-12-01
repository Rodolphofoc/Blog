namespace Applications.Project.Model
{
    public class PostModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string Description { get; set; }

        public bool? Deleted { get; set; }

        public string User { get; set; }
    }

}
