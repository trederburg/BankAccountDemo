using BankAccount.CLI;
using BankAccount.Commands;
using BankAccount.Events.Interfaces;
using BankAccount.Infrastructure;
using BankAccount.ReadModel;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Cosmos DB
        services.AddSingleton(new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
        services.AddScoped<IEventStore, CosmosDbEventStore>();

        // for setting up cosmos db the first time.
        services.AddScoped<CosmosDbInitializer>();

        // Read Model (EF Core)
        services.AddDbContext<ReadDbContext>(options =>
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ReadModelDb;Trusted_Connection=True;"));
        services.AddScoped<IAccountSummaryProjector, AccountSummaryProjector>();

        // Commands
        services.AddScoped<DepositMoneyHandler>();
        services.AddScoped<OpenAccountHandler>();

        // Optional: wire CLI command orchestration layer
        services.AddScoped<CommandLoop>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

// Initialize Cosmos DB
var cosmosInit = provider.GetRequiredService<CosmosDbInitializer>();
await cosmosInit.InitializeAsync(); // Sets up DB + container if needed

var loop = provider.GetRequiredService<CommandLoop>();
await loop.Run();