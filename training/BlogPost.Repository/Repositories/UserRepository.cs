using BlogPost.Repository.Entities;
using BlogPost.Repository.Models.SettingModels;

namespace BlogPost.Repository.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User> FindByUserName(string userName);
    Task<User> Get(string userName, string password);
    Task<List<User>> GetAll();
}

public class UserRepository : Repository<User>, IUserRepository
{
    public override string ContainerName => nameof(User);

    public UserRepository(Microsoft.Azure.Cosmos.CosmosClient client, CosmosDbSettings cosmosDbSetting)
        : base(client, cosmosDbSetting)
    {
    }

    public async Task<User> Get(string userName, string password)
    {
        var queryString = $"SELECT * FROM c WHERE c.UserName = '{userName}' AND c.Password = '{password}'";
        var query = _container.GetItemQueryIterator<User>(new Microsoft.Azure.Cosmos.QueryDefinition(queryString));
        var result = await query.ReadNextAsync();
                
        return result.FirstOrDefault();
    }

    public async Task<User> FindByUserName(string userName)
    {
        var queryString = $"SELECT * FROM c WHERE c.UserName = '{userName}'";
        var query = _container.GetItemQueryIterator<User>(new Microsoft.Azure.Cosmos.QueryDefinition(queryString));
        var result = await query.ReadNextAsync();

        return result.FirstOrDefault();
    }

    public async Task<List<User>> GetAll()
    {
        var queryString = "SELECT * FROM c";
        var query = _container.GetItemQueryIterator<User>(new Microsoft.Azure.Cosmos.QueryDefinition(queryString));
        var result = await query.ReadNextAsync();

        return result.ToList();
    }
}
