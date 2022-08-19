namespace BlogPost.Repository.Models;

public class UserModel
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateTime Expiration { get; set; }
}
