namespace BankAccount.Events.Interfaces
{
    public interface IEvent
    {
        Guid AccountId { get; }
        DateTime Timestamp { get; }
        string Type { get; }
    }
}
