using System.Net;
using System.Text.Json;

namespace EmployeeStorage.API.Middlewares;

public class ExceptionHandingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandingMiddleware(RequestDelegate next, 
        ILogger<ExceptionHandingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(
                "{DateTime}: {Message}", 
                DateTime.Now,
                $"{ex.Message}\n{ex.Source}\n{ex.StackTrace}"
            );

            var result = JsonSerializer.Serialize(new { message = "Что-то пошло не так" });
            await response.WriteAsync(result);
        }
    }
}
