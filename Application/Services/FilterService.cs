using AutoMapper;
using Core.Enums;
using Core.Models;
using Core.Models.Community;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FilterService
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
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                string[] parts = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                query = query
                    .Where(p => parts.Any(part => p.Title.ToLower().Contains(part)))
                    .Select(p => new
                    {
                        Post = p,
                        MatchCount = parts.Count(part => p.Title.ToLower().Contains(part))
                    })
                    .OrderByDescending(x => x.MatchCount)
                    .ThenBy(x => x.Post.Title)
                    .Select(x => x.Post);
            }

            if(filter.PostFilter.MinViews > 0)
            {
                query = query.Where(p => p.Views >= filter.PostFilter.MinViews);
            }

            if (!string.IsNullOrEmpty(filter.PostFilter.Categories))
            {
                query = query.Where(p => p.Categories.Contains(filter.PostFilter.Categories));
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
                    "views" => ascending ? query.OrderBy(p => p.Views) : query.OrderByDescending(p => p.Views),
                    "title" => ascending ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.Title),
                    "created" => ascending ? query.OrderBy(p => p.CreateAt) : query.OrderByDescending(p => p.CreateAt),
                    _ => ascending ? query.OrderBy(p => p.Views) : query.OrderByDescending(p => p.Views)
                };
            }

            var skip = (filter.Pagination.Page - 1) * filter.Pagination.PageSize;
            query = query.Skip(skip).Take(filter.Pagination.PageSize);

            var posts = await query.ToListAsync();
            var result = _mapper.Map<List<PostResponse>>(posts);

            return ServiceResult<List<PostResponse>>.Success(result);
        }

        public async Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesByFilter(ParametersFilter filter)
        {
            var query = _context.Communities.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                string[] parts = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                query = query
                    .Where(c => parts.Any(part => c.Name.ToLower().Contains(part)))
                    .Select(c => new
                    {
                        Community = c,
                        MatchCount = parts.Count(part => c.Name.ToLower().Contains(part))
                    })
                    .OrderByDescending(x => x.MatchCount)
                    .ThenBy(x => x.Community.Name)
                    .Select(x => x.Community);
            }

            if(filter.CommunityFilter.MinFollowers > 0)
            {
                query = query.Where(c => c.UserCommunities.Count() >= filter.CommunityFilter.MinFollowers);
            }

            if(filter.DateCreateAt != DateCreateRange.None)
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

            if (!string.IsNullOrEmpty(filter.CommunityFilter.Categories))
            {
                query = query.Where(p => p.Categories.Contains(filter.CommunityFilter.Categories));
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool ascending = filter.DirectionSort?.ToLower() == "ask";

                query = filter.SortBy.ToLower() switch
                {
                    "followers" => ascending ? query.OrderBy(p => p.UserCommunities.Count()) : query.OrderByDescending(p => p.UserCommunities.Count()),
                    "title" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "created" => ascending ? query.OrderBy(p => p.CreateAt) : query.OrderByDescending(p => p.CreateAt),
                    _ => ascending ? query.OrderBy(p => p.UserCommunities.Count()) : query.OrderByDescending(p => p.UserCommunities.Count())
                };
            }

            var skip = (filter.Pagination.Page - 1) * filter.Pagination.PageSize;
            query = query.Skip(skip).Take(filter.Pagination.PageSize);

            var communities = await query.ToListAsync();
            var result = _mapper.Map<List<CommunityResponse>>(communities);

            return ServiceResult<List<CommunityResponse>>.Success(result);
        }

        public async Task<ServiceResult<List<UserResponse>>> GetUsersByFilter (ParametersFilter filter)
        {
            var query = _context.Users.Include(u => u.Reactions).AsQueryable();


            if (!string.IsNullOrEmpty(filter.Name))
            {
                string[] parts = filter.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                query = query
                    .Where(u => parts.Any(part => u.UserName.ToLower().Contains(part)))
                    .Select(u => new
                    {
                        User = u,
                        MatchCount = parts.Count(part => u.UserName.ToLower().Contains(part))
                    })
                    .OrderByDescending(x => x.MatchCount)
                    .ThenBy(x => x.User.UserName)
                    .Select(x => x.User);
            }

            if (filter.UserFilter.MinLikes > 0)
            {
                query = query.Where(u => u.Reactions.Count(r => r.IsLike == true) >= filter.UserFilter.MinLikes);
            }

            if(filter.UserFilter.MinPublishPosts > 0)
            {
                query = query.Where(u => u.Posts.Count() >= filter.UserFilter.MinPublishPosts);
            }

            if(filter.UserFilter.MinWrittenComments > 0)
            {
                query = query.Where(u => u.Comments.Count() >= filter.UserFilter.MinWrittenComments);
            }

            if(filter.DateCreateAt != DateCreateRange.None)
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
                    "likes" => ascending ? query.OrderBy(p => p.Reactions.Count(x => x.IsLike == true)) : query.OrderByDescending(p => p.Reactions.Count(x => x.IsLike == true)),
                    "comments" => ascending ? query.OrderBy(p => p.Comments.Count()) : query.OrderByDescending(p => p.Comments.Count()),
                    "posts" => ascending ? query.OrderBy(p => p.Posts.Count()) : query.OrderByDescending(p => p.Posts.Count()),
                    "created" => ascending ? query.OrderBy(p => p.CreateAt) : query.OrderByDescending(p => p.CreateAt),
                    _ => ascending ? query.OrderBy(p => p.UserCommunities.Count()) : query.OrderByDescending(p => p.UserCommunities.Count())
                };
            }

            var skip = (filter.Pagination.Page - 1) * filter.Pagination.PageSize;
            query = query.Skip(skip).Take(filter.Pagination.PageSize);

            var users = await query.ToListAsync();
            var result = _mapper.Map<List<UserResponse>>(users);

            return ServiceResult<List<UserResponse>>.Success(result);
        }
    }
}
