namespace BankAccount.Infrastructure
{
    public class EventDocument
    {
        public string id { get; set; } // cosmos id
        public string aggregateId { get; set; } // cosmos partition key
        public Guid AccountId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public object EventData { get; set; } // raw object
    }
}
