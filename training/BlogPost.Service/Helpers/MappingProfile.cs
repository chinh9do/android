using AutoMapper;
using BlogPost.Repository.Entities;
using BlogPost.Repository.Models;

namespace BlogPost.Service.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Blog, BlogModel>().ReverseMap();

        CreateMap<BlogResponseModel, Blog>().ReverseMap();
        
        CreateMap<BlogResponseModel, BlogModel>().ReverseMap();

        CreateMap<Post, PostModel>().ReverseMap();
    }
}
