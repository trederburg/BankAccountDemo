using BankAccount.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Tests.Unit.Helpers
{
    public static class TestDbContextFactory
    {
        public static ReadDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<ReadDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ReadDbContext(options);
        }
    }
}
