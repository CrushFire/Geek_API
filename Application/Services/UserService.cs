using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.User;
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
            .Include(u => u.Comments)
            .Include(u => u.Posts)
            .Include(u => u.Reactions)
            .Include(u => u.UserCommunities.Where(uc => uc.UserRole != "creator"))
            .Include(u => u.Images)//тут был экстендид ну щас вроде ок...
            .FirstOrDefaultAsync(x => x.Id == id);

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
            .Include(u => u.Reactions)
            .IncludeUserImages()
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(page)
            .ToListAsync();

        var userResponse = _mapper.Map<List<UserResponse>>(user);

        return ServiceResult<List<UserResponse>>.Success(userResponse);
    }

    public async Task<ServiceResult<List<UserReactionsResponse>>> GetUserReactionsAsync (long id)
    {
        var reactions = await _context.Likes.Where(l => l.UserId == id)
            .ToListAsync();

        if (reactions == null)
            return ServiceResult<List<UserReactionsResponse>>.Failure("У этого пользователя нет реакций");

        var reactionsResponse = _mapper.Map<List<UserReactionsResponse>>(reactions);

        return ServiceResult<List<UserReactionsResponse>>.Success(reactionsResponse);
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

        var userImageIds = await _context.Images
            .Where(i => i.EntityId == id && i.EntityTarget == nameof(User))
            .Select(i => i.Id)
            .ToListAsync();

        // 2. Удалить изображения постов пользователя
        var postIds = await _context.Posts
            .Where(p => p.AuthorId == id)
            .Select(p => p.Id)
            .ToListAsync();

        var postImageIds = await _context.Images
            .Where(i => postIds.Contains(i.EntityId) && i.EntityTarget == nameof(Post))
            .Select(i => i.Id)
            .ToListAsync();

        // 3. Найти сообщества, где пользователь creator
        var userCommunities = await _context.UsersCommunities
            .Where(uc => uc.UserId == id && uc.UserRole == "creator")
            .ToListAsync();

        var userCommunityIds = userCommunities.Select(uc => uc.Id).ToList();

        var userCommunityImageIds = await _context.Images
            .Where(i => userCommunityIds.Contains(i.EntityId) && i.EntityTarget == nameof(Community))
            .Select(i => i.Id)
            .ToListAsync();

        // Собираем все изображения
        var allImageIds = userImageIds
            .Concat(postImageIds)
            .Concat(userCommunityImageIds)
            .ToList();

        // Удаляем изображения
        await _imageService.RemoveImages(allImageIds);

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
        var userAvatar = await _context.Images.Where(i => i.EntityTarget == "User" && i.ImageType == "avatar" && i.EntityId == id).FirstOrDefaultAsync();

        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден");

        if (user.UserName != userDto.UserName)
        {
            var uniqueUserName = await _context.Users.AnyAsync(u => u.UserName == userDto.UserName);

            if (uniqueUserName)
            {
                return ServiceResult<bool>.Failure("Такой юзер уже есть", 203);
            }
        }

        user.UserName = userDto.UserName;
        user.Description = userDto.Description;

        if (userAvatar != null && userDto.Avatar != null)
        {
            _imageService.RemoveImageFromServer(userAvatar.Id);
            var newImage = _imageService.AddUploadedImageAsync("User", id, "avatar", userDto.Avatar);
            userAvatar.ImageUrl = newImage.Result.ImageUrl;
            _context.Images.Update(userAvatar);
        }
        else if (userAvatar == null && userDto.Avatar != null)
        {
            var newUploadImage = _imageService.AddUploadedImageAsync("User", id, "avatar", userDto.Avatar);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
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

    public async Task<ServiceResult<bool>> ChangeUserRoleAsync(long userId, string newRole)
    {
        var allowedRoles = new[] { "User", "Admin" };
        if (string.IsNullOrWhiteSpace(newRole) || !allowedRoles.Contains(newRole, StringComparer.OrdinalIgnoreCase))
        {
            return ServiceResult<bool>.Failure("Недопустимая роль");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден");

        user.Role = allowedRoles.First(r => r.Equals(newRole, StringComparison.OrdinalIgnoreCase));
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }

    public async Task<List<Core.Models.User.UserAdminResponse>> GetUsersAdminAsync(string name, int curPage, int pageSize = 20)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(u => u.UserName.Contains(name));

        var users = await query
            .OrderBy(u => u.UserName)
            .Skip((curPage - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new Core.Models.User.UserAdminResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.UserEmail,
                Role = u.Role
            })
            .ToListAsync();

        return users;
    }
}