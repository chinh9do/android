using BlogPost.Repository.Constain;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;

namespace BlogAPI.Config;
public static class CosmosDbConfig
{
    public static void SetupCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind database-related bindings
        var cosmosDbSetting = configuration.GetSection(AppSettingConstain.COSMOSDB).Get<CosmosDbSettings>();

        services.Configure<CosmosDbSettings>(configuration).AddSingleton(cosmosDbSetting);

        services.AddSingleton(InitializeCosmosClientInstanceAsync(cosmosDbSetting).Result);
    }

    private static async Task<CosmosClient> InitializeCosmosClientInstanceAsync(CosmosDbSettings cosmosDbSetting)
    {
        var client = new CosmosClient(cosmosDbSetting.Account, cosmosDbSetting.Key);

        var database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbSetting.DatabaseName);
        foreach (var container in cosmosDbSetting.Containers)
        {
            await database.Database.CreateContainerIfNotExistsAsync(container.Name, container.PartitionKey);
        }

        return client;
    }
}
