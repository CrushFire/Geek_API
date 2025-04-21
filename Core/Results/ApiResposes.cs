namespace Geek_API.Controllers;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponse CreateSuccess(object? data = null)
    {
        return new ApiResponse { Success = true, Data = data, Error = null };
    }

    public static ApiResponse CreateFailure(string error = "Server error")
    {
        return new ApiResponse { Success = false, Data = null, Error = error };
    }
}