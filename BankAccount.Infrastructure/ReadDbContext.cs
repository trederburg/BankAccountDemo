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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountSummary>()
                .HasKey(a => a.AccountId);

            modelBuilder.Entity<AccountSummary>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);
        }

    }
}
