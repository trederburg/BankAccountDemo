using BankAccount.Events.Interfaces;

namespace BankAccount.Events
{
    public class AccountOpened : IEvent
    {
        public AccountOpened() { }
        public AccountOpened(Guid id, decimal amount)
        {
            AccountId = id;
            InitialDeposit = amount;
            Timestamp = DateTime.UtcNow;
        }
        public Guid AccountId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type => nameof(AccountOpened);
        public decimal InitialDeposit { get; set; }
    }
}
