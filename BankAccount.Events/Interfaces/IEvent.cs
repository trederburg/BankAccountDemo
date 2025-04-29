namespace BankAccount.Events.Interfaces
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime Timestamp { get; }
        string Type { get; }
    }
}
