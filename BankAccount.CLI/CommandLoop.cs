using BankAccount.Commands;
using BankAccount.Infrastructure;

namespace BankAccount.CLI
{
    public class CommandLoop
    {
        private readonly DepositMoneyHandler _depositHandler;
        private readonly OpenAccountHandler _openHandler;
        private readonly ReadDbContext _readDb;

        public CommandLoop(
            DepositMoneyHandler depositHandler,
            OpenAccountHandler openHandler,
            ReadDbContext readDb)
        {
            _openHandler = openHandler;
            _depositHandler = depositHandler;
            _readDb = readDb;
        }

        public async Task Run()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "deposit":
                        await HandleDeposit();
                        break;
                    case "balance":
                        await HandleBalance();
                        break;
                    case "open":
                        await HandleOpen();
                        break;

                    case "exit":
                    case "quit":
                        return;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }

        private async Task HandleDeposit()
        {
            Console.Write("Account ID: ");
            var accountId = Guid.Parse(Console.ReadLine());

            Console.Write("Amount: ");
            var amount = decimal.Parse(Console.ReadLine());

            await _depositHandler.Handle(new DepositMoneyCommand
            {
                AccountId = accountId,
                Amount = amount
            });

            Console.WriteLine("Deposit complete.");
        }

        private async Task HandleBalance()
        {
            Console.Write("Account ID: ");
            var accountId = Guid.Parse(Console.ReadLine());

            var summary = await _readDb.AccountSummaries.FindAsync(accountId);

            if (summary != null)
            {
                Console.WriteLine($"Balance: ${summary.Balance}");
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }

        private async Task HandleOpen()
        {
            Console.Write("Account ID: ");
            var accountId = Guid.Parse(Console.ReadLine());
            Console.Write("Initial Deposit: ");
            var initialDeposit = decimal.Parse(Console.ReadLine());
            var command = new OpenAccountCommand
            {
                AccountId = accountId,
                InitialDeposit = initialDeposit
            };
            await _openHandler.Handle(command);
            Console.WriteLine("Account opened.");
        }
    }
}
