using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Middleware;

/// <summary>
/// Global error handling middleware that converts exceptions to standardized ProblemDetails responses
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Processes the HTTP request and handles any exceptions that occur
    /// </summary>
    /// <param name="context">The HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions by converting them to appropriate ProblemDetails responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception that occurred</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = CreateProblemDetails(context, exception);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Creates a ProblemDetails object based on the exception type
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A ProblemDetails object with appropriate status code and details</returns>
    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case ArgumentException:
                problemDetails.Title = "Bad Request";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = _environment.IsDevelopment() ? exception.Message : "Invalid request data";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                problemDetails.Title = "Unauthorized";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Detail = "You are not authorized to access this resource";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                problemDetails.Title = "Not Found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = _environment.IsDevelopment() ? exception.Message : "The requested resource was not found";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case TimeoutException:
                problemDetails.Title = "Request Timeout";
                problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                problemDetails.Detail = "The request timed out";
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        // Add exception details only in development
        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return problemDetails;
    }
}
