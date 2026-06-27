using System.Net;
using System.Text.Json;
using FCG.UsersAPI.Domain.Exceptions;

namespace FCG.UsersAPI.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain violation");
            await WriteError(ctx, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(ctx, HttpStatusCode.InternalServerError,
                "Ocorreu um erro interno.");
        }
    }

    private static Task WriteError(HttpContext ctx, HttpStatusCode code, string message)
    {
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(
            JsonSerializer.Serialize(new { error = message }));
    }
}
