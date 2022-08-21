using AutoMapper;
using BlogPost.Repository.Entities;
using BlogPost.Repository.Repositories;
using BlogPost.Repository.Models;
using BlogPost.Service.Services.Communication;

namespace BlogPost.Service.Services;

public interface IBlogService : IBaseService<BlogModel>
{
    Task<BlogResponseModel> GetBlogModelByIdAsync(string id);
    Task<List<PostModel>> GetAllPost();
    Task<BlogResponse> AddBlogAsync(BlogModel item);
    Task<BlogResponse> DeletePostByBlogIdAndPostId(string blogId, string postId);
    Task<List<BlogModel>> GetByUserIdAsync(string userId);
}

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IApiCall _apiCall;
    private readonly IMapper _mapper;
    private const string POST_URL = "/api/posts/";

    public BlogService(IBlogRepository blogRepository, IApiCall apiCall, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _apiCall = apiCall;
        _mapper = mapper;
    }

    public async Task<BlogResponse> AddBlogAsync(BlogModel item)
    {
        try
        {
            var blog = new Blog
            {
                Name = item.Name,
                CreateDate = DateTime.UtcNow.AddMinutes(420),
                UserId = item.UserId
            };

            await _blogRepository.AddAsync(blog);

            return new BlogResponse(_mapper.Map<BlogResponseModel>(item));
        }
        catch (Exception ex)
        {
            throw ex;
            // Do some logging stuff
            // return new BlogResponse($"An error occurred when saving the blog: {ex.Message}");
        }
    }

    public async Task DeleteAsync(string id)
    {
        // Delete Posts
        var posts = await _apiCall.GetAsync($"blogId={id}");
        if (posts?.Count > 0)
        {
            foreach (var post in posts)
            {
                await _apiCall.DeleteAsync(post.Id);
            }
        }

        // Delete Blog
        await _blogRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BlogModel>> GetAsync()
    {
        return _mapper.Map<IEnumerable<BlogModel>>(await _blogRepository.GetAsync());
    }

    public async Task<List<PostModel>> GetAllPost()
    {
        var result = await _apiCall.GetAsync("");

        return result;
    }

    public async Task<BlogModel> GetByIdAsync(string id)
    {
        return _mapper.Map<BlogModel>(await _blogRepository.FindByIdAsync(id));
    }

    public async Task<BlogResponseModel> GetBlogModelByIdAsync(string id)
    {
        var blog = await _blogRepository.FindByIdAsync(id);
        var result = _mapper.Map<BlogResponseModel>(blog);
        //if (blog != null)
        //{
        //    var posts = await _apiCall.GetAsync($"blogId={id}");
        //    if (posts?.Count > 0)
        //    {
        //        result.Posts = posts;
        //    }
        //}

        return result;
    }

    public async Task UpdateAsync(BlogModel item)
    {
        item.CreateDate = DateTime.UtcNow.AddMinutes(420);
        await _blogRepository.UpdateAsync(_mapper.Map<Blog>(item));
    }

    public async Task<BlogResponse> DeletePostByBlogIdAndPostId(string blogId, string postId)
    {
        try
        {
            var url = $"{POST_URL}{blogId}/{postId}";
            await _apiCall.DeleteAsync(url);

            return new BlogResponse("Delete successfully");
        }
        catch (Exception ex)
        {
            return new BlogResponse($"An error occurred when deleting the post: {ex.Message}");
        }

    }

    public async Task<List<BlogModel>> GetByUserIdAsync(string userId)
    {
        return _mapper.Map<List<BlogModel>>(await _blogRepository.FindByUserIdAsync(userId));
    }
}
