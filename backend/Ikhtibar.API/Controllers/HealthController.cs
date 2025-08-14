using Ikhtibar.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Health check controller for monitoring API status
/// </summary>
public class HealthController : ApiControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple ping endpoint to verify API is responding
    /// </summary>
    /// <returns>200 OK with basic status information</returns>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        _logger.LogInformation("Health ping endpoint called");

        return SuccessResponse(new
        {
            Status = "Healthy",
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            ServerTime = DateTime.UtcNow,
            Uptime = Environment.TickCount64
        }, "API is running successfully");
    }

    /// <summary>
    /// Detailed health check endpoint
    /// </summary>
    /// <returns>200 OK with detailed health information</returns>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Status()
    {
        _logger.LogInformation("Health status endpoint called");

        try
        {
            var healthData = new
            {
                Status = "Healthy",
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                ServerTime = DateTime.UtcNow,
                Machine = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = Environment.WorkingSet,
                GCMemory = GC.GetTotalMemory(false),
                Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64)
            };

            return SuccessResponse(healthData, "API is healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                Success = false,
                Message = "Service Unavailable",
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
