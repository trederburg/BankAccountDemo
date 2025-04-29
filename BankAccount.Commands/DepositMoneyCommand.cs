using BankAccount.Events.Interfaces;
using Account = BankAccount.Domain.Model.BankAccount;

namespace BankAccount.Commands
{
    public class DepositMoneyCommand
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }

    public class DepositMoneyHandler
    {
        private readonly IEventStore _eventStore;

        public async Task Handle(DepositMoneyCommand cmd)
        {
            var events = await _eventStore.LoadEventsAsync(cmd.AccountId);
            var account = new Account(cmd.AccountId);

            foreach (var e in events) account.Apply(e);

            var newEvents = account.Deposit(cmd.Amount);
            await _eventStore.AppendEventsAsync(cmd.AccountId, newEvents);
        }
    }
}
