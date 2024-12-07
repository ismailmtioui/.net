using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Xyz.Infrastructure.EF;
using Xyz.SDK.Dao.Config;
using Xyz.SDK.Jwt;
using Xyz.SDK.Jwt.Config;
using Xyz.Services;
using Xyz.WebAPI.Middlewares;

#pragma warning disable CS8604 // Possible null reference argument.

namespace  Xyz.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var databaseConfig = builder.Configuration.GetRequiredSection("Database").Get<DatabaseConfig>();
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                    return new[] { api.GroupName };

                if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    return new[] { controllerActionDescriptor.ControllerName };

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });
            c.DocInclusionPredicate((_, _) => true);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
            c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = HeaderNames.Authorization,
                Description = "Enter the Authorization header as: `Bearer {JwtToken}`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });

        //config
        builder.Services.Configure<JwtConfig>(builder.Configuration.GetRequiredSection("Jwt"));
        builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetRequiredSection("SendGrid"));
        builder.Services.Configure<SwaggerConfig>(builder.Configuration.GetRequiredSection("Swagger"));

        // http context
        builder.Services.AddHttpContextAccessor();
        
        // infrastructure
        builder.Services.AddDatabaseDependencies(databaseConfig);
        
        // services
        builder.Services.AddServicesDependencies();
        
        // logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        
        // jwt
        builder.Services.AddJwtHandler();
        
        // cors
        builder.Services.AddCors();
        
        //mappers
        builder.Services.AddAutoMapper(typeof(Program));
        
        // health check
        builder.Services.AddHealthChecks()
            .AddNpgSql(databaseConfig.ConnectionString, name: "Main DB", timeout: TimeSpan.FromSeconds(5));
        
        // healthchecks-ui
        builder.Services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(10);    
            opt.MaximumHistoryEntriesPerEndpoint(60);
            opt.SetApiMaxActiveRequests(1);
            opt.AddHealthCheckEndpoint("feedback api", "/api/health");   
        }).AddInMemoryStorage();
        
        await RunApiAsync(builder);
    }

    private static async Task RunApiAsync(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.MapHealthChecks("/health");
        app.MapHealthChecksUI();
        
        app.UseMiddleware<CurlLoggingMiddleware>();
        app.UseMiddleware<SwaggerBasicAuthMiddleware>();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseAuthentication();

        app.UseAuthorization();
        
        app.MapControllers();

        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        await app.RunAsync();
    }
}
