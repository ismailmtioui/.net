using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xyz.Infrastructure.Abstractions;
using Xyz.Infrastructure.EF.Users;
using Xyz.Infrastructure.EF.UserTokens;
using Xyz.SDK.Dao;
using Xyz.SDK.Dao.Config;

#pragma warning disable CS8603 // Possible null reference return.

namespace Xyz.Infrastructure.EF;

public static class Registration
{
    public static IServiceCollection AddDatabaseDependencies(
        this IServiceCollection services, 
        DatabaseConfig databaseConfig)
    {
        //db context
        services.AddDbContext<UnitOfWork>(options =>
            options.UseNpgsql(databaseConfig.ConnectionString));

        //uow
        services.AddScoped<IUnitOfWork, UnitOfWork>(scope => scope.GetService<UnitOfWork>());
        
        //repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        return services;
    }
}