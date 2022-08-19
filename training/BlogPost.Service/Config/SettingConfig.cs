using BlogPost.Repository.Constain;
using BlogPost.Repository.Models.SettingModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPost.Service.Config;

public static class SettingConfig
{
    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostApiSettings>(configuration)
        .AddSingleton(configuration.GetSection(AppSettingConstain.POST_API).Get<PostApiSettings>());

        services.Configure<JwtSettings>(configuration)
            .AddSingleton(configuration.GetSection(AppSettingConstain.JWT).Get<JwtSettings>());
    }
}
