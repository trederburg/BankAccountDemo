namespace BankAccount.Infrastructure
{
    public class EventDocument
    {
        public string Id { get; set; } // cosmos id
        public Guid AggregateId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public object EventData { get; set; } // raw object
    }
}
