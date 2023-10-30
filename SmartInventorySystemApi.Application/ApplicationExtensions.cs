using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystemApi.Application.Mapping;

namespace SmartInventorySystemApi.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserProfile)));

        return services;
    }
}
