using BankAccount.Events.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace BankAccount.Infrastructure
{
    public class CosmosDbEventStore : IEventStore
    {
        private readonly CosmosClient _client;

        public CosmosDbEventStore(CosmosClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<IEvent>> LoadEventsAsync(Guid aggregateId)
        {
            // query Cosmos
            var container = _client.GetContainer("EventStoreDb", "BankAccountEvents");

            var query = new QueryDefinition("SELECT * FROM c WHERE c.aggregateId = @aggregateId ORDER BY c.timestamp ASC")
                            .WithParameter("@aggregateId", aggregateId);

            var resultSet = container.GetItemQueryIterator<EventDocument>(query);

            var events = new List<IEvent>();

            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();

                foreach (var doc in response)
                {
                    var eventType = Type.GetType($"BankAccount.Events.{doc.Type}, BankAccount.Events");

                    if (eventType == null)
                        throw new InvalidOperationException($"Unknown event type {doc.Type}");

                    var eventDataJson = doc.EventData.ToString();
                    var @event = (IEvent)JsonSerializer.Deserialize(eventDataJson, eventType);

                    events.Add(@event);
                }
            }

            return events;
        }

        public async Task AppendEventsAsync(Guid aggregateId, IEnumerable<IEvent> events)
        {
            // write to Cosmos
            var container = _client.GetContainer("EventStoreDb", "BankAccountEvents");

            foreach (var e in events)
            {
                var document = new EventDocument
                {
                    id = Guid.NewGuid().ToString(),           // Cosmos needs an 'id'
                    aggregateId = e.AccountId.ToString(),    // Partition key
                    AccountId = e.AccountId.ToString(),
                    Timestamp = e.Timestamp,
                    Type = e.Type,
                    EventData = e                             // Store the raw event
                };

                await container.CreateItemAsync(document, new PartitionKey(document.aggregateId));
            }
        }
    }
}
