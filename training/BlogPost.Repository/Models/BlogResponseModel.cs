namespace BlogPost.Repository.Models;

public class BlogResponseModel : BlogModel
{
    public List<PostModel>? Posts { get; set; }
}
