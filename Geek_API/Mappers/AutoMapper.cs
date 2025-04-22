using AutoMapper;
using Core.Entities;
using Core.Models;
using Core.Models.Category;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Models.Post;

namespace Geek_API.Mappers;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.NumberOfPosts,
                opt => opt.MapFrom(src => src.Posts.Count))
            .ForMember(dest => dest.NumberOfComments,
                opt => opt.MapFrom(src => src.Comments.Count));

        //CreateMap<UserUpdateRequest, User>(); Этот мап не нужен, если понадобился, то ты даун

        CreateMap<Image, ImageResponse>();

        CreateMap<PostAddRequest, Post>();
        CreateMap<Post, PostResponse>();
        CreateMap<PostWithLikes, PostResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Post.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Post.Author))
            .ForMember(dest => dest.CommunityId, opt => opt.MapFrom(src => src.Post.CommunityId))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Post.Images))
            .ForMember(dest => dest.Views, opt => opt.MapFrom(src => src.Post.Views))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.Post.CreateAt))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.CountLikes))
            .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.CountDislikes));


        CreateMap<Community, CommunityResponse>();
        //CreateMap<CommunityAddRequest, Community>() Этот мап не нужен, если понадобился, то ты даун

        CreateMap<Comment, CommentResponse>();
        //CreateMap<CommentAddRequest, Comment>(); Этот мап не нужен, если понадобился, то ты даун

        CreateMap<Category, CategoryResponse>();
    }
}