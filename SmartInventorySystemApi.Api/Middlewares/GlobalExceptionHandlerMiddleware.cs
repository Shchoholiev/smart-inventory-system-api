using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Domain.Common;

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
