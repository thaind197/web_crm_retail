namespace SalesCRM.Shared.Models;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResult<T> Success(T data, string? message = null) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message
    };

    public static ApiResult<T> Failure(string message, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };
}

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResult Success(string? message = null) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ApiResult Failure(string message, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };
}
