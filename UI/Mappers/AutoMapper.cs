using AutoMapper;
using Core.Entities;
using Core.Models;
using Core.Models.Category;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Models.DataPage;
using Core.Models.Post;
using Core.Models.User;

namespace UI.Mappers;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.NumberOfPosts,
                opt => opt.MapFrom(src => src.Posts.Count))
            .ForMember(dest => dest.NumberOfComments,
                opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.NumberOfLikes,
                opt => opt.MapFrom(src => src.Reactions.Count))
            .ForMember(dest => dest.NumberOfCommunities,
                opt => opt.MapFrom(src => src.UserCommunities.Count));

        CreateMap<Like, UserReactionsResponse>()
            .ForMember(dest => dest.PostId,
                opt => opt.MapFrom(src => src.PostId))
            .ForMember(dest => dest.IsLike,
                opt => opt.MapFrom(src => src.IsLike));

        //CreateMap<UserUpdateRequest, User>(); Этот мап не нужен, если понадобился, то ты даун

        CreateMap<Image, ImageResponse>();

        CreateMap<PostAddRequest, Post>();
        CreateMap<Post, PostResponse>();
        CreateMap<PostWithLikes, PostResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Post.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
            .ForMember(dest => dest.CategoriesRu, opt => opt.MapFrom(src => src.CategoriesRu))
            .ForMember(dest => dest.CategoriesEng, opt => opt.MapFrom(src => src.CategoriesEng))
            .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(src => src.CommunityName))
            .ForMember(dest => dest.CommunityAvatar, opt => opt.MapFrom(src => src.CommunityAvatar))
            .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.UserAvatar))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Post.Author))
            .ForMember(dest => dest.CommunityId, opt => opt.MapFrom(src => src.Post.CommunityId))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Post.Images))
            .ForMember(dest => dest.PostImages, opt => opt.MapFrom(src => src.PostImages))
            .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(src => src.CommunityName))
            .ForMember(dest => dest.Views, opt => opt.MapFrom(src => src.Post.Views))
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.Post.CreateAt))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.CountLikes))
            .ForMember(dest => dest.Dislikes, opt => opt.MapFrom(src => src.CountDislikes))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.CountComments));


        CreateMap<Community, CommunityResponse>();
        CreateMap<CommunityAddRequest, Community>();

        CreateMap<Comment, CommentResponse>();
        CreateMap<CommentAddRequest, Comment>();

        CreateMap<Category, CategoryResponse>();

        CreateMap<DataPage, DataPageResponse>();
        CreateMap<DataPageRequest, DataPage>();

    }
}