namespace Core.Results;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public ErrorResponse? Error { get; set; }

    public static ServiceResult<T> Success(T? data = default)
    {
        return new ServiceResult<T> { IsSuccess = true, Data = data, Error = null };
    }

    public static ServiceResult<T> Failure(string error, int statusCode = 400)
    {
        return new ServiceResult<T> { IsSuccess = false, Data = default, Error = new ErrorResponse(error, statusCode) };
    }

    public class ErrorResponse
    {
        public string ErrorMessage;
        public int StatusCode;

        public ErrorResponse(string errorMessage, int statusCode)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}