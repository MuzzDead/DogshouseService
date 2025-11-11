namespace DogshouseService.WebAPI.Middlewares;

public class SimpleRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _limitPerSecond;
    private static long _windowStartTicks = DateTime.UtcNow.Ticks;
    private static int _count = 0;

    public SimpleRateLimitMiddleware(RequestDelegate next, int limitPerSecond)
    {
        _next = next;
        _limitPerSecond = limitPerSecond;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var nowTicks = DateTime.UtcNow.Ticks;
        var nowSec = TimeSpan.FromTicks(nowTicks).TotalSeconds;
        var startSec = TimeSpan.FromTicks(Interlocked.Read(ref _windowStartTicks)).TotalSeconds;

        if (nowTicks - startSec >= 1)
        {
            Interlocked.Exchange(ref _count, 0);
            Interlocked.Exchange(ref _windowStartTicks, DateTime.UtcNow.Ticks);
        }

        var currentCount = Interlocked.Increment(ref _count);
        if (currentCount > _limitPerSecond)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        await _next(context);
    }
}
