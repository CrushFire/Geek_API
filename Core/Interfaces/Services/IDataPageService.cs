using Core.Models.DataPage;
using Core.Results;

namespace Core.Interfaces.Services
{
    public interface IDataPageService
    {
        Task<ServiceResult<DataPageResponse>> AddDataAsync(DataPageRequest dataRequest);
        Task<ServiceResult<bool>> DeleteDataAsync(int id);
        Task<ServiceResult<DataPageResponse>> GetByIdAsync(int id);
        Task<ServiceResult<DataPageResponse>> GetByNameAsync(string name);
        Task<ServiceResult<Dictionary<string, DataPageRequest>>> GetByPageAsync(string page);
        Task<ServiceResult<DataPageResponse>> GetByPageAndNameAsync(DataPageNameRequest request);
        Task<ServiceResult<bool>> UpdateRuDataAsync(DataPageUpdateRequest dataRequest);
        Task<ServiceResult<bool>> UpdateEngDataAsync(DataPageUpdateRequest dataRequest);
    }
}