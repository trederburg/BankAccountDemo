using BankAccount.Events;
using BankAccount.Events.Interfaces;

namespace BankAccount.Domain.Model
{
    public class BankAccount
    {
        public Guid Id { get; private set; }
        public decimal Balance { get; private set; }

        public BankAccount(Guid id)
        {
            Id = id;
            Balance = 0;
        }

        public IEnumerable<IEvent> OpenAccount(decimal intialDeposit)
        {
            yield return new AccountOpened(Id, intialDeposit);
        }
        public IEnumerable<IEvent> Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Invalid deposit amount.");
            yield return new MoneyDeposited(Id, amount);
        }

        public IEnumerable<IEvent> Withdraw(decimal amount)
        {
            if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");
            yield return new MoneyWithdrawn(Id, amount);
        }

        public void Apply(IEvent e)
        {
            switch (e)
            {
                case AccountOpened a: Balance = a.InitialDeposit; break;
                case MoneyDeposited d: Balance += d.Amount; break;
                case MoneyWithdrawn w: Balance -= w.Amount; break;
            }
        }
    }
}
