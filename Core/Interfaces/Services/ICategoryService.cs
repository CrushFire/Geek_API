using Core.Models;
using Core.Models.Category;
using Core.Results;

namespace Core.Interfaces.Services;

public interface ICategoryService
{
    Task<ServiceResult<CategoryResponse>> AddCategoryAsync(CategoryRequest categoryRequest);
    Task<ServiceResult<List<CategoryResponse>>> GetCategoryAsync(PaginationRequest pagination);
    Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
    Task<ServiceResult<bool>> UpdateCategoryAsync(CategoryRequest categoryRequest, int id);
}