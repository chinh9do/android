using BlogPost.Repository.Entities;
using BlogPost.Repository.Models;

namespace BlogPost.Service.Services.Communication;

public class PostResponse : BaseResponse
{
    public PostModel Post { get; private set; }

    private PostResponse(bool success, string message, PostModel post) : base(success, message)
    {
        Post = post;
    }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="category">Saved category.</param>
    /// <returns>Response.</returns>
    public PostResponse(PostModel post) : this(true, string.Empty, post)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public PostResponse(string message) : this(false, message, null)
    { }
}
