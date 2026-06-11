using System.Net;
using System.Text.Json;
using SalesCRM.Shared.Exceptions;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";
        List<string>? errors = null;

        if (exception is AppException appEx)
        {
            statusCode = appEx.StatusCode;
            message = appEx.Message;
            errors = appEx.Errors;
        }

        context.Response.StatusCode = (int)statusCode;

        var result = ApiResult.Failure(message, errors);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var payload = JsonSerializer.Serialize(result, jsonOptions);

        return context.Response.WriteAsync(payload);
    }
}
