using AutoMapper;
using BlogPost.Service.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPost.Service.Config;

public static class MappingConfig
{
    public static void AutoMapperConfigurations(this IServiceCollection services)
    {
        // Auto Mapper Configurations
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        IMapper mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
    }
}
