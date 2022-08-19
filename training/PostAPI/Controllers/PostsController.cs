using BlogPost.Repository.Models;
using BlogPost.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace PostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService blogService)
    {
        _postService = blogService ?? throw new ArgumentNullException(nameof(blogService));
    }

    // GET api/items
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await _postService.GetAsync());
    }

    // GET api/items/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        return Ok(await _postService.GetByIdAsync(id));
    }

    // GET api/items/5
    [HttpGet("blogId={blogId}")]
    public async Task<IActionResult> GetByBlogId(string blogId)
    {
        return Ok(await _postService.GetByBlogIdAsync(blogId));
    }

    // POST api/items
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostModel item)
    {
        var response = await _postService.AddPostAsync(item);

        return Ok(response);
    }

    // PUT api/items/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] PostModel item)
    {
        item.Id = id;
        await _postService.UpdateAsync(item);

        return Ok();
    }

    // DELETE api/items/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _postService.DeleteAsync(id);

        return Ok();
    }

    // DELETE api/items/5
    [HttpDelete("blogId={id}")]
    public async Task<IActionResult> DeleteByBlogId(string id)
    {
        await _postService.DeleteAsync(id);

        return Ok();
    }

    // DELETE api/items/5
    [HttpDelete("{blogId}/{postId}")]
    public async Task<IActionResult> DeletePostByBlogIdAndPostId(string blogId, string postId)
    {
        await _postService.DeletePostByBlogIdAndPostId(blogId, postId);

        return Ok();
    }
}
