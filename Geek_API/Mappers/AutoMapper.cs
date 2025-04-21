using AutoMapper;
using Core.Models;
using Core.Models.Category;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Models.Image;
using Core.Models.Post;
using DataAccess.Entities;

namespace Geek_API.Mappers;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<UserEntity, UserResponse>();
        CreateMap<UserUpdateRequest, UserEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        CreateMap<ImageEntity, ImageResponse>();
        CreateMap<ImageAddRequest, ImageEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        CreateMap<PostEntity, PostResponse>();
        CreateMap<PostAddRequest, PostEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Community, opt => opt.MapFrom(x => x.CommunityId));

        CreateMap<CommunityEntity, CommunityResponse>();
        CreateMap<CommunityAddRequest, CommunityEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(x => x.CategoriesId));

        CreateMap<CommentEntity, CommentResponse>();
        CreateMap<CommentAddRequest, CommentEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.MapFrom(x => x.PostId));

        CreateMap<CategoryEntity, CategoryResponse>();
        CreateMap<CategoryAddRequest, CategoryEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}