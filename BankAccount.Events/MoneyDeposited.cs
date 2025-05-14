using BankAccount.Events.Interfaces;

namespace BankAccount.Events
{
    public class MoneyDeposited : IEvent
    {
        public MoneyDeposited() { }
        public MoneyDeposited(Guid id, decimal amount)
        {
            AccountId = id;
            Amount = amount;
            Timestamp = DateTime.UtcNow;
        }
        public Guid AccountId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type => nameof(MoneyDeposited);
        public decimal Amount { get; set; }
    }
}
