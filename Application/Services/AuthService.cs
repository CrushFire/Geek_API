using Application.Utils;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Models.Authorisation;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwtGenerator;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(ApplicationDbContext context, JwtTokenGenerator jwtGenerator, IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtGenerator = jwtGenerator;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<ServiceResult<string>> RegisterAsync(RegisterRequest userRequest)
    {
        var userExist = await _context.Users.AnyAsync(u => u.UserName == userRequest.UserName);

        if (userExist)
            return ServiceResult<string>.Failure("Пользователь с таким ником уже существует", 409);

        userExist = await _context.Users.AnyAsync(u => u.UserEmail == userRequest.UserEmail);

        if (userExist)
            return ServiceResult<string>.Failure("Данная почта уже используется", 409);

        var newUser = new User { UserName = userRequest.UserName, UserEmail = userRequest.UserEmail, Role = "User" };
        newUser.PasswordHash = _passwordHasher.HashPassword(userRequest.Password);

        await _context.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var token = _jwtGenerator.GenerateToken(newUser.Id.ToString(), newUser.UserName, newUser.UserEmail, newUser.Role);
        return ServiceResult<string>.Success(token);
    }

    public async Task<ServiceResult<(string token, long id)>> AuthenticateAsync(LoginRequest userData)
    {
        var user = await _context.Users
            .Where(u => u.UserEmail == userData.UserEmail)
            .FirstOrDefaultAsync();

        if (user == null)
            return ServiceResult<(string token, long id)>.Failure("Пользователь не найден");

        if (!_passwordHasher.VerifyPassword(userData.Password, user.PasswordHash))
            return ServiceResult<(string token, long id)>.Failure("Неверный пароль", 401);

        var token = _jwtGenerator.GenerateToken(user.Id.ToString(), user.UserName, user.UserEmail, user.Role);

        return ServiceResult<(string, long)>.Success((token, user.Id));
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(long userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult<bool>.Failure("Пользователь не найден", 404);

        if (!_passwordHasher.VerifyPassword(oldPassword, user.PasswordHash))
            return ServiceResult<bool>.Failure("Неверный пароль", 401);

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }
}