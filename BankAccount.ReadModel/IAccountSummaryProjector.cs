using BankAccount.Events.Interfaces;

namespace BankAccount.ReadModel
{
    public interface IAccountSummaryProjector
    {
        Task Handle(IEvent @event);
    }
}
