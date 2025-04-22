using AutoMapper;
using Core.Entities;
using Core.Models.Category;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        CategoryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return ServiceResult<CategoryResponse>.Failure("Категория не найдена");
            }

            var categoryResponse = _mapper.Map<CategoryResponse>(category);
            return ServiceResult<CategoryResponse>.Success(categoryResponse);
        }

        public async Task<ServiceResult<List<CategoryResponse>>> GetCategoryAsync(int page = 1, int pageSize = 10)
        {
            var categories = await _context.Categories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var categoriesResponse = _mapper.Map<List<CategoryResponse>>(categories);
            return ServiceResult<List<CategoryResponse>>.Success(categoriesResponse);
        }

        public async Task<ServiceResult<CategoryResponse>> AddCategoryAsync(string title)
        {
            Category category = new Category();
            category.Title = title;

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return ServiceResult<CategoryResponse>.Success(categoryResponse);
        }

        public async Task<ServiceResult<bool>> UpdateCategoryAsync(string title, int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return ServiceResult<bool>.Failure("Данной категории не найдено");
            }

            category.Title = title;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}