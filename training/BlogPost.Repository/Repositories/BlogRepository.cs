using BlogPost.Repository.Entities;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;

namespace BlogPost.Repository.Repositories;

public interface IBlogRepository : IRepository<Blog>
{
    Task<List<Blog>> FindByUserIdAsync(string? userId);
}

public class BlogRepository : Repository<Blog>, IBlogRepository
{
    public override string ContainerName => nameof(Blog);

    public BlogRepository(CosmosClient client, CosmosDbSettings cosmosDbSetting) : base(client, cosmosDbSetting)
    {
    }

    public async Task<List<Blog>> FindByUserIdAsync(string? userId)
    {
        userId = userId is null || userId == "null" ? userId : $"'{userId}'";
        var queryString = $"SELECT * FROM c WHERE c.UserId = {userId}";
        var query = _container.GetItemQueryIterator<Blog>(new QueryDefinition(queryString));
        var results = new List<Blog>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }
}
