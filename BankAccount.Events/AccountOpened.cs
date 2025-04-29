using BankAccount.Events.Interfaces;

namespace BankAccount.Events
{
    public class AccountOpened : IEvent
    {
        public AccountOpened() { }
        public AccountOpened(Guid id, decimal amount)
        {
            AggregateId = id;
            InitialDeposit = amount;
            Timestamp = DateTime.UtcNow;
        }
        public Guid AggregateId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type => nameof(AccountOpened);
        public decimal InitialDeposit { get; set; }
    }
}
