using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models;
using Core.Results;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper autoMapper, IImageService imageService)
    {
        _context = context;
        _mapper = autoMapper;
        _imageService = imageService;
    }

    public async Task<ServiceResult<UserResponse>> GetUserByIdAsync(long id)
    {
        var user = await _context.Users
            .Where(x => x.Id == id)
            .Include(u => u.Comments)
            .Include(u => u.Posts)
            .IncludeUserImages()
            .FirstOrDefaultAsync();

        if (user == null)
            return ServiceResult<UserResponse>.Failure("Пользователя с таким id не существует");

        Console.WriteLine(user);

        var userResponse = _mapper.Map<UserResponse>(user);

        return ServiceResult<UserResponse>.Success(userResponse);
    }

    //Объединил бы в один
    public async Task<ServiceResult<List<UserResponse>>> GetUsersByCommunityAsync(long communityId, int page = 1,
        int pageSize = 10)
    {
        var user = await _context.Users
            .Include(u => u.UserCommunities)
            .Where(u => u.UserCommunities.Any(uc => uc.CommunityId == communityId))
            .Include(u => u.Posts)
            .Include(u => u.Comments)
            .IncludeUserImages()
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(page)
            .ToListAsync();

        var userResponse = _mapper.Map<List<UserResponse>>(user);

        return ServiceResult<List<UserResponse>>.Success(userResponse);
    }

    public async Task<ServiceResult<List<UserResponse>>> GetUsersAsync(int page = 1, int limit = 10)
    {
        var users = await _context.Users
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Include(u => u.Posts)
            .Include(u => u.Comments)
            .IncludeUserImages()
            .ToListAsync();

        var mappingUsers = _mapper.Map<List<UserResponse>>(users);

        return ServiceResult<List<UserResponse>>.Success(mappingUsers);
    }

    public async Task<ServiceResult<bool>> DeleteUser(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден");

        var imageIds = await _context.Images
            .Where(i => i.EntityId == id && i.EntityTarget == nameof(User))
            .Select(i => i.Id)
            .ToListAsync();

        await _imageService.RemoveImages(imageIds);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<Image>> UploadBannerAsync(List<IFormFile> images, long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult<Image>.Failure("Пользователь не найден");

        //var oldBanner = await _context.Images
        //    .Where(i => i.EntityTarget == nameof(User) && i.EntityId == userId && i.ImageType == "banner")
        //    .Select(i => i.Id)
        //    .ToListAsync();

        //if (oldBanner.Count > 0) await _imageService.RemoveImages(oldBanner);

        await _imageService.AddUploadedImagesAsync(nameof(User), user.Id, "banner", images);

        return ServiceResult<Image>.Success();
    }


    public async Task<ServiceResult<bool>> UpdateUserInfoAsync(UserUpdateRequest userDto, long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден");

        if (user.UserName != userDto.UserName)
        {
            var uniqueUserName = await _context.Users.AnyAsync(u => u.UserName == userDto.UserName);
        }

        user.UserName = userDto.UserName;
        user.Description = userDto.Description;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<Image>> UploadAvatarAsync(IFormFile image, long userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult<Image>.Failure("Пользователь не найден");

        var oldAvatar = await _context.Images
            .FirstOrDefaultAsync(i =>
                i.EntityTarget == nameof(User) && i.EntityId == userId && i.ImageType == "avatar");

        if (oldAvatar != null) await _imageService.RemoveImages(new List<long> { oldAvatar.Id });

        await _imageService.AddUploadedImagesAsync(nameof(User), user.Id, "avatar", new List<IFormFile> { image });

        return ServiceResult<Image>.Success();
    }
}