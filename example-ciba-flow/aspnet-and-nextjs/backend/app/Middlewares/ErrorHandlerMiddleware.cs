using ExampleAspnet.Exceptions;
using ExampleAspnet.Models;

namespace ExampleAspnet.Middlewares;

public class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ExampleAppException e)
        {
            await HandleKnownException(context, e);
        }
        catch (Exception e)
        {
            await HandleUnknownException(context, e);
        }
    }

    private async Task HandleKnownException(HttpContext context, ExampleAppException exception)
    {
        _logger.LogWarning("Request failed with exception: {}", exception.Message);

        var statusCode = exception switch
        {
            ApiClientException e => (int)e.StatusCode,
            GrantTimeoutException => StatusCodes.Status400BadRequest,
            InvalidSessionException => StatusCodes.Status400BadRequest,
            AuthFlowException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = exception switch
        {
            ApiClientException e => new ErrorResponse(e.Name ?? "unknown", e.Description ?? "Unknown error"),
            GrantTimeoutException => ErrorResponse.GrantTimeout,
            InvalidSessionException => ErrorResponse.InvalidSession,
            AuthFlowException => ErrorResponse.AuthFlowError,
            _ => ErrorResponse.InternalServerError
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleUnknownException(HttpContext context, Exception exception)
    {
        _logger.LogError("Unhandled exception: {}", exception);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(ErrorResponse.InternalServerError);
    }
}