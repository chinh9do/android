namespace BlogPost.Service.Helpers;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message) { }
}
