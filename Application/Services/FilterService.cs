using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Community;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Models.User;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FilterService : IFilterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FilterService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ServiceResult<List<PostResponse>>> GetPostsByFilter(ParametersFilter filter)
        {
            // Начинаем запрос с нужными Include
            var query = _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Community)
                .Include(p => p.Reactions)
                .IncludePostImages()
                .Select(p => new
                {
                    CommunityName = p.Community.Name,
                    CommunityAvatar = _context.Images.Where(i => i.EntityTarget == "Community" && i.EntityId == p.CommunityId && i.ImageType == "avatar").FirstOrDefault(),
                    UserAvatar = _context.Images.Where(i => i.EntityTarget == "User" && i.EntityId == p.AuthorId && i.ImageType == "avatar").FirstOrDefault(),
                    Post = p,
                    LikeCount = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                    DislikeCount = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike),
                    CommentsCount = _context.Comments.Count(c => c.PostId == p.Id),
                }).AsQueryable();

            // Безопасная проверка filter.Name
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var parts = filter.Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(part => $"%{part}%")  // подготовить шаблон с % здесь
                    .ToList();

                query = query.Where(p => parts.Any(part => EF.Functions.Like(p.Post.Title, part)));
            }


            // Фильтрация по минимальному количеству просмотров
            if (filter.PostFilter?.MinViews > 0)
            {
                query = query.Where(p => p.Post.Views >= filter.PostFilter.MinViews);
            }

            // Фильтрация по категориям с проверкой на null
            //if (!string.IsNullOrEmpty(filter.PostFilter?.Categories))
            //{
            //    query = query.Where(p => !string.IsNullOrEmpty(p.Post) && p.Post.Categories.Contains(filter.PostFilter.Categories));
            //}

            // Фильтр по сообществу
            if (filter.PostFilter?.CommunityId != null)
            {
                query = query.Where(p => p.Post.CommunityId == filter.PostFilter.CommunityId);
            }

            // Фильтр по дате создания
            if (filter.DateCreateAt != DateCreateRange.None)
            {
                var dateThreshold = filter.DateCreateAt switch
                {
                    DateCreateRange.Today => DateTime.UtcNow.Date,
                    DateCreateRange.Yesterday => DateTime.UtcNow.Date.AddDays(-1),
                    DateCreateRange.Week => DateTime.UtcNow.AddDays(-7),
                    DateCreateRange.Month => DateTime.UtcNow.AddMonths(-1),
                    DateCreateRange.ThreeMonth => DateTime.UtcNow.AddMonths(-3),
                    DateCreateRange.SixMonth => DateTime.UtcNow.AddMonths(-6),
                    DateCreateRange.Year => DateTime.UtcNow.AddYears(-1),
                    DateCreateRange.MoreYear => DateTime.UtcNow.AddYears(-5),
                    _ => DateTime.MinValue
                };

                query = query.Where(p => p.Post.CreateAt >= dateThreshold);
            }

            // Сортировка с учётом направления (дефолт — ascending = true)
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool ascending = string.Equals(filter.DirectionSort, "ask", StringComparison.OrdinalIgnoreCase);

                query = filter.SortBy.ToLower() switch
                {
                    "views" => ascending ? query.OrderBy(p => p.Post.Views) : query.OrderByDescending(p => p.Post.Views),
                    "title" => ascending ? query.OrderBy(p => p.Post.Title) : query.OrderByDescending(p => p.Post.Title),
                    "likes" => ascending ? query.OrderBy(p => p.LikeCount) : query.OrderByDescending(p => p.LikeCount),
                    "created" => ascending ? query.OrderBy(p => p.Post.CreateAt) : query.OrderByDescending(p => p.Post.CreateAt),
                    _ => ascending ? query.OrderBy(p => p.Post.Views) : query.OrderByDescending(p => p.Post.Views)
                };
            }

            // Пагинация с защитой от нулей и отрицательных
            int page = filter.Pagination?.Page > 0 ? filter.Pagination.Page : 1;
            int pageSize = filter.Pagination?.PageSize > 0 ? filter.Pagination.PageSize : 10;
            int skip = (page - 1) * pageSize;

            query = query.Skip(skip).Take(pageSize);

            var postDtos = await query
                .ToListAsync();  // Сначала выгружаем все данные из БД

            var posts = postDtos.Select(p => new PostWithLikes()
            {
                Post = p.Post,
                CountLikes = p.LikeCount,
                CountDislikes = p.DislikeCount,
                CategoriesRu = p.Post.PostCategories
                    .Select(pc => pc.Category.Title)
                    .ToList(),
                CategoriesEng = p.Post.PostCategories
                    .Select(pc => pc.Category.EngTitle)
                    .ToList(),
                CommunityAvatar = p.CommunityAvatar?.ImageUrl,
                UserAvatar = p.UserAvatar?.ImageUrl,
                CountComments = p.CommentsCount,
                PostImages = _context.Images.Where(i => i.EntityTarget == "Post" && i.EntityId == p.Post.Id && i.ImageType == "image").Select(p => p.ImageUrl).ToList()
            });

            var mappingPost = _mapper.Map<List<PostResponse>>(posts);

            return ServiceResult<List<PostResponse>>.Success(mappingPost);
        }

        public async Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesByFilter(ParametersFilter filter)
        {

            var query = _context.Communities
                .Include(c => c.UserCommunities)
                .Include(c => c.CommunityCategories)
                .OrderByDescending(p => p.CreateAt)
                .Select(c => new
                {
                    Community = c,
                    CategoriesRu = c.CommunityCategories
                        .Select(pc => pc.Category.Title)
                        .ToList(),
                    CategoriesEng = c.CommunityCategories
                        .Select(pc => pc.Category.EngTitle)
                        .ToList(),
                    Avatar = _context.Images.FirstOrDefault(i => i.EntityTarget == "Community" && i.ImageType == "avatar" && i.EntityId == c.Id),
                    Author = c.UserCommunities.FirstOrDefault(u => u.CommunityId == c.Id && u.UserRole == "creator")
                }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var parts = filter.Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(part => $"%{part}%")  // подготовить шаблон с % здесь
                    .ToList();

                query = query.Where(p => parts.Any(part => EF.Functions.Like(p.Community.Name, part)));
            }

            if (filter.CommunityFilter.MinFollowers > 0)
            {
                query = query.Where(c => c.Community.UserCommunities.Count() >= filter.CommunityFilter.MinFollowers);
            }

            if (filter.DateCreateAt != DateCreateRange.None)
            {
                var dateThreshold = filter.DateCreateAt switch
                {
                    DateCreateRange.Today => DateTime.UtcNow.Date,
                    DateCreateRange.Yesterday => DateTime.UtcNow.Date.AddDays(-1),
                    DateCreateRange.Week => DateTime.UtcNow.AddDays(-7),
                    DateCreateRange.Month => DateTime.UtcNow.AddMonths(-1),
                    DateCreateRange.ThreeMonth => DateTime.UtcNow.AddMonths(-3),
                    DateCreateRange.SixMonth => DateTime.UtcNow.AddMonths(-6),
                    DateCreateRange.Year => DateTime.UtcNow.AddYears(-1),
                    DateCreateRange.MoreYear => DateTime.UtcNow.AddYears(-5),
                    _ => DateTime.MinValue
                };

                query = query.Where(p => p.Community.CreateAt >= dateThreshold);
            }

            var skip = (filter.Pagination.Page - 1) * filter.Pagination.PageSize;
            query = query.Skip(skip).Take(filter.Pagination.PageSize);

            var communitiesResponse = query.Select(s => new CommunityResponse()
            {
                Id = s.Community.Id,
                CommunityName = s.Community.Name,
                Description = s.Community.Description,
                AvatarUrl = s.Avatar.ImageUrl,
                CategoriesRu = s.CategoriesRu,
                CategoriesEng = s.CategoriesEng,
                NumberOfMember = s.Community.UserCommunities.Count() - 1,
                Author = _mapper.Map<UserResponse>(_context.Users.FirstOrDefault(u => u.Id == s.Author.UserId)),
                CreateAt = s.Community.CreateAt
            });

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool ascending = filter.DirectionSort?.ToLower() == "ask";

                communitiesResponse = filter.SortBy.ToLower() switch
                {
                    "subscribers" => ascending ? communitiesResponse.OrderBy(p => p.NumberOfMember) : communitiesResponse.OrderByDescending(p => p.NumberOfMember),
                    "title" => ascending ? communitiesResponse.OrderBy(p => p.CommunityName) : communitiesResponse.OrderByDescending(p => p.CommunityName),
                    "created" => ascending ? communitiesResponse.OrderBy(p => p.CreateAt) : communitiesResponse.OrderByDescending(p => p.CreateAt),
                    _ => ascending ? communitiesResponse.OrderBy(p => p.NumberOfMember) : communitiesResponse.OrderByDescending(p => p.NumberOfMember)
                };
            }

            var result = await communitiesResponse.ToListAsync();

            return ServiceResult<List<CommunityResponse>>.Success(result);
        }

        public async Task<ServiceResult<List<UserSearchResponse>>> GetUsersByFilter(ParametersFilter filter)
        {
            var query = _context.Users.AsQueryable();


            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var parts = filter.Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(part => $"%{part}%")  // подготовить шаблон с % здесь
                    .ToList();

                query = query.Where(p => parts.Any(part => EF.Functions.Like(p.UserName, part)));
            }

            if (filter.UserFilter.MinLikes > 0)
            {
                query = query.Where(u => u.Reactions.Count(r => r.IsLike == true) >= filter.UserFilter.MinLikes);
            }

            if (filter.UserFilter.MinPublishPosts > 0)
            {
                query = query.Where(u => u.Posts.Count() >= filter.UserFilter.MinPublishPosts);
            }

            if (filter.UserFilter.MinWrittenComments > 0)
            {
                query = query.Where(u => u.Comments.Count() >= filter.UserFilter.MinWrittenComments);
            }

            if (filter.DateCreateAt != DateCreateRange.None)
            {
                var dateThreshold = filter.DateCreateAt switch
                {
                    DateCreateRange.Today => DateTime.UtcNow.Date,
                    DateCreateRange.Yesterday => DateTime.UtcNow.Date.AddDays(-1),
                    DateCreateRange.Week => DateTime.UtcNow.AddDays(-7),
                    DateCreateRange.Month => DateTime.UtcNow.AddMonths(-1),
                    DateCreateRange.ThreeMonth => DateTime.UtcNow.AddMonths(-3),
                    DateCreateRange.SixMonth => DateTime.UtcNow.AddMonths(-6),
                    DateCreateRange.Year => DateTime.UtcNow.AddYears(-1),
                    DateCreateRange.MoreYear => DateTime.UtcNow.AddYears(-5),
                    _ => DateTime.MinValue
                };

                query = query.Where(p => p.CreateAt >= dateThreshold);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool ascending = filter.DirectionSort?.ToLower() == "ask";

                query = filter.SortBy.ToLower() switch
                {
                    "names" => ascending ? query.OrderBy(p => p.UserName) : query.OrderByDescending(p => p.UserName),
                    "created" => ascending ? query.OrderBy(p => p.CreateAt) : query.OrderByDescending(p => p.CreateAt),
                    _ => ascending ? query.OrderBy(p => p.UserCommunities.Count()) : query.OrderByDescending(p => p.UserCommunities.Count())
                };
            }

            var skip = (filter.Pagination.Page - 1) * filter.Pagination.PageSize;
            query = query.Skip(skip).Take(filter.Pagination.PageSize);

            var users = query.Select(u => new UserSearchResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                Description = u.Description,
                Avatar = _context.Images
                    .Where(i => i.EntityTarget == "User" && i.ImageType == "avatar" && i.EntityId == u.Id)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
            });

            var result = await users.ToListAsync();

            return ServiceResult<List<UserSearchResponse>>.Success(result);
        }
    }
}
