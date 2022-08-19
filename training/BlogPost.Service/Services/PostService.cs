using AutoMapper;
using BlogPost.Repository.Entities;
using BlogPost.Repository.Repositories;
using BlogPost.Repository.Models;
using BlogPost.Service.Services.Communication;

namespace BlogPost.Service.Services;

public interface IPostService : IBaseService<PostModel>
{
    Task<List<PostModel>> GetByBlogIdAsync(string blogId);
    Task<PostResponse> DeletePostByBlogIdAndPostId(string blogId, string postId);
    Task<PostResponse> AddPostAsync(PostModel item);
}

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    public PostService(IPostRepository postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostResponse> AddPostAsync(PostModel item)
    {
        try
        {
            await _postRepository.AddAsync(_mapper.Map<Post>(item));

            return new PostResponse(item);
        }
        catch (Exception ex)
        {
            // Do some logging stuff
            return new PostResponse($"An error occurred when saving the post: {ex.Message}");
        }
    }

    public async Task DeleteAsync(string id)
    {
        await _postRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PostModel>> GetAsync()
    {
        return _mapper.Map<List<PostModel>>(await _postRepository.GetAsync());
    }

    public async Task<PostModel> GetByIdAsync(string id)
    {
        return _mapper.Map<PostModel>(await _postRepository.FindByIdAsync(id));
    }

    public async Task<List<PostModel>> GetByBlogIdAsync(string blogId)
    {
        return _mapper.Map<List<PostModel>>(await _postRepository.FindByBlogIdAsync(blogId));
    }

    public async Task UpdateAsync(PostModel item)
    {
        await _postRepository.UpdateAsync(_mapper.Map<Post>(item));
    }

    public async Task<PostResponse> DeletePostByBlogIdAndPostId(string blogId, string postId)
    {
        try
        {
            var post = await _postRepository.FindByBlogIdAndPostIdAsync(blogId, postId);
            if (post != null)
            {
                await _postRepository.DeleteAsync(post.Id);
                return new PostResponse("Remove successfully.");
            }
            else
            {
                return new PostResponse("Post not found.");
            }
        }
        catch (Exception ex)
        {
            return new PostResponse($"An error occurred when deleting the post: {ex.Message}");
        }
    }
}
