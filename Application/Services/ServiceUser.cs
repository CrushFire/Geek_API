using Application.Exceptions;
using AutoMapper;
using Core.Models;
using DataAccess;
using DataAccess.Entities;
using DataAccess.Interfaces;
using FluentNHibernate.Automapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHibernate.Mapping.ByCode.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServiceUser : IServiceUser
    {
        private readonly DBContext _dbContext;
        private readonly IMapper _mapper;
        public ServiceUser(DBContext dbContext, IMapper autoMapper)
        {
            _dbContext = dbContext;
            _mapper = autoMapper;
        }

        public async Task<DtoResponseUser> GetUserAsync(int id)
        {
            var user = _mapper.Map<DtoResponseUser>(await _dbContext.Users.Where(x => x.Id == id).FirstAsync());

            if (user == null)
            {
                throw new DataNullException("Пользователя с таким айди не существует");
            }

            var banners = _mapper.Map<List<DtoResponseImage>>(await _dbContext.Images
            .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == id)
            .ToListAsync());

            user.Banners = banners;
            return user;
        }

        public async Task<List<DtoResponseUser>> GetUsersAsync(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new DataInValidValues("Стартовая позиция или количество элементов имеют некорректное значение");
            }//Проверка старт позиции?
            var users = _mapper.Map<List<DtoResponseUser>>(await _dbContext.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
            foreach (var user in users)
            {
                var banners = _mapper.Map<List<DtoResponseImage>>(await _dbContext.Images
                    .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == user.Id)
                    .ToListAsync());

                user.Banners = banners;
            }
            return users;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new DataNullException("Пользователь не найден");
            }

            var images = await _dbContext.Images
                .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == id)
                .ToListAsync();

            _dbContext.Images.RemoveRange(images);

            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> AddUserAsync(DtoRequestUser _user)
        {
            if (_user == null)
            {
                throw new DataNullException("Данные о пользователе отсутствуют");
            }
            var user = _mapper.Map<EntityUser>(_user);
            user.AvatarUrl = "/uploads/avatars/defaultAvatar";//маршрут де лежит дефолтная ава
            var passwordHasher = new PasswordHasher<object>();
            user.Password = passwordHasher.HashPassword(null, user.Password);//хешируем
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        //обновление юзера
        public bool UpdateUser(DtoRequestUser _user, int id)
        {
            if (_user == null)
            {
                throw new DataNullException("Данные о пользователе отсутствуют");
            }

            var user = _mapper.Map<EntityUser>(_user);
            user.Id = id;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return true;
        }

        //обновление аватарки(или загрузка)
        public async Task<string> UploadAvatar(IFormFile avatar, int userId)
        {
            if (avatar == null || avatar.Length == 0)
                throw new ErrorOpenFileException("Файл не выбран");

            //директория
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");

            //если нет создаем
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
            await _dbContext.SaveChangesAsync();

            return user.AvatarUrl;
        }

    }
}
