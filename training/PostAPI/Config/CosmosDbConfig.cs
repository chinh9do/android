using BlogPost.Repository.Constain;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;

namespace PostAPI.Config;

public static class CosmosDbConfig
{
    public static void SetupCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind database-related bindings
        var cosmosDbSettings = configuration.GetSection(AppSettingConstain.COSMOSDB).Get<CosmosDbSettings>();

        services.Configure<CosmosDbSettings>(configuration).AddSingleton(cosmosDbSettings);

        services.AddSingleton(InitializeCosmosClientInstanceAsync(cosmosDbSettings).Result);
    }

    private static async Task<CosmosClient> InitializeCosmosClientInstanceAsync(CosmosDbSettings cosmosDbSettings)
    {
        var client = new CosmosClient(cosmosDbSettings.Account, cosmosDbSettings.Key);

        var database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbSettings.DatabaseName);
        foreach (var container in cosmosDbSettings.Containers)
        {
            await database.Database.CreateContainerIfNotExistsAsync(container.Name, container.PartitionKey);
        }

        return client;
    }
}