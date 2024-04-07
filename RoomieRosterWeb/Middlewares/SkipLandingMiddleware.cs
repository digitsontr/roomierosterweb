public class SkipLandingMiddleware
{
    private readonly RequestDelegate _next;

    public SkipLandingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){
        if(context.Request.Cookies.ContainsKey("LandingVisited") 
        && (context.Request.Path.Value == "/" || (context.Request.Path.Value ?? "").ToLower().Contains("index"))){
            context.Response.Redirect("/Home/App");
        }

        await _next(context);
    }
}

public static class SkipLandingMiddlewareExtensions
{
    public static IApplicationBuilder UseSkipLandingMiddleware(this IApplicationBuilder builder){
        return builder.UseMiddleware<SkipLandingMiddleware>();
    }
}