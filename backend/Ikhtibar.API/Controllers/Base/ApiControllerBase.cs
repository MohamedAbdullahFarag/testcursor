using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers.Base;

/// <summary>
/// Base controller for all API controllers in the Ikhtibar application
/// Provides common functionality and consistent routing patterns
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the current user ID from the JWT token claims
    /// </summary>
    protected int? CurrentUserId
    {
        get
        {
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == "sub" || c.Type == "userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Gets the current user's email from the JWT token claims
    /// </summary>
    protected string? CurrentUserEmail
    {
        get
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == "email")?.Value;
        }
    }

    /// <summary>
    /// Creates a standardized success response with data
    /// </summary>
    /// <typeparam name="T">Type of the response data</typeparam>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A standardized API response</returns>
    protected IActionResult SuccessResponse<T>(T data, string message = "Success")
    {
        return Ok(new
        {
            success = true,
            message = message,
            data = data,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>A standardized API error response</returns>
    protected IActionResult ErrorResponse(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, new
        {
            success = false,
            message = message,
            data = (object?)null,
            timestamp = DateTime.UtcNow
        });
    }
}
