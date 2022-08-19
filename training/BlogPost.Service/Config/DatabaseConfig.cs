using BlogPost.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPost.Service.Config;

public static class DatabaseConfig
{
    public static void AddBlogRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void AddPostApiRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostRepository>();
    }
}
