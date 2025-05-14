using BankAccount.Events.Interfaces;
using Account = BankAccount.Domain.Model.BankAccount;

namespace BankAccount.Commands
{
    public class OpenAccountCommand
    {
        public Guid AccountId { get; set; }
        public decimal InitialDeposit { get; set; }
    }
    public class OpenAccountHandler
    {
        private readonly IEventStore _eventStore;
        public OpenAccountHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task Handle(OpenAccountCommand cmd)
        {
            var events = await _eventStore.LoadEventsAsync(cmd.AccountId);
            var account = new Account(cmd.AccountId);
            foreach (var e in events) account.Apply(e);
            var newEvents = account.OpenAccount(cmd.InitialDeposit);
            await _eventStore.AppendEventsAsync(cmd.AccountId, newEvents);
        }
    }
}
