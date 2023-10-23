using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.IServices.Identity;
using SmartInventorySystemApi.Infrastructure.Services;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IGroupsService, GroupsService>();
        services.AddScoped<IDevicesService, DevicesService>();

        return services;
    }

    public static IServiceCollection AddJWTTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateIssuer"),
                    ValidateAudience = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateAudience"),
                    ValidateLifetime = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateLifetime"),
                    ValidateIssuerSigningKey = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateIssuerSigningKey"),
                    ValidIssuer = configuration.GetValue<string>("JsonWebTokenKeys:ValidIssuer"),
                    ValidAudience = configuration.GetValue<string>("JsonWebTokenKeys:ValidAudience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JsonWebTokenKeys:IssuerSigningKey"))),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
