using Application.Exceptions;
using Application.Extensions;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Results;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext applicationDbContext, IMapper autoMapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = autoMapper;
    }

    public async Task<ServiceResult<UserResponse>> GetUserAsync(int id)
    {
        var user = _mapper.Map<UserResponse>(await _applicationDbContext.Users.Where(x => x.Id == id).FirstAsync());

        if (user == null)
            return ServiceResult<UserResponse>.Failure("Пользователя с таким айди не существует");

        var banners = _mapper.Map<List<ImageResponse>>(await _applicationDbContext.Images
            .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == id)
            .ToListAsync());

        user.Banners = banners;
        return ServiceResult<UserResponse>.Success(user);
    }

    public async Task<ServiceResult<List<UserResponse>>> GetUsersAsync(int page = 1, int limit = 10)
    {
        if (page < 1 || limit < 1)
            return ServiceResult<List<UserResponse>>.Failure(
                "Стартовая позиция или количество элементов имеют некорректное значение");

        var users = await _applicationDbContext.Users
            .Skip((page - 1) * limit)
            .Take(limit)
            .OrderBy(u => u.UserName)
            .IncludeUserImages()
            .ToListAsync();

        var mappingUsers = _mapper.Map<List<UserResponse>>(users);

        return ServiceResult<List<UserResponse>>.Success(mappingUsers);
    }

    public async Task<ServiceResult<bool>> DeleteUser(int id)
    {
        var user = await _applicationDbContext.Users.FindAsync(id);
        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден");

        var images = await _applicationDbContext.Images
            .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == id)
            .ToListAsync();

        _applicationDbContext.Images.RemoveRange(images);

        _applicationDbContext.Users.Remove(user);

        await _applicationDbContext.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<long>> AddUserAsync(UserRequest userDto)
    {
        if (userDto == null)
            return ServiceResult<long>.Failure("Данные о пользователе отсутствуют");

        var user = _mapper.Map<EntityUser>(userDto);

        var passwordHasher = new PasswordHasher<object>();
        user.PasswordHash = passwordHasher.HashPassword(null, user.PasswordHash);

        await _applicationDbContext.Users.AddAsync(user);
        await _applicationDbContext.SaveChangesAsync();

        return ServiceResult<long>.Success(user.Id);
    }

    //обновление аватарки(или загрузка)
    //public async Task<ServiceResult<string>> UploadAvatar(IFormFile avatar, int userId)
    //{
    //    if (avatar == null || avatar.Length == 0) //Проверить на png,jpg... макс размер файла
    //        return ServiceResult<string>.Failure("Неверный формат ошибка");

    //    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");

    //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
    //    var filePath = Path.Combine(uploadsPath, fileName);

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await avatar.CopyToAsync(stream);
    //    }

    //    var user = await _applicationDbContext.Users.FindAsync(userId);
    //    if (user == null)
    //        throw new NotFoundException("Пользователь не найден");

    //    user.AvatarUrl = $"/uploads/avatars/{fileName}";
    //    await _applicationDbContext.SaveChangesAsync();

    //    return ServiceResult<string>.Success(user.AvatarUrl);
    //}

    //обновление юзера
    public async Task<ServiceResult<bool>> UpdateUserAsync(UserRequest userDto, int id)
    {
        if (userDto == null)
            return ServiceResult<bool>.Failure("Данные о пользователе отсутствуют");

        var user = _mapper.Map<EntityUser>(userDto);
        user.Id = id;
        _applicationDbContext.Users.Update(user);
        await _applicationDbContext.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }
}