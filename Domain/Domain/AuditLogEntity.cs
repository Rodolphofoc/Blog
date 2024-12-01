namespace Domain.Domain
{
    public class AuditLogEntity : Entity
    {
        public string TableName { get; set; }

        public string ActionType { get; set; }

        public string EntityId { get; set; }

        public string Changes { get; set; }

        public string User { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
