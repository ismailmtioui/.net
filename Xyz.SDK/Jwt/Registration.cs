using Microsoft.Extensions.DependencyInjection;

namespace Xyz.SDK.Jwt;

public static class Registration
{
    public static IServiceCollection AddJwtHandler(
        this IServiceCollection services)
    {
        services.AddScoped<IJwtHandler, JwtHandler>();

        return services;
    }
}