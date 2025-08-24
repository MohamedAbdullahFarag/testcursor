using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question creation workflow management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionCreationWorkflowController : ControllerBase
{
    private readonly IQuestionCreationWorkflowService _questionCreationWorkflowService;
    private readonly ILogger<QuestionCreationWorkflowController> _logger;

    public QuestionCreationWorkflowController(
        IQuestionCreationWorkflowService questionCreationWorkflowService,
        ILogger<QuestionCreationWorkflowController> logger)
    {
        _questionCreationWorkflowService = questionCreationWorkflowService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question creation workflows
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionCreationWorkflowDto>>> GetAllQuestionCreationWorkflows()
    {
        try
        {
            var workflows = await _questionCreationWorkflowService.GetAllAsync();
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflows");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question creation workflows");
        }
    }

    /// <summary>
    /// Get question creation workflow by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> GetQuestionCreationWorkflow(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.GetByIdAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question creation workflow");
        }
    }

    /// <summary>
    /// Get question creation workflows by creator ID
    /// </summary>
    [HttpGet("creator/{creatorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionCreationWorkflowDto>>> GetQuestionCreationWorkflowsByCreator(int creatorId)
    {
        try
        {
            var workflows = await _questionCreationWorkflowService.GetByCreatorIdAsync(creatorId);
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflows for creator {CreatorId}", creatorId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question creation workflows");
        }
    }

    /// <summary>
    /// Get question creation workflows by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionCreationWorkflowDto>>> GetQuestionCreationWorkflowsByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<WorkflowStatus>(status, true, out var workflowStatus))
            {
                return BadRequest($"Invalid workflow status: {status}");
            }

            var workflows = await _questionCreationWorkflowService.GetByStatusAsync(workflowStatus);
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflows with status {Status}", status);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question creation workflows");
        }
    }

    /// <summary>
    /// Get active question creation workflows
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionCreationWorkflowDto>>> GetActiveQuestionCreationWorkflows()
    {
        try
        {
            var workflows = await _questionCreationWorkflowService.GetActiveAsync();
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active question creation workflows");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving active workflows");
        }
    }

    /// <summary>
    /// Create a new question creation workflow
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> CreateQuestionCreationWorkflow([FromBody] CreateQuestionCreationWorkflowDto createDto)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionCreationWorkflow), new { id = workflow.Id }, workflow);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question creation workflow");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question creation workflow");
        }
    }

    /// <summary>
    /// Update an existing question creation workflow
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> UpdateQuestionCreationWorkflow(int id, [FromBody] UpdateQuestionCreationWorkflowDto updateDto)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.UpdateAsync(id, updateDto);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question creation workflow");
        }
    }

    /// <summary>
    /// Delete a question creation workflow
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionCreationWorkflow(int id)
    {
        try
        {
            var result = await _questionCreationWorkflowService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question creation workflow");
        }
    }

    /// <summary>
    /// Start a question creation workflow
    /// </summary>
    [HttpPost("{id}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> StartQuestionCreationWorkflow(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.StartAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while starting the question creation workflow");
        }
    }

    /// <summary>
    /// Pause a question creation workflow
    /// </summary>
    [HttpPost("{id}/pause")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> PauseQuestionCreationWorkflow(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.PauseAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while pausing the question creation workflow");
        }
    }

    /// <summary>
    /// Resume a question creation workflow
    /// </summary>
    [HttpPost("{id}/resume")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> ResumeQuestionCreationWorkflow(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.ResumeAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while resuming the question creation workflow");
        }
    }

    /// <summary>
    /// Complete a question creation workflow
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> CompleteQuestionCreationWorkflow(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.CompleteAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while completing the question creation workflow");
        }
    }

    /// <summary>
    /// Cancel a question creation workflow
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> CancelQuestionCreationWorkflow(int id, [FromBody] CancelWorkflowDto cancelDto)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.CancelAsync(id, cancelDto);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while canceling the question creation workflow");
        }
    }

    /// <summary>
    /// Move to next step in question creation workflow
    /// </summary>
    [HttpPost("{id}/next-step")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> MoveToNextStep(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.MoveToNextStepAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving to next step in question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while moving to the next step");
        }
    }

    /// <summary>
    /// Move to previous step in question creation workflow
    /// </summary>
    [HttpPost("{id}/previous-step")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCreationWorkflowDto>> MoveToPreviousStep(int id)
    {
        try
        {
            var workflow = await _questionCreationWorkflowService.MoveToPreviousStepAsync(id);
            if (workflow == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving to previous step in question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while moving to the previous step");
        }
    }

    /// <summary>
    /// Get question creation workflow steps
    /// </summary>
    [HttpGet("{id}/steps")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<object>>> GetQuestionCreationWorkflowSteps(int id)
    {
        try
        {
            var steps = await _questionCreationWorkflowService.GetStepsAsync(id);
            if (steps == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(steps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving steps for question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the steps");
        }
    }

    /// <summary>
    /// Get question creation workflow current step
    /// </summary>
    [HttpGet("{id}/current-step")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionCreationWorkflowCurrentStep(int id)
    {
        try
        {
            var step = await _questionCreationWorkflowService.GetCurrentStepAsync(id);
            if (step == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(step);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current step for question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the current step");
        }
    }

    /// <summary>
    /// Get question creation workflow progress
    /// </summary>
    [HttpGet("{id}/progress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionCreationWorkflowProgress(int id)
    {
        try
        {
            var progress = await _questionCreationWorkflowService.GetProgressAsync(id);
            if (progress == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving progress for question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the progress");
        }
    }

    /// <summary>
    /// Get question creation workflow timeline
    /// </summary>
    [HttpGet("{id}/timeline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<object>>> GetQuestionCreationWorkflowTimeline(int id)
    {
        try
        {
            var timeline = await _questionCreationWorkflowService.GetTimelineAsync(id);
            if (timeline == null)
            {
                return NotFound($"Question creation workflow with ID {id} not found");
            }
            return Ok(timeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timeline for question creation workflow {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the timeline");
        }
    }

    /// <summary>
    /// Get question creation workflow statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionCreationWorkflowStatistics()
    {
        try
        {
            var statistics = await _questionCreationWorkflowService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflow statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Get question creation workflow dashboard
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionCreationWorkflowDashboard()
    {
        try
        {
            var dashboard = await _questionCreationWorkflowService.GetDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflow dashboard");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving dashboard data");
        }
    }

    /// <summary>
    /// Get question creation workflow reports
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionCreationWorkflowReports([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var reports = await _questionCreationWorkflowService.GetReportsAsync(startDate, endDate);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question creation workflow reports");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving reports");
        }
    }
}
