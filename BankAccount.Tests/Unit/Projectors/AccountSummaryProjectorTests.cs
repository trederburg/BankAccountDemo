using BankAccount.Events;
using BankAccount.Events.Interfaces;
using BankAccount.Infrastructure;
using BankAccount.ReadModel;
using BankAccount.Tests.Unit.Helpers;
using NUnit.Framework;

namespace BankAccount.Tests.Unit.Projectors
{
    [TestFixture]
    public class AccountSummaryProjectorTests
    {
        [Test]
        public async Task Handle_AccountOpened_AddsNewSummary()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = TestDbContextFactory.Create(dbName);
            var projector = new AccountSummaryProjector(context);

            var openedEvent = new AccountOpened()
            {
                AccountId = Guid.NewGuid(),
                InitialDeposit = 100m,
                Timestamp = DateTime.UtcNow
            };


            // Act
            await projector.Handle(openedEvent);

            // Assert
            var summary = await context.AccountSummaries.FindAsync(openedEvent.AccountId);
            Assert.That(summary, Is.Not.Null);
            Assert.That(100m == summary.Balance);
            Assert.That(openedEvent.Timestamp == summary.LastUpdated);
        }

        [Test]
        public async Task Handle_MoneyWithdrawn_UpdatesBalance()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = TestDbContextFactory.Create(dbName);
            var accountId = Guid.NewGuid();
            context.AccountSummaries.Add(new AccountSummary
            {
                AccountId = accountId,
                Balance = 200m,
                LastUpdated = DateTime.UtcNow.AddMinutes(-20)
            });
            await context.SaveChangesAsync();

            var projector = new AccountSummaryProjector(context);
            var withdrawEvent = new MoneyWithdrawn()
            {
                AccountId = accountId,
                Amount = 50m,
                Timestamp = DateTime.UtcNow
            };

            // Act
            await projector.Handle(withdrawEvent);

            // Assert
            var summary = await context.AccountSummaries.FindAsync(accountId);
            Assert.That(150m == summary.Balance);
            Assert.That(withdrawEvent.Timestamp == summary.LastUpdated);
        }

        [Test]
        public async Task Handle_UnknownEvent_ThrowsNotSupported()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = TestDbContextFactory.Create(dbName);
            var projector = new AccountSummaryProjector(context);

            var fakeEvent = new FakeEvent();

            // Act & Assert
            Assert.ThrowsAsync<NotSupportedException>(async () => await projector.Handle(fakeEvent));
        }

        private class FakeEvent : IEvent
        {
            public Guid AccountId => Guid.Empty;
            public DateTime Timestamp => DateTime.UtcNow;
            public string Type => nameof(FakeEvent);
        }
    }
}