namespace BankAccount.Events.Interfaces
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> LoadEventsAsync(Guid accountId);
        Task AppendEventsAsync(Guid accountId, IEnumerable<IEvent> events);
    }
}
