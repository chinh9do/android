using BlogPost.Repository.Entities;
using BlogPost.Repository.Models;

namespace BlogPost.Service.Services.Communication;

public class BlogResponse : BaseResponse
{
    public BlogResponseModel Blog { get; private set; }

    private BlogResponse(bool success, string message, BlogResponseModel blog) : base(success, message)
    {
        Blog = blog;
    }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="category">Saved category.</param>
    /// <returns>Response.</returns>
    public BlogResponse(BlogResponseModel blog) : this(true, string.Empty, blog)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public BlogResponse(string message) : this(false, message, null)
    { }
}
