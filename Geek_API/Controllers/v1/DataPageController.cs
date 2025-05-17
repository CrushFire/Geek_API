using Application.Services;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models.DataPage;
using Core.Models.Post;
using Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DataPageController : CustomControllerBase
    {
        private readonly IDataPageService _dataService;

        public DataPageController(IDataPageService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("{Id}")]

        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _dataService.GetByIdAsync(id);
            return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpGet("byNamePage")]

        public async Task<IActionResult> GetByPageAsync(string pageName)
        {
            var result = await _dataService.GetByPageAsync(pageName);
            return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpGet("byNameData")]

        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var result = await _dataService.GetByNameAsync(name);
            return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpGet("byPageNameData")]

        public async Task<IActionResult> GetByPageNameAsync(DataPageNameRequest request)
        {
            var result = await _dataService.GetByPageAndNameAsync(request);
            return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpPost]

        public async Task<IActionResult> AddAsync([FromBody] DataPageRequest request)
        {
            var result = await _dataService.AddDataAsync(request);
            return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpPut("eng")]

        public async Task<IActionResult> UpdateEngAsync([FromBody] DataPageUpdateRequest dataRequest)
        {

            var result = await _dataService.UpdateEngDataAsync(dataRequest);
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpPut("ru")]

        public async Task<IActionResult> UpdateRuAsync([FromBody] DataPageUpdateRequest dataRequest)
        {

            var result = await _dataService.UpdateRuDataAsync(dataRequest);
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }

        [HttpDelete]

        public async Task<IActionResult> DeleteAsync([FromBody] int id)
        {

            var result = await _dataService.DeleteDataAsync(id);
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
        }
    }
}
