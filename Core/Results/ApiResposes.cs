namespace Geek_API.Controllers;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public ErrorResponse? Error { get; set; }

    public static ApiResponse CreateSuccess(object? data = null)
    {
        return new ApiResponse { Success = true, Data = data, Error = null };
    }

    public static ApiResponse CreateFailure(string errorMessage, Dictionary<string, string> errorDetails = null)
    {
        return new ApiResponse { Success = false, Data = null, Error = new ErrorResponse(errorMessage, errorDetails) };
    }

    public class ErrorResponse
    {
        public ErrorResponse(string errorMessage, Dictionary<string, string>? detailsError)
        {
            ErrorMessage = errorMessage;
            DetailsError = detailsError;
        }

        public string ErrorMessage { get; set; }
        public Dictionary<string, string>? DetailsError { get; set; }
    }
}