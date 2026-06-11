using System.Net;

namespace SalesCRM.Shared.Exceptions;

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public List<string>? Errors { get; }

    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string>? errors = null) 
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound)
    {
    }
}

public class ValidationException : AppException
{
    public ValidationException(string message, List<string> errors) 
        : base(message, HttpStatusCode.BadRequest, errors)
    {
    }
    
    public ValidationException(List<string> errors) 
        : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, errors)
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized access.") 
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
