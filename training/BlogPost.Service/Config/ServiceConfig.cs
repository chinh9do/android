using BlogPost.Service.Services;
using BlogPost.Service.Services.Communication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogPost.Service.Config;

public static class ServiceConfig
{
    public static void AddBlogApiServices(this IServiceCollection services)
    {
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IApiCall, ApiCall>();
        services.AddScoped<IUserService, UserService>();
    }

    public static void AddBlogAuthentication(this IServiceCollection services, IConfiguration configuration)  
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidIssuer = configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
            };
        });
    }

    public static void AddPostApiServices(this IServiceCollection services)
    {
        services.AddScoped<IPostService, PostService>();
    }
}
