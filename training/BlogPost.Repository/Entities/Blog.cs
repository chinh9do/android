namespace BlogPost.Repository.Entities;

public class Blog : BaseEntity
{
    public string? Name { get; set; }
    public string? UserId { get; set; }
    public DateTime CreateDate { get; set; }
}
