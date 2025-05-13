using BankAccount.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Infrastructure
{
    public class ReadDbContext : DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }
        public DbSet<AccountSummary> AccountSummaries { get; set; }

    }
}
