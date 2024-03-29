﻿using BlogPost.Repository.Entities;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace BlogPost.Repository.Repositories;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAsync();
    Task<T> FindByIdAsync(string id);
    Task AddAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(string id);
}

public abstract class Repository<T> where T : BaseEntity
{
    protected readonly Container _container;

    public abstract string ContainerName { get; }

    public Repository(CosmosClient client, CosmosDbSettings cosmosDbSetting)
    {
        _container = client.GetContainer(cosmosDbSetting.DatabaseName, ContainerName);
    }

    public async Task AddAsync(T item)
    {
        await _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
    }

    public async Task<IEnumerable<T>> GetAsync()
    {
        var queryString = "SELECT * FROM c";
        var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<T> FindByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw ex;
        }
    }

    public async Task UpdateAsync(T item)
    {
        await _container.UpsertItemAsync(item, new PartitionKey(item.Id));
    }
}
