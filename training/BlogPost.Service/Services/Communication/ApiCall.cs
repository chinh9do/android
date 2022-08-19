using BlogPost.Repository.Constain;
using BlogPost.Repository.Models;
using BlogPost.Repository.Models.SettingModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BlogPost.Service.Services.Communication;

public interface IApiCall
{
    Task<List<PostModel>> GetAsync(string url);
    PostModel GetByIdAsync(string url);
    Task PostAsync(PostModel post);
    Task DeleteAsync(string url);
}

public class ApiCall : IApiCall
{
    private readonly HttpClient _httpClient;
    private readonly PostApiSettings _postApiSettings;

    public ApiCall(PostApiSettings postApiSettings)
    {
        _postApiSettings = postApiSettings;
        var socketsHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        _httpClient = new HttpClient(socketsHandler)
        {
            BaseAddress = new Uri($"https://localhost:{_postApiSettings.PostPort}")
        };

        _httpClient.DefaultRequestHeaders.Add(AppSettingConstain.API_KEY, _postApiSettings.ApiKey);
    }

    public async Task DeleteAsync(string url)
    {
        url = $"{_postApiSettings.PostUrl}{url}";
        await _httpClient.DeleteAsync(url);
    }

    public async Task<List<PostModel>> GetAsync(string url)
    {
        url = $"{_postApiSettings.PostUrl}{url}";
        var response = await _httpClient.GetAsync(url);

        var result = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<PostModel>>(result);
    }

    public PostModel GetByIdAsync(string url)
    {
        throw new NotImplementedException();
    }

    public async Task PostAsync(PostModel post)
    {
        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        await _httpClient.PostAsync(_postApiSettings.PostUrl, httpContent);
    }
}
