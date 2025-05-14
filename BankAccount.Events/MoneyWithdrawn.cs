using BankAccount.Events.Interfaces;

namespace BankAccount.Events
{
    public class MoneyWithdrawn : IEvent
    {
        public MoneyWithdrawn() { }
        public MoneyWithdrawn(Guid id, decimal amount)
        {
            AccountId = id;
            Amount = amount;
            Timestamp = DateTime.UtcNow;
        }
        public Guid AccountId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type => nameof(MoneyWithdrawn);
        public decimal Amount { get; set; }
    }
}
