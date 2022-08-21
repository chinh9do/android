using BlogPost.Repository.Models;
using BlogPost.Service.Services;
using BlogPost.Service.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;
//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly IApiCall _apiCall;

    public BlogsController(IBlogService blogService, IApiCall apiCall)
    {
        _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        _apiCall = apiCall ?? throw new ArgumentNullException(nameof(apiCall));
    }

    #region Blog APIs
    // GET api/blogs
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await _blogService.GetAsync());
    }

    // GET /api/blogs/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var response = await _blogService.GetBlogModelByIdAsync(id);

        return Ok(response);
    }

    [HttpGet("userId={userId}")]
    public async Task<IActionResult> GetByBlogId(string userId)
    {
        return Ok(await _blogService.GetByUserIdAsync(userId));
    }

    // POST api/items
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogModel item)
    {
        var result = await _blogService.AddBlogAsync(item);
        return CreatedAtAction(nameof(Create), result);
    }

    // PUT api/items/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] BlogModel item)
    {
        item.Id = id;
        await _blogService.UpdateAsync(item);

        return Ok();
    }

    // DELETE api/items/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _blogService.DeleteAsync(id);

        return Ok();
    }

    // DELETE: /api/blogs/{blogid}/posts/{postId}
    [HttpDelete("{blogId}/posts/{postId}")]
    public async Task<IActionResult> DeletePostByBlogIdAndPostId(string blogId, string postId)
    {
        await _blogService.DeletePostByBlogIdAndPostId(blogId, postId);

        return Ok();
    }

    #endregion

    #region Call Post APIs
    [HttpGet("posts")]
    public async Task<IActionResult> GetAllPosts()
    {
        var result = await _blogService.GetAllPost();

        return Ok(result);
    }

    [HttpPost("posts/{blogId}")]
    public async Task<IActionResult> AddPost(string blogId, [FromBody] PostModel item)
    {
        item.BlogId = blogId;
        await _apiCall.PostAsync(item);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    #endregion
}
