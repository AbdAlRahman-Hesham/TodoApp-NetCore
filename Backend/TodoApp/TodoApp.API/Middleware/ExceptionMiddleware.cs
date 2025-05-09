using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Application.Errors;

namespace TodoApp.API.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        context.Response.ContentType = "application/json";
        var response = new ApiError(500, "Internal Server Error");

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiError(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: "Validation error occurred",
                    errors: new Dictionary<string, string[]>
                    {
                        { "ValidationError", new[] { validationEx.Message } }
                    });
                break;

            case BadHttpRequestException badRequestEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiError(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: badRequestEx.Message);
                break;

            case KeyNotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ApiError(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: notFoundEx.Message);
                break;

            case UnauthorizedAccessException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new ApiError(
                    statusCode: (int)HttpStatusCode.Unauthorized,
                    message: unauthorizedEx.Message);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = _environment.IsDevelopment()
                    ? new ApiError(
                        statusCode: (int)HttpStatusCode.InternalServerError,
                        message: exception.Message,
                        details: exception.StackTrace)
                    : new ApiError(
                        statusCode: (int)HttpStatusCode.InternalServerError,
                        message: "An error occurred while processing your request");
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
