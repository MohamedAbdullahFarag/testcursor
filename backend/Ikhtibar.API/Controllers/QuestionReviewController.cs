using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question review workflow management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionReviewController : ControllerBase
{
    private readonly IQuestionReviewService _questionReviewService;
    private readonly ILogger<QuestionReviewController> _logger;

    public QuestionReviewController(
        IQuestionReviewService questionReviewService,
        ILogger<QuestionReviewController> logger)
    {
        _questionReviewService = questionReviewService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question reviews
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionReviewDto>>> GetAllQuestionReviews()
    {
        try
        {
            var reviews = await _questionReviewService.GetAllAsync();
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question reviews");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question reviews");
        }
    }

    /// <summary>
    /// Get question review by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> GetQuestionReview(int id)
    {
        try
        {
            var review = await _questionReviewService.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question review");
        }
    }

    /// <summary>
    /// Get question reviews by question ID
    /// </summary>
    [HttpGet("question/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionReviewDto>>> GetQuestionReviewsByQuestion(int questionId)
    {
        try
        {
            var reviews = await _questionReviewService.GetByQuestionIdAsync(questionId);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question reviews for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question reviews");
        }
    }

    /// <summary>
    /// Get question reviews by reviewer ID
    /// </summary>
    [HttpGet("reviewer/{reviewerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionReviewDto>>> GetQuestionReviewsByReviewer(int reviewerId)
    {
        try
        {
            var reviews = await _questionReviewService.GetByReviewerIdAsync(reviewerId);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question reviews for reviewer {ReviewerId}", reviewerId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question reviews");
        }
    }

    /// <summary>
    /// Get pending question reviews
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionReviewDto>>> GetPendingQuestionReviews()
    {
        try
        {
            var reviews = await _questionReviewService.GetPendingAsync();
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending question reviews");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving pending reviews");
        }
    }

    /// <summary>
    /// Get question reviews by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionReviewDto>>> GetQuestionReviewsByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<ReviewStatus>(status, true, out var reviewStatus))
            {
                return BadRequest($"Invalid review status: {status}");
            }

            var reviews = await _questionReviewService.GetByStatusAsync(reviewStatus);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question reviews with status {Status}", status);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question reviews");
        }
    }

    /// <summary>
    /// Create a new question review
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> CreateQuestionReview([FromBody] CreateQuestionReviewDto createDto)
    {
        try
        {
            var review = await _questionReviewService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionReview), new { id = review.Id }, review);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question review");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question review");
        }
    }

    /// <summary>
    /// Update an existing question review
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> UpdateQuestionReview(int id, [FromBody] UpdateQuestionReviewDto updateDto)
    {
        try
        {
            var review = await _questionReviewService.UpdateAsync(id, updateDto);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question review");
        }
    }

    /// <summary>
    /// Delete a question review
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionReview(int id)
    {
        try
        {
            var result = await _questionReviewService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question review");
        }
    }

    /// <summary>
    /// Submit a question review
    /// </summary>
    [HttpPost("{id}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> SubmitQuestionReview(int id)
    {
        try
        {
            var review = await _questionReviewService.SubmitAsync(id);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while submitting the question review");
        }
    }

    /// <summary>
    /// Approve a question review
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> ApproveQuestionReview(int id)
    {
        try
        {
            var review = await _questionReviewService.ApproveAsync(id);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the question review");
        }
    }

    /// <summary>
    /// Reject a question review
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> RejectQuestionReview(int id, [FromBody] RejectQuestionReviewDto rejectDto)
    {
        try
        {
            var review = await _questionReviewService.RejectAsync(id, rejectDto);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the question review");
        }
    }

    /// <summary>
    /// Request revision for a question review
    /// </summary>
    [HttpPost("{id}/request-revision")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> RequestRevision(int id, [FromBody] RequestRevisionDto requestDto)
    {
        try
        {
            var review = await _questionReviewService.RequestRevisionAsync(id, requestDto);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting revision for question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while requesting revision");
        }
    }

    /// <summary>
    /// Assign a reviewer to a question review
    /// </summary>
    [HttpPost("{id}/assign-reviewer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionReviewDto>> AssignReviewer(int id, [FromBody] AssignReviewerDto assignDto)
    {
        try
        {
            var review = await _questionReviewService.AssignReviewerAsync(id, assignDto);
            if (review == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning reviewer to question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while assigning reviewer");
        }
    }

    /// <summary>
    /// Get question review workflow
    /// </summary>
    [HttpGet("{id}/workflow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionReviewWorkflow(int id)
    {
        try
        {
            var workflow = await _questionReviewService.GetWorkflowAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow for question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the workflow");
        }
    }

    /// <summary>
    /// Get question review statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionReviewStatistics()
    {
        try
        {
            var statistics = await _questionReviewService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question review statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Get question review timeline
    /// </summary>
    [HttpGet("{id}/timeline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<object>>> GetQuestionReviewTimeline(int id)
    {
        try
        {
            var timeline = await _questionReviewService.GetTimelineAsync(id);
            if (timeline == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(timeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timeline for question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the timeline");
        }
    }

    /// <summary>
    /// Add comment to question review
    /// </summary>
    [HttpPost("{id}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> AddComment(int id, [FromBody] AddReviewCommentDto commentDto)
    {
        try
        {
            var comment = await _questionReviewService.AddCommentAsync(id, commentDto);
            if (comment == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return CreatedAtAction(nameof(GetQuestionReview), new { id }, comment);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the comment");
        }
    }

    /// <summary>
    /// Get question review comments
    /// </summary>
    [HttpGet("{id}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<object>>> GetComments(int id)
    {
        try
        {
            var comments = await _questionReviewService.GetCommentsAsync(id);
            if (comments == null)
            {
                return NotFound($"Question review with ID {id} not found");
            }
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for question review {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving comments");
        }
    }

    /// <summary>
    /// Get question review dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionReviewDashboard()
    {
        try
        {
            var dashboard = await _questionReviewService.GetDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question review dashboard");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving dashboard data");
        }
    }

    /// <summary>
    /// Get question review reports
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionReviewReports([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var reports = await _questionReviewService.GetReportsAsync(startDate, endDate);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question review reports");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving reports");
        }
    }
}
