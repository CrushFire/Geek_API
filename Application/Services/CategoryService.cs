using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Category;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(ApplicationDbContext context, IMapper mapper)
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

        public async Task<ServiceResult<List<CategoryResponse>>> GetCategoryAsync(PaginationRequest pagination)
        {
            var categories = await _context.Categories.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

            var categoriesResponse = _mapper.Map<List<CategoryResponse>>(categories);
            return ServiceResult<List<CategoryResponse>>.Success(categoriesResponse);
        }

        public async Task<ServiceResult<CategoryResponse>> AddCategoryAsync(CategoryRequest categoryRequest)
        {
            Category category = new Category();
            category.Title = categoryRequest.RuTitle;
            category.EngTitle = categoryRequest.EngTitle;

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return ServiceResult<CategoryResponse>.Success(categoryResponse);
        }

        public async Task<ServiceResult<bool>> UpdateCategoryAsync(CategoryRequest categoryRequest, int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return ServiceResult<bool>.Failure("Данной категории не найдено");
            }

            category.Title = categoryRequest.RuTitle;
            category.EngTitle = categoryRequest.EngTitle;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}