using BlogPost.Repository.Entities;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;

namespace BlogPost.Repository.Repositories;

public interface IBlogRepository : IRepository<Blog>
{

}

public class BlogRepository : Repository<Blog>, IBlogRepository
{
    public override string ContainerName => nameof(Blog);

    public BlogRepository(CosmosClient client, CosmosDbSettings cosmosDbSetting) : base(client, cosmosDbSetting)
    {
    }
}
