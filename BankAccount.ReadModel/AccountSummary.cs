namespace BankAccount.ReadModel
{
    public class AccountSummary
    {
        public Guid AccountId { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
