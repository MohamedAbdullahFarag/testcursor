using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.Services;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for testing audit functionality
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuditTestController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditTestController> _logger;

    public AuditTestController(IAuditService auditService, ILogger<AuditTestController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// Test endpoint to demonstrate API_GET audit logging
    /// This endpoint will be automatically logged by AuditMiddleware
    /// </summary>
    /// <returns>Test response with audit information</returns>
    [HttpGet("test-get-endpoint")]
    [AllowAnonymous]
    public async Task<IActionResult> TestGetEndpoint()
    {
        try
        {
            _logger.LogInformation("Test GET endpoint called - this will be captured by audit middleware");
            
            // Optional: Add a specific audit log with entity information
            await _auditService.LogSystemActionAsync(
                "TEST_ENDPOINT_ACCESS",
                "Test endpoint accessed for audit demonstration",
                "TestEndpoint",
                "test-get-endpoint");
            
            return Ok(new
            {
                message = "This GET request will be automatically logged as 'API_GET' by the audit middleware",
                timestamp = DateTime.UtcNow,
                auditInfo = new
                {
                    action = "API_GET",
                    category = "DataAccess",
                    severity = "Low",
                    automaticallyLogged = true,
                    additionalAudit = "TEST_ENDPOINT_ACCESS with EntityId"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test GET endpoint");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Test endpoint to manually create a specific audit log entry
    /// </summary>
    /// <returns>Manual audit log result</returns>
    [HttpPost("create-manual-audit")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateManualAuditLog()
    {
        try
        {
            // Create a manual audit log entry for demonstration
            var auditId = await _auditService.LogSystemActionAsync(
                "MANUAL_API_GET",
                "Manually created audit log for API GET demonstration",
                "TestEntity",
                "test-123");

            _logger.LogInformation("Manual audit log created with ID: {AuditId}", auditId);
            
            return Ok(new
            {
                message = "Manual audit log created successfully",
                auditLogId = auditId,
                action = "MANUAL_API_GET"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating manual audit log");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Test endpoint with authentication to show user-specific audit logging
    /// </summary>
    /// <returns>Authenticated test response</returns>
    [HttpGet("test-authenticated-get")]
    [Authorize]
    public IActionResult TestAuthenticatedGetEndpoint()
    {
        try
        {
            var userId = User?.Identity?.Name;
            _logger.LogInformation("Authenticated GET endpoint called by user: {UserId}", userId);
            
            // The audit middleware will automatically capture this with user information
            
            return Ok(new
            {
                message = "This authenticated GET request will be logged with user information",
                userId = userId,
                timestamp = DateTime.UtcNow,
                auditInfo = new
                {
                    action = "API_GET",
                    category = "DataAccess",
                    severity = "Low",
                    authenticatedUser = true,
                    automaticallyLogged = true
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in authenticated test GET endpoint");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Enhanced audit logging with custom details
    /// </summary>
    /// <param name="entityId">Entity ID for demonstration</param>
    /// <returns>Enhanced audit response</returns>
    [HttpGet("enhanced-audit/{entityId}")]
    [AllowAnonymous]
    public async Task<IActionResult> EnhancedAuditDemo(string entityId)
    {
        try
        {
            // Before the middleware logs the API call, we can add additional context
            using (_logger.BeginScope("Enhanced audit demo for entity {EntityId}", entityId))
            {
                _logger.LogInformation("Processing enhanced audit demo request");

                // Manually log additional context if needed
                await _auditService.LogSystemActionAsync(
                    "ENHANCED_API_GET",
                    $"Enhanced GET request for entity {entityId} with additional context",
                    "DemoEntity",
                    entityId);

                return Ok(new
                {
                    message = "Enhanced audit logging demonstration",
                    entityId = entityId,
                    timestamp = DateTime.UtcNow,
                    auditInfo = new
                    {
                        primaryAction = "API_GET (from middleware)",
                        enhancedAction = "ENHANCED_API_GET (manual)",
                        entityType = "DemoEntity",
                        entityId = entityId,
                        dualLogging = true
                    }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in enhanced audit demo");
            return StatusCode(500, "Internal server error");
        }
    }
}
