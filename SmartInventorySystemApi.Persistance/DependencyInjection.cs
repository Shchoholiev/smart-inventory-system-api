using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Persistance.Database;
using SmartInventorySystemApi.Persistance.Repositories;

namespace SmartInventorySystemApi.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();

        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        
        return services;
    }
}