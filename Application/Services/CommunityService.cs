using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Category;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Models.Image;
using Core.Models.Post;
using Core.Results;
using DataAccess;
using FluentNHibernate.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CommunityService(ApplicationDbContext context, IMapper mapper, IImageService image)
        {
            _context = context;
            _mapper = mapper;
            _imageService = image;
        }

        public async Task<ServiceResult<CommunityResponse>> GetByIdAsync(long id)
        {
            var c = await _context.Communities
                .Include(c => c.UserCommunities)
                .Include(c => c.CommunityCategories)
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
                }).FirstOrDefaultAsync(c => c.Community.Id == id);

            var communitiesResponse = new CommunityResponse()
            {
                Id = c.Community.Id,
                CommunityName = c.Community.Name,
                Description = c.Community.Description,
                AvatarUrl = c.Avatar.ImageUrl,
                CategoriesRu = c.CategoriesRu,
                CategoriesEng = c.CategoriesEng,
                NumberOfMember = c.Community.UserCommunities.Count() - 1,
                Author = _mapper.Map<UserResponse>(_context.Users.FirstOrDefault(u => u.Id == c.Author.Id)),
                CreateAt = c.Community.CreateAt
            };

            return ServiceResult<CommunityResponse>.Success(communitiesResponse);
        }

        public async Task<ServiceResult<List<CommunityResponse>>> GetCommunityAsync(int page = 1, int pageSize = 10)
        {
            var communities = await _context.Communities
                .IncludeCommunityImages()
                .OrderByDescending(p => p.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var communitiesResponse = _mapper.Map<List<CommunityResponse>>(communities);
            return ServiceResult<List<CommunityResponse>>.Success(communitiesResponse);
        }

        public async Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesSubscribeUser(PaginationRequest paginationRequest, long userId)
        {
            var subscribedCommunities = await _context.Communities
                .Include(c => c.UserCommunities)
                .Where(u => u.UserCommunities.Any(u => u.UserId == userId && u.UserRole == "subscriber"))
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
                })
                .Skip((paginationRequest.Page - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .ToListAsync();

            var communitiesResponse = subscribedCommunities.Select(s => new CommunityResponse()
            {
                Id = s.Community.Id,
                CommunityName = s.Community.Name,
                Description = s.Community.Description,
                AvatarUrl = s.Avatar.ImageUrl,
                CategoriesRu = s.CategoriesRu,
                CategoriesEng = s.CategoriesEng,
                NumberOfMember = s.Community.UserCommunities.Count() - 1,
                Author = _mapper.Map<UserResponse>(_context.Users.FirstOrDefault(u => u.Id == s.Author.Id)),
                CreateAt = s.Community.CreateAt
            });

            return ServiceResult<List<CommunityResponse>>.Success(communitiesResponse.ToList());
        }

        public async Task<ServiceResult<CommunityWithCategoriesResponse>> GetByIdWithCategoriesAsync(long id)
        {
            var p = await _context.Communities
                .Include(p => p.CommunityCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            var communityImage = await _context.Images
                .FirstOrDefaultAsync(i => i.EntityTarget == "Community" && i.EntityId == id && i.ImageType == "avatar");

                var communityImageResponse = new ImageIdResponse
                {
                    Id = communityImage.Id,
                    Url = communityImage.ImageUrl
                };

            var response = new CommunityWithCategoriesResponse
            {
                Title = p.Name,
                Description = p.Description,
                CategoriesIds = p.CommunityCategories.Select(pc => pc.CategoryId).ToList(),
                CommunityImage = communityImageResponse
            };

            return ServiceResult<CommunityWithCategoriesResponse>.Success(response);
        }

        public async Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesCreatedUser(PaginationRequest paginationRequest, long userId)
        {
            var subscribedCommunities = await _context.Communities
                .Include(c => c.UserCommunities)
                .Where(u => u.UserCommunities.Any(u => u.UserId == userId && u.UserRole == "creator"))
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
                    Author = _context.Users.FirstOrDefault(u => u.Id == userId)
                })
                .Skip((paginationRequest.Page - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .ToListAsync();

            var communitiesResponse = subscribedCommunities.Select(s => new CommunityResponse()
            {
                Id = s.Community.Id,
                CommunityName = s.Community.Name,
                Description = s.Community.Description,
                AvatarUrl = s.Avatar.ImageUrl,
                CategoriesRu = s.CategoriesRu,
                CategoriesEng = s.CategoriesEng,
                NumberOfMember = s.Community.UserCommunities.Count() - 1,
                Author = _mapper.Map<UserResponse>(s.Author),
                CreateAt = s.Community.CreateAt
            });

            return ServiceResult<List<CommunityResponse>>.Success(communitiesResponse.ToList());
        }

        public async Task<ServiceResult<CommunityResponse>> AddCommunityAsync(CommunityAddRequest communityAddRequest, long authorId)
        {
            // Сначала создать сущность и сохранить, чтобы появился ID
            var community = _mapper.Map<Community>(communityAddRequest);
            await _context.Communities.AddAsync(community);
            await _context.SaveChangesAsync(); // <- после этого у community.Id есть значение!

            // Сохраняем изображение (если зависит от ID)
            var newImage = await _imageService.AddUploadedImageAsync("Community", community.Id, "avatar", communityAddRequest.Avatar);

            // Теперь можно добавлять зависимости
            var communityCategories = communityAddRequest.CategoriesIds.Select(catId => new CommunityCategory
            {
                CommunityId = community.Id,
                CategoryId = catId
            }).ToList();

            var userCommunity = new UserCommunity
            {
                CommunityId = community.Id,
                UserId = authorId,
                UserRole = "creator"
            };

            await _context.CommunityCategories.AddRangeAsync(communityCategories);
            await _context.UsersCommunities.AddAsync(userCommunity);
            await _context.SaveChangesAsync();

            var communityResponse = _mapper.Map<CommunityResponse>(community);
            return ServiceResult<CommunityResponse>.Success(communityResponse);
        }

        public async Task<ServiceResult<string>> SubOrNo(long userId, long communityId)
        {
            var isSub = _context.UsersCommunities.FirstOrDefault(uc => uc.UserId == userId && uc.CommunityId == communityId);

            if (isSub == null)
            {
                return ServiceResult<string>.Success("unSub");
            }
            else if (isSub.UserRole == "creator")
            {
                return ServiceResult<string>.Success("creator");
            }
            else
            {
                return ServiceResult<string>.Success("sub");
            }
        }

        public async Task<ServiceResult<bool>> SubsribeAsync(long userId, long communityId)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userExists == null)
            {
                return ServiceResult<bool>.Failure("Юзер не существует");
            }
            var communityExists = await _context.Communities.FirstOrDefaultAsync(x => x.Id == communityId);
            if (communityExists == null)
            {
                return ServiceResult<bool>.Failure("Сообщество не найдено");
            }

            UserCommunity userCommunity = new();
            userCommunity.UserId = userId;
            userCommunity.CommunityId = communityId;
            userCommunity.UserRole = "subscriber";

            await _context.UsersCommunities.AddAsync(userCommunity);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> UnSubscribeAsync(long userId, long communityId)
        {
            var userCommunity = await _context.UsersCommunities.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
            if (userCommunity.UserRole == "creator")
            {
                return ServiceResult<bool>.Failure("Админ не может отписаться от сообщества");
            }

            _context.UsersCommunities.Remove(userCommunity);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);

        }

        public async Task<ServiceResult<bool>> UpdateCommunityAsync(CommunityUpdateRequest request, long id)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(x => x.Id == id);

            if (community == null)
            {
                return ServiceResult<bool>.Failure("Такого сообщества не существует");
            }
            if (request.ImageToRemove != null)
            {
                await _imageService.RemoveImageFromServer(request.ImageToRemove);
            }
            if (request.NewImage != null)
            {
                await _imageService.AddUploadedImageAsync("Community", community.Id, "avatar", request.NewImage);
            }

            var communityCategories = await _context.CommunityCategories.Where(pc => pc.CommunityId == community.Id).ToListAsync();
            _context.CommunityCategories.RemoveRange(communityCategories);

            var newCommunityCategories = request.Categories
                    .Select(catId => new CommunityCategory
                    {
                        CommunityId = community.Id,
                        CategoryId = catId
                    })
                    .ToList();

            _context.CommunityCategories.AddRange(newCommunityCategories);

            community.Name = request.Title;
            community.Description = request.Description;

            _context.Communities.Update(community);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> DeleteCommunityAsync(long id)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(x => x.Id == id);
            if (community == null)
            {
                return ServiceResult<bool>.Failure("Такого сообщества не существует");
            }

            var imageIds = await _context.Images
            .Where(im => im.EntityId == community.Id && im.EntityTarget == nameof(Community))
            .Select(im => im.Id)
            .ToListAsync();

            await _imageService.RemoveImages(imageIds);

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<List<CommunityExploreResponse>>> CommunityExplore(long categoryId, PaginationRequest pagination)
        {
            var communitiesQuery = _context.Communities
                .Include(c => c.CommunityCategories)
                .Include(c => c.UserCommunities)
                .Where(c => c.CommunityCategories.Any(cc => cc.CategoryId == categoryId))
                .Select(c => new CommunityExploreResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    NumberOfMember = c.UserCommunities.Count() - 1,
                    AvatarUrl = _context.Images
                        .Where(i => i.ImageType == "avatar" && i.EntityTarget == "Community" && i.EntityId == c.Id)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .OrderByDescending(c => c.NumberOfMember);

            var communities = await communitiesQuery
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();


            return ServiceResult<List<CommunityExploreResponse>>.Success(communities);
        }
    }
}
