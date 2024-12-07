using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;

namespace Xyz.WebAPI.Middlewares;

public class SwaggerBasicAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string? authHeader = context.Request.Headers["Authorization"];
            if (authHeader is not null 
                && authHeader.StartsWith("Basic ") 
                && AuthenticationHeaderValue.TryParse(authHeader, out var header) 
                && header.Parameter is not null)
            {
                // Get the credentials from request header
                var inBytes = Convert.FromBase64String(header.Parameter);
                var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                
                // validate credentials
                if (IsValidUser(context, credentials[0], credentials[1]))
                {
                    await next.Invoke(context).ConfigureAwait(false);
                    return;
                }
            }
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            await next.Invoke(context).ConfigureAwait(false);
        }
    }
    
    private static bool IsValidUser(HttpContext context, string username, string password)
    {
        var swaggerCredentials =
            context.RequestServices.GetRequiredService<IOptions<SwaggerConfig>>().Value;

        return username == swaggerCredentials.UserName && password == swaggerCredentials.Password;
    }
}

public class SwaggerConfig
{
    public string UserName { get; set; }
    public string Password { get; set; }
}