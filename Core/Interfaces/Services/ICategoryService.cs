using Core.Models.Category;
using Core.Results;

namespace Application.Services
{
    public interface ICategoryService
    {
        Task<ServiceResult<CategoryResponse>> AddCategoryAsync(string title);
        Task<ServiceResult<List<CategoryResponse>>> GetCategoryAsync(int page = 1, int pageSize = 10);
        Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
        Task<ServiceResult<bool>> UpdateCategoryAsync(string title, int id);
    }
}