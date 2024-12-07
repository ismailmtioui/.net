using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xyz.Services.Abstractions;

namespace Xyz.Services;

public static class Registration
{
    public static IServiceCollection AddServicesDependencies(
        this IServiceCollection services)
    {
        //services
        services.AddScoped<IUserService, UserService>();
        
        //validators
        services.AddValidatorsFromAssemblyContaining(typeof(Registration));
        
        return services;
    }
}