namespace BankAccount.Events.Interfaces
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> LoadEventsAsync(Guid aggregateId);
        Task AppendEventsAsync(Guid aggregateId, IEnumerable<IEvent> events);
    }
}
