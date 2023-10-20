using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;
using SmartInventorySystemApi.Application.Models.GlobalInstances;

namespace SmartInventorySystemApi.Api.Middlewares;

public class GlobalUserCustomMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalUserCustomMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (ObjectId.TryParse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ObjectId id))
        {
            GlobalUser.Id = id;
        }
        GlobalUser.Name = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        GlobalUser.Email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        GlobalUser.Phone = httpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value;
        
        GlobalUser.Roles = new List<string>();
        foreach (var role in httpContext.User.FindAll(ClaimTypes.Role))
        {
            GlobalUser.Roles.Add(role.Value);
        }

        await this._next(httpContext);
    }
}
