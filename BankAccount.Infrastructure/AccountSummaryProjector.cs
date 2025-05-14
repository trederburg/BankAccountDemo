using BankAccount.Events;
using BankAccount.Events.Interfaces;
using BankAccount.ReadModel;

namespace BankAccount.Infrastructure
{
    public class AccountSummaryProjector : IAccountSummaryProjector
    {
        private readonly ReadDbContext readDbContext;

        public AccountSummaryProjector(ReadDbContext readDbContext)
        {
            this.readDbContext = readDbContext;
        }
        /// <summary>
        /// Handles the event and updates the read model. This is optimized for getting the current balance of an account.
        /// Right now this is simple and synchronous,
        /// In production you would want to use a message queue or similar to handle events asynchronously.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task Handle(IEvent @event)
        {
            switch (@event)
            {
                case AccountOpened opened:
                    readDbContext.AccountSummaries.Add(new AccountSummary
                    {
                        AccountId = opened.AccountId,
                        Balance = opened.InitialDeposit,
                        LastUpdated = opened.Timestamp
                    });
                    break;
                case MoneyDeposited deposited:
                    var depositAccount = await readDbContext.AccountSummaries.FindAsync(deposited.AccountId);
                    ArgumentNullException.ThrowIfNull(depositAccount, nameof(depositAccount));
                    depositAccount.Balance += deposited.Amount;
                    depositAccount.LastUpdated = deposited.Timestamp;
                    break;
                case MoneyWithdrawn withdrawn:
                    var withdrawAccount = await readDbContext.AccountSummaries.FindAsync(withdrawn.AccountId);
                    ArgumentNullException.ThrowIfNull(withdrawAccount, nameof(withdrawAccount));
                    withdrawAccount.Balance -= withdrawn.Amount;
                    withdrawAccount.LastUpdated = withdrawn.Timestamp;
                    break;
                default:
                    throw new NotSupportedException($"Event type {@event.GetType().Name} is not supported.");
            }

            await readDbContext.SaveChangesAsync();
        }
    }
}
