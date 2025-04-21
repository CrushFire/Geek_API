namespace Geek_API.Controllers
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data, bool success = true)
        {
            Success = success;
            Data = data;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
    }

    class ApiErrorResponse
    {
        public ApiErrorResponse(int code, string message, IEnumerable<string>? errors = null, bool success = false)
        {
            Success = success;
            Error = new ApiError(code, message, errors);
        }

        public bool Success { get; set; }
        public ApiError Error { get; set; }

    }

    class ApiError
    {
        public ApiError(int code, string message, IEnumerable<string>? errors = null)
        {
            Code = code;
            Message = message;
            Errors = errors;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
