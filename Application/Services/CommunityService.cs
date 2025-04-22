using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Models.Category;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommunityService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CommunityResponse>> GetByIdAsync(long id)
        {
            var community = await _context.Communities.Where(c => c.Id == id).IncludeCommunityImages().FirstOrDefaultAsync(c => c.Id == id);
            if (community == null)
            {
                return ServiceResult<CommunityResponse>.Failure("Сообщество не найдено");
            }

            var communityResponse = _mapper.Map<CommunityResponse>(community);
            return ServiceResult<CommunityResponse>.Success(communityResponse);
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

        public async Task<ServiceResult<List<CommunityResponse>>> GetByUserIdAsync(long userId, int page = 1, int pageSize = 10)
        {
            var communities = await _context.Communities
                .Include(c => c.UserCommunities)
                .Where(x => x.UserCommunities.Any(u => u.UserId == userId))
                .IncludeCommunityImages()
                .OrderByDescending(p => p.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var communitiesResponse = _mapper.Map<List<CommunityResponse>>(communities);
            return ServiceResult<List<CommunityResponse>>.Success(communitiesResponse);
        }

        public async Task<ServiceResult<CommunityResponse>> AddCommunityAsync(CommunityAddRequest communityAddRequest)
        {
            var community = _mapper.Map<Community>(communityAddRequest);

            await _context.Communities.AddAsync(community);
            var communityResponse = _mapper.Map<CommunityResponse>(community);

            return ServiceResult<CommunityResponse>.Success(communityResponse);
        }

        public async Task<ServiceResult<bool>> SubsribeAsync(long userId, long communityId)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userExists == null)
            {
                return ServiceResult<bool>.Failure("Юзер не существует");
            }
            var communityExists = await _context.Users.FirstOrDefaultAsync(x => x.Id == communityId);
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
            if (userCommunity.UserRole == "admin")
            {
                return ServiceResult<bool>.Failure("Админ не может отписаться от сообщества");
            }

            _context.UsersCommunities.Remove(userCommunity);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);

        }

        public async Task<ServiceResult<bool>> UpdateCommunityAsync(CommunityAddRequest communityAddRequest, long id)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(x => x.Id == id);
            if (community == null)
            {
                return ServiceResult<bool>.Failure("Такого сообщества не существует");
            }
            community.Name = communityAddRequest.Name;
            community.Description = communityAddRequest.Description;

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

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}
