using BlogPost.Repository.Entities;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;

namespace BlogPost.Repository.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<List<Post>> FindByBlogIdAsync(string? blogId);
    Task<Post> FindByBlogIdAndPostIdAsync(string? blogId, string? postId);
    Task DeleteByBlogIdAsync(string? blogId);
}

public class PostRepository : Repository<Post>, IPostRepository
{
    public override string ContainerName => nameof(Post);

    public PostRepository(CosmosClient client, CosmosDbSettings cosmosDbSetting) : base(client, cosmosDbSetting)
    {
    }

    public async Task<List<Post>> FindByBlogIdAsync(string? blogId)
    {
        blogId = blogId is null || blogId == "null" ? blogId : $"'{blogId}'";
        var queryString = $"SELECT * FROM c WHERE c.BlogId = {blogId}";
        var query = _container.GetItemQueryIterator<Post>(new QueryDefinition(queryString));
        var results = new List<Post>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public Task DeleteByBlogIdAsync(string blogId)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> FindByBlogIdAndPostIdAsync(string? blogId, string? postId)
    {
        blogId = blogId is null || blogId == "null" ? blogId : $"'{blogId}'";
        postId = postId is null || postId == "null" ? postId : $"'{postId}'";

        var queryString = $"SELECT * FROM c WHERE c.BlogId = {blogId} AND c.id = {postId}";
        var query = _container.GetItemQueryIterator<Post>(new QueryDefinition(queryString));
        var result = await query.ReadNextAsync();

        return result.FirstOrDefault();
    }
}
