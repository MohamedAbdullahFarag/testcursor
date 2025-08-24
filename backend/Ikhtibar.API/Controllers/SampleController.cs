using Ikhtibar.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Sample controller demonstrating proper dependency injection patterns
/// and controller implementation standards for the Ikhtibar API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SampleController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<SampleController> _logger;

    /// <summary>
    /// Initializes a new instance of the SampleController
    /// </summary>
    /// <param name="userService">User service for demonstration</param>
    /// <param name="logger">Logger instance</param>
    public SampleController(
        IUserService userService,
        ILogger<SampleController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Demonstrates basic GET endpoint with dependency injection
    /// </summary>
    /// <returns>Sample response with service information</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get()
    {
        _logger.LogInformation("Sample controller GET endpoint called");

        try
        {
            var sampleData = new
            {
                Message = "Sample controller is working correctly",
                ServiceType = _userService.GetType().Name,
                Timestamp = DateTime.UtcNow,
                ControllerName = nameof(SampleController),
                BaseController = nameof(ApiControllerBase)
            };

            return SuccessResponse(sampleData, "Sample endpoint executed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in sample controller GET endpoint");
            return ErrorResponse("An error occurred while processing the request", 500);
        }
    }

    /// <summary>
    /// Demonstrates POST endpoint with model validation
    /// </summary>
    /// <param name="request">Sample request data</param>
    /// <returns>Processed request response</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Post([FromBody] SampleRequest request)
    {
        _logger.LogInformation("Sample controller POST endpoint called with data: {Data}", request?.Message);

        try
        {
            if (request == null)
            {
                return ErrorResponse("Request body cannot be null", 400);
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return ErrorResponse("Message cannot be empty", 400);
            }

            var response = new
            {
                ReceivedMessage = request.Message,
                ProcessedAt = DateTime.UtcNow,
                MessageLength = request.Message.Length,
                IsValid = true
            };

            return SuccessResponse(response, "Request processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in sample controller POST endpoint");
            return ErrorResponse("An error occurred while processing the request", 500);
        }
    }

    /// <summary>
    /// Demonstrates dependency injection with service method call
    /// </summary>
    /// <param name="userId">User ID to demonstrate service usage</param>
    /// <returns>User information from service</returns>
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserInfo(int userId)
    {
        _logger.LogInformation("Sample controller GetUserInfo called for user ID: {UserId}", userId);

        try
        {
            // Demonstrate service usage (this is just for demonstration)
            // In a real scenario, you would use the actual service method
            var userInfo = new
            {
                UserId = userId,
                ServiceAvailable = _userService != null,
                ServiceType = _userService.GetType().Name,
                RetrievedAt = DateTime.UtcNow
            };

            return SuccessResponse(userInfo, "User information retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in sample controller GetUserInfo endpoint for user ID: {UserId}", userId);
            return ErrorResponse("An error occurred while retrieving user information", 500);
        }
    }

    /// <summary>
    /// Demonstrates error handling and logging
    /// </summary>
    /// <param name="shouldError">Whether to simulate an error</param>
    /// <returns>Success response or simulated error</returns>
    [HttpGet("test-error-handling")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult TestErrorHandling([FromQuery] bool shouldError = false)
    {
        _logger.LogInformation("Sample controller TestErrorHandling called with shouldError: {ShouldError}", shouldError);

        if (shouldError)
        {
            _logger.LogWarning("Simulating error condition as requested");
            return ErrorResponse("This is a simulated error for testing purposes", 400);
        }

        var response = new
        {
            Message = "Error handling test completed successfully",
            Timestamp = DateTime.UtcNow,
            ErrorSimulated = false
        };

        return SuccessResponse(response, "Error handling test passed");
    }
}

/// <summary>
/// Sample request model for demonstration purposes
/// </summary>
public class SampleRequest
{
    /// <summary>
    /// Sample message to process
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 100 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional additional data
    /// </summary>
    public string? AdditionalData { get; set; }
}
