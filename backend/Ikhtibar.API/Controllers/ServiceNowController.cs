using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// ServiceNow integration controller - provides mock endpoints for support functionality
/// </summary>
[ApiController]
[Route("api/serviceNow/v1")]
[Authorize]
public class ServiceNowController : ControllerBase
{
    private readonly ILogger<ServiceNowController> _logger;

    public ServiceNowController(ILogger<ServiceNowController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get support categories
    /// </summary>
    [HttpGet("categories")]
    public ActionResult GetCategories([FromQuery] string? source = null, [FromQuery] string? lang = "en", [FromQuery] int? roles = null)
    {
        _logger.LogInformation("ServiceNow categories requested - source: {Source}, lang: {Lang}, roles: {Roles}", source, lang, roles);
        
        return Ok(new
        {
            data = new[]
            {
                new { id = "1", name = "Technical Support", description = "Technical issues and support" },
                new { id = "2", name = "Account Management", description = "Account related questions" },
                new { id = "3", name = "General Inquiry", description = "General questions and information" }
            },
            success = true
        });
    }

    /// <summary>
    /// Get support subcategories
    /// </summary>
    [HttpGet("subCategories/{categoryId}")]
    public ActionResult GetSubCategories(string categoryId, [FromQuery] string? source = null, [FromQuery] string? lang = "en", [FromQuery] int? roles = null)
    {
        _logger.LogInformation("ServiceNow subcategories requested for category: {CategoryId}", categoryId);
        
        return Ok(new
        {
            data = new[]
            {
                new { id = "1", name = "Login Issues", description = "Problems with logging in" },
                new { id = "2", name = "Performance Issues", description = "System performance problems" }
            },
            success = true
        });
    }

    /// <summary>
    /// Get cases by user ID
    /// </summary>
    [HttpGet("cases")]
    public ActionResult GetCases([FromQuery] string? source = null, [FromQuery] string? userId = null, [FromQuery] int limit = 10, [FromQuery] int offset = 0, [FromQuery] string? email = null)
    {
        _logger.LogInformation("ServiceNow cases requested - userId: {UserId}, email: {Email}, limit: {Limit}, offset: {Offset}", userId, email, limit, offset);
        
        return Ok(new
        {
            data = new object[0], // Empty array for now
            metadata = new
            {
                resultset = new
                {
                    count = 0
                }
            },
            success = true
        });
    }

    /// <summary>
    /// Get case by case number
    /// </summary>
    [HttpGet("cases/{caseNum}")]
    public ActionResult GetCaseByCaseNum(string caseNum, [FromQuery] string? source = null, [FromQuery] string? userId = null)
    {
        _logger.LogInformation("ServiceNow case requested - caseNum: {CaseNum}, userId: {UserId}", caseNum, userId);
        
        return NotFound(new { message = "Case not found", success = false });
    }

    /// <summary>
    /// Submit a new ticket/case
    /// </summary>
    [HttpPost("cases")]
    [HttpPost("loggedInUserCases")]
    public ActionResult SubmitTicket([FromBody] object ticketData, [FromQuery] string? source = null)
    {
        _logger.LogInformation("ServiceNow ticket submission requested");
        
        return Ok(new
        {
            data = new
            {
                caseId = "CASE001",
                caseNum = "INC0001001",
                status = "Open"
            },
            success = true
        });
    }

    /// <summary>
    /// Add attachment to case
    /// </summary>
    [HttpPost("cases/{caseId}/attachments")]
    public ActionResult AddAttachment(string caseId, [FromBody] object attachmentData, [FromQuery] string? source = null)
    {
        _logger.LogInformation("ServiceNow attachment upload requested for case: {CaseId}", caseId);
        
        return Ok(new
        {
            data = new
            {
                attachmentId = "ATT001",
                fileName = "attachment.pdf"
            },
            success = true
        });
    }

    /// <summary>
    /// Get attachments for case
    /// </summary>
    [HttpGet("cases/{caseId}/attachments")]
    public ActionResult GetAttachments(string caseId, [FromQuery] string? source = null)
    {
        _logger.LogInformation("ServiceNow attachments requested for case: {CaseId}", caseId);
        
        return Ok(new
        {
            data = new object[0],
            success = true
        });
    }

    /// <summary>
    /// Get case comments
    /// </summary>
    [HttpGet("cases/{caseId}/comments")]
    public ActionResult GetComments(string caseId, [FromQuery] string? source = null, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        _logger.LogInformation("ServiceNow comments requested for case: {CaseId}", caseId);
        
        return Ok(new
        {
            data = new object[0],
            success = true
        });
    }

    /// <summary>
    /// Add comment to case
    /// </summary>
    [HttpPost("cases/{caseId}/comment")]
    public ActionResult AddComment(string caseId, [FromBody] object commentData, [FromQuery] string? source = null)
    {
        _logger.LogInformation("ServiceNow comment addition requested for case: {CaseId}", caseId);
        
        return Ok(new
        {
            data = new
            {
                commentId = "COM001",
                message = "Comment added successfully"
            },
            success = true
        });
    }
}
