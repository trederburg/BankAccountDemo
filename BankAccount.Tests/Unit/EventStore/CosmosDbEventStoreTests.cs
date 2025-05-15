using BankAccount.Events; // adjust namespace for your domain events
using BankAccount.Events.Interfaces;
using BankAccount.Infrastructure;
using Microsoft.Azure.Cosmos;
using NUnit.Framework;

namespace BankAccount.Tests
{
    [TestFixture]
    public class CosmosDbEventStoreTests
    {
        private CosmosClient _client;
        private CosmosDbEventStore _store;
        private const string DatabaseId = "TestEventStoreDb";
        private const string ContainerId = "TestBankAccountEvents";

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to Cosmos DB Emulator
            var endpoint = Environment.GetEnvironmentVariable("COSMOS_EMULATOR_ENDPOINT") ?? "https://localhost:8081";
            var key = Environment.GetEnvironmentVariable("COSMOS_EMULATOR_KEY") ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="; // default emulator key
            _client = new CosmosClient(endpoint, key);

            // Ensure database and container exist
            var dbResponse = await _client.CreateDatabaseIfNotExistsAsync(DatabaseId);
            await dbResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(ContainerId, "/aggregateId"));

            _store = new CosmosDbEventStore(_client);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Clean up test database
            await _client.GetDatabase(DatabaseId).DeleteAsync();
            _client.Dispose();
        }

        [Test]
        public async Task AppendEventsAsync_ShouldStoreAndLoadEvents()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var eventsToAppend = new List<IEvent>
            {
                new AccountOpened { AccountId = accountId, Timestamp = now, InitialDeposit = 100m },
                new MoneyDeposited { AccountId = accountId, Timestamp = now.AddSeconds(1), Amount = 50m }
            };

            // Act
            await _store.AppendEventsAsync(accountId, eventsToAppend);
            var loadedEvents = (await _store.LoadEventsAsync(accountId)).ToList();

            // Assert

            Assert.That(2 == loadedEvents.Count, "Should load two events");
            Assert.That(loadedEvents, Is.Ordered.By(nameof(IEvent.Timestamp)), "Events should be ordered by timestamp");

            // Check event types
            Assert.That(loadedEvents[0].Type, Is.EqualTo(nameof(AccountOpened)), "First event should be AccountOpened");
            Assert.That(loadedEvents[1].Type, Is.EqualTo(nameof(MoneyDeposited)));

            var created = (AccountOpened)loadedEvents[0];
            var deposited = (MoneyDeposited)loadedEvents[1];

            Assert.That(100m == created.InitialDeposit, "Initial balance should match");
            Assert.That(50m == deposited.Amount, "Deposited amount should match");
        }

        [Test]
        public async Task LoadEventsAsync_NoEvents_ReturnsEmpty()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            // Act
            var loadedEvents = await _store.LoadEventsAsync(accountId);

            // Assert
            Assert.That(loadedEvents.Count() == 0, "No events should return an empty collection");
        }
    }
}
