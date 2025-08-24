using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using System.Security.Claims;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for managing questions in the question bank
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(
        IQuestionService questionService,
        ILogger<QuestionsController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all questions with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> GetQuestions([FromQuery] QuestionFilterDto filter)
    {
        try
        {
            var questions = await _questionService.GetQuestionsAsync(filter);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving questions");
        }
    }

    /// <summary>
    /// Get a specific question by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        try
        {
            var question = await _questionService.GetQuestionAsync(id);
            return Ok(question);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question");
        }
    }

    /// <summary>
    /// Create a new question
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createDto)
    {
        try
        {
            // Set the creator ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            createDto.CreatedBy = userId;

            var question = await _questionService.CreateQuestionAsync(createDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionId }, question);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question");
        }
    }

    /// <summary>
    /// Update an existing question
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, UpdateQuestionDto updateDto)
    {
        try
        {
            // Set the updater ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            updateDto.UpdatedBy = userId;

            var question = await _questionService.UpdateQuestionAsync(id, updateDto);
            return Ok(question);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question");
        }
    }

    /// <summary>
    /// Delete a question
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteQuestion(int id)
    {
        try
        {
            var result = await _questionService.DeleteQuestionAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question");
        }
    }

    /// <summary>
    /// Archive a question
    /// </summary>
    [HttpPost("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> ArchiveQuestion(int id)
    {
        try
        {
            var result = await _questionService.ArchiveQuestionAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while archiving the question");
        }
    }

    /// <summary>
    /// Validate a question
    /// </summary>
    [HttpPost("{id}/validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestion(int id, ValidateQuestionDto validateDto)
    {
        try
        {
            var result = await _questionService.ValidateQuestionAsync(validateDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating the question");
        }
    }

    /// <summary>
    /// Publish a question
    /// </summary>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> PublishQuestion(int id)
    {
        try
        {
            var result = await _questionService.PublishQuestionAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while publishing the question");
        }
    }

    /// <summary>
    /// Unpublish a question
    /// </summary>
    [HttpPost("{id}/unpublish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> UnpublishQuestion(int id)
    {
        try
        {
            var result = await _questionService.UnpublishQuestionAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while unpublishing the question");
        }
    }

    /// <summary>
    /// Duplicate a question
    /// </summary>
    [HttpPost("{id}/duplicate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> DuplicateQuestion(int id, DuplicateQuestionDto duplicateDto)
    {
        try
        {
            var question = await _questionService.DuplicateQuestionAsync(id, duplicateDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionId }, question);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while duplicating the question");
        }
    }

    /// <summary>
    /// Get question versions
    /// </summary>
    [HttpGet("{id}/versions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetQuestionVersions(int id)
    {
        try
        {
            var versions = await _questionService.GetQuestionVersionsAsync(id);
            return Ok(versions);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving versions for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question versions");
        }
    }

    /// <summary>
    /// Create a new question version
    /// </summary>
    [HttpPost("{id}/versions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDto>> CreateQuestionVersion(int id, CreateVersionDto createVersionDto)
    {
        try
        {
            var question = await _questionService.CreateQuestionVersionAsync(id, createVersionDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionId }, question);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating version for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question version");
        }
    }

    /// <summary>
    /// Restore a question version
    /// </summary>
    [HttpPost("{id}/versions/{version}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> RestoreQuestionVersion(int id, string version)
    {
        try
        {
            var result = await _questionService.RestoreQuestionVersionAsync(id, version);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question version {version} for question {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring version {Version} for question {QuestionId}", version, id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while restoring the question version");
        }
    }

    /// <summary>
    /// Get related questions
    /// </summary>
    [HttpGet("{id}/related")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetRelatedQuestions(int id)
    {
        try
        {
            var questions = await _questionService.GetRelatedQuestionsAsync(id);
            return Ok(questions);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving related questions for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving related questions");
        }
    }

    /// <summary>
    /// Link questions
    /// </summary>
    [HttpPost("{id}/link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> LinkQuestions(int id, [FromBody] LinkQuestionsDto linkDto)
    {
        try
        {
            var result = await _questionService.LinkQuestionsAsync(id, linkDto.TargetQuestionId, linkDto.RelationshipType);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking questions {SourceQuestionId} and {TargetQuestionId}", id, linkDto.TargetQuestionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while linking the questions");
        }
    }

    /// <summary>
    /// Unlink questions
    /// </summary>
    [HttpDelete("{id}/link/{targetId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> UnlinkQuestions(int id, int targetId)
    {
        try
        {
            var result = await _questionService.UnlinkQuestionsAsync(id, targetId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking questions {SourceQuestionId} and {TargetQuestionId}", id, targetId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while unlinking the questions");
        }
    }

    /// <summary>
    /// Attach media to question
    /// </summary>
    [HttpPost("{id}/media/{mediaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> AttachMedia(int id, int mediaId)
    {
        try
        {
            var result = await _questionService.AttachMediaToQuestionAsync(id, mediaId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error attaching media {MediaId} to question {QuestionId}", mediaId, id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while attaching media to the question");
        }
    }

    /// <summary>
    /// Detach media from question
    /// </summary>
    [HttpDelete("{id}/media/{mediaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DetachMedia(int id, int mediaId)
    {
        try
        {
            var result = await _questionService.DetachMediaFromQuestionAsync(id, mediaId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detaching media {MediaId} from question {QuestionId}", mediaId, id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while detaching media from the question");
        }
    }

    /// <summary>
    /// Get question media
    /// </summary>
    [HttpGet("{id}/media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetQuestionMedia(int id)
    {
        try
        {
            var media = await _questionService.GetQuestionMediaAsync(id);
            return Ok(media);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving media for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question media");
        }
    }

    /// <summary>
    /// Get question usage statistics
    /// </summary>
    [HttpGet("{id}/usage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionUsageStatisticsDto>> GetQuestionUsage(int id)
    {
        try
        {
            var usage = await _questionService.GetQuestionUsageAsync(id);
            return Ok(usage);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usage statistics for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question usage statistics");
        }
    }

    /// <summary>
    /// Get question performance data
    /// </summary>
    [HttpGet("{id}/performance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionPerformanceDto>> GetQuestionPerformance(int id)
    {
        try
        {
            var performance = await _questionService.GetQuestionPerformanceAsync(id);
            return Ok(performance);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance data for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question performance data");
        }
    }

    /// <summary>
    /// Get similar questions
    /// </summary>
    [HttpGet("{id}/similar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetSimilarQuestions(int id)
    {
        try
        {
            var questions = await _questionService.GetSimilarQuestionsAsync(id);
            return Ok(questions);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving similar questions for question {QuestionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving similar questions");
        }
    }

    /// <summary>
    /// Bulk create questions
    /// </summary>
    [HttpPost("bulk/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkOperationResult>> BulkCreateQuestions([FromBody] IEnumerable<CreateQuestionDto> questions)
    {
        try
        {
            var result = await _questionService.BulkCreateQuestionsAsync(questions);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk creating questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while bulk creating questions");
        }
    }

    /// <summary>
    /// Bulk update questions
    /// </summary>
    [HttpPut("bulk/update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkOperationResult>> BulkUpdateQuestions([FromBody] BulkUpdateQuestionsDto updateDto)
    {
        try
        {
            var result = await _questionService.BulkUpdateQuestionsAsync(updateDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while bulk updating questions");
        }
    }

    /// <summary>
    /// Bulk delete questions
    /// </summary>
    [HttpDelete("bulk/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkOperationResult>> BulkDeleteQuestions([FromBody] IEnumerable<int> questionIds)
    {
        try
        {
            var result = await _questionService.BulkDeleteQuestionsAsync(questionIds);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while bulk deleting questions");
        }
    }

    /// <summary>
    /// Bulk tag questions
    /// </summary>
    [HttpPost("bulk/tag")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkOperationResult>> BulkTagQuestions([FromBody] BulkTagDto tagDto)
    {
        try
        {
            var result = await _questionService.BulkTagQuestionsAsync(tagDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk tagging questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while bulk tagging questions");
        }
    }
}

/// <summary>
/// DTO for linking questions
/// </summary>
public class LinkQuestionsDto
{
    public int TargetQuestionId { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
}
