using System.Net;
using Microsoft.IdentityModel.Tokens;
using SmartInventorySystemApi.Application.Exceptions;

namespace SmartInventorySystemApi.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An exception occurred while processing the request");
            await HandleGlobalExceptionAsync(context, ex);
        }
    }

    private async Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var message = exception.Message;
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case EntityAlreadyExistsException entityAlreadyExistsException:
                message = entityAlreadyExistsException.Message;
                statusCode = HttpStatusCode.Conflict;
                break;

            case InvalidOperationException invalidOperationException:
                message = invalidOperationException.Message;
                statusCode = HttpStatusCode.Conflict;
                break;
                    
            case EntityNotFoundException entityNotFoundException:
                message = entityNotFoundException.Message;
                statusCode = HttpStatusCode.NotFound;
                break;

            case InvalidEmailException invalidEmailException:
                message = invalidEmailException.Message;
                statusCode = HttpStatusCode.UnprocessableEntity;
                break;

            case InvalidPhoneNumberException invalidPhoneNumberException:
                message = invalidPhoneNumberException.Message;
                statusCode = HttpStatusCode.UnprocessableEntity;
                break;

            case InvalidDataException invalidDataException:
                message = invalidDataException.Message;
                statusCode = HttpStatusCode.BadRequest;
                break;

            case SecurityTokenException securityTokenException:
                message = securityTokenException.Message;
                statusCode = HttpStatusCode.Unauthorized;
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                message = unauthorizedAccessException.Message;
                statusCode = HttpStatusCode.Forbidden;
                break;

            default:
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var response = new 
        {
            message = message
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
