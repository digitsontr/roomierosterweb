public class SkipLandingMiddleware
{
    private readonly RequestDelegate _next;

    public SkipLandingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string requestPath = (context.Request.Path.Value ?? "").ToLower();

        if (context.Request.Cookies.ContainsKey("LandingVisited")
        && (requestPath == "/" || (requestPath.Contains("index") && requestPath.Contains("home"))))
        {
            context.Response.Redirect("/Home/App");
        }

        await _next(context);
    }
}

public static class SkipLandingMiddlewareExtensions
{
    public static IApplicationBuilder UseSkipLandingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SkipLandingMiddleware>();
    }
}