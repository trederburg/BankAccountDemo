using Microsoft.Azure.Cosmos;

namespace BankAccount.Infrastructure
{
    public class CosmosDbInitializer
    {
        private readonly CosmosClient _client;

        public CosmosDbInitializer(CosmosClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            var db = await _client.CreateDatabaseIfNotExistsAsync("EventStoreDb");
            await db.Database.CreateContainerIfNotExistsAsync("BankAccountEvents", "/aggregateId");
        }
    }
}
