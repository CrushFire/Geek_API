using Core.Enums;
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

        public FilterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IQueryable<PostResponse>>> GetPostsByFilter(ParametersFilter filter)
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

            if (filter.PostFilter.CategoryIds.Any())
            {
                query = query.Where(p =>
                    p.PostCategories.Any(pc => filter.PostFilter.CategoryIds.Contains(pc.CategoryId)));
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




        }
    }
}
