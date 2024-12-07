using System.Text;

namespace Xyz.WebAPI.Middlewares;

public class CurlLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public CurlLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Clone the original request body so that we can read it here and then pass it to the next middleware
        using var requestBodyStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        // Generate curl command
        var curlCommand = GenerateCurlCommand(context.Request, requestBodyText);
        Console.WriteLine(curlCommand);
        await _next(context);
    }
    
    private string GenerateCurlCommand(HttpRequest request, string requestBodyText)
    {
        var curl = new StringBuilder("curl");

        // Add HTTP method
        curl.Append($" -X {request.Method}");

        // Add URL
        curl.Append($" \"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}\"");

        // Add headers
        foreach (var header in request.Headers)
        {
            curl.Append($" -H \"{header.Key}: {header.Value}\"");
        }

        // Add content-type and body if applicable
        if (!string.IsNullOrEmpty(requestBodyText))
        {
            var contentType = request.ContentType ?? "application/x-www-form-urlencoded";
            curl.Append($" -H \"Content-Type: {contentType}\"");
            curl.Append($" --data \"{requestBodyText.Replace("\"", "\\\"")}\"");
        }

        return curl.ToString();
    }
}