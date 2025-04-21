using Application.Exceptions;
using Core.Models;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class ControllerUser : ControllerBase
    {
        IServiceUser _serviceUser;
        public ControllerUser(IServiceUser serviceUser)
        {
            _serviceUser = serviceUser;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            try
            {
                var user = await _serviceUser.GetUserAsync(id);
                return Ok(new ApiResponse<DtoResponseUser>(user));
            }
            catch (DataNullException)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid data"));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var users = await _serviceUser.GetUsersAsync(page, pageSize);
                return Ok(new ApiResponse<List<DtoResponseUser>>(users));
            }
            catch (DataInValidValues)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid data"));
            }
        }

        [HttpPost]

        public async Task<IActionResult> AddUserAsync([FromBody] DtoRequestUser _user)
        {
            try
            {
                var id = await _serviceUser.AddUserAsync(_user);
                return Ok(new ApiResponse<int>(id));
            }
            catch (DataNullException)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid data"));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            try
            {
                await _serviceUser.DeleteUser(id);
                return Ok(new ApiResponse<bool>(true));
            }
            catch (DataNullException)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid data"));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public IActionResult UpdateUser( [FromBody] DtoRequestUser _user, int id)
        {
            try
            {
                _serviceUser.UpdateUser(_user, id);
                return Ok(new ApiResponse<bool>(true));
            }
            catch (DataNullException)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid data"));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("avatar/{id}")]

        public async Task<IActionResult> UploadAvatarUserAsync( [FromBody] IFormFile _file, [FromRoute] int id)
        {
            try
            {
                var filePath = await _serviceUser.UploadAvatar(_file, id);
                return Ok(new ApiResponse<string>(filePath));
            }
            catch (ErrorOpenFileException)
            {
                return BadRequest(new ApiErrorResponse(404, "Invalid file"));
            }
            catch (NotFoundException)
            {
                return BadRequest(new ApiErrorResponse(404, "User not found"));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        }
}
