using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question version management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionVersionsController : ControllerBase
{
    private readonly IQuestionVersionService _questionVersionService;
    private readonly ILogger<QuestionVersionsController> _logger;

    public QuestionVersionsController(
        IQuestionVersionService questionVersionService,
        ILogger<QuestionVersionsController> logger)
    {
        _questionVersionService = questionVersionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question versions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetAllQuestionVersions()
    {
        try
        {
            var versions = await _questionVersionService.GetAllAsync();
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question versions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question versions");
        }
    }

    /// <summary>
    /// Get question version by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> GetQuestionVersion(int id)
    {
        try
        {
            var version = await _questionVersionService.GetByIdAsync(id);
            if (version == null)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question version");
        }
    }

    /// <summary>
    /// Get question versions by question ID
    /// </summary>
    [HttpGet("question/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetQuestionVersionsByQuestion(int questionId)
    {
        try
        {
            var versions = await _questionVersionService.GetByQuestionIdAsync(questionId);
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question versions for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question versions");
        }
    }

    /// <summary>
    /// Get current active version of a question
    /// </summary>
    [HttpGet("question/{questionId}/current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> GetCurrentQuestionVersion(int questionId)
    {
        try
        {
            var version = await _questionVersionService.GetCurrentVersionAsync(questionId);
            if (version == null)
            {
                return NotFound($"No active version found for question {questionId}");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current version for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the current version");
        }
    }

    /// <summary>
    /// Create a new question version
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> CreateQuestionVersion([FromBody] CreateQuestionVersionDto createDto)
    {
        try
        {
            var version = await _questionVersionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionVersion), new { id = version.Id }, version);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question version");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question version");
        }
    }

    /// <summary>
    /// Update an existing question version
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> UpdateQuestionVersion(int id, [FromBody] UpdateQuestionVersionDto updateDto)
    {
        try
        {
            var version = await _questionVersionService.UpdateAsync(id, updateDto);
            if (version == null)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return Ok(version);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question version");
        }
    }

    /// <summary>
    /// Delete a question version
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionVersion(int id)
    {
        try
        {
            var result = await _questionVersionService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question version");
        }
    }

    /// <summary>
    /// Activate a question version
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> ActivateQuestionVersion(int id)
    {
        try
        {
            var version = await _questionVersionService.ActivateAsync(id);
            if (version == null)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while activating the question version");
        }
    }

    /// <summary>
    /// Publish a question version
    /// </summary>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> PublishQuestionVersion(int id)
    {
        try
        {
            var version = await _questionVersionService.PublishAsync(id);
            if (version == null)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while publishing the question version");
        }
    }

    /// <summary>
    /// Archive a question version
    /// </summary>
    [HttpPost("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> ArchiveQuestionVersion(int id)
    {
        try
        {
            var version = await _questionVersionService.ArchiveAsync(id);
            if (version == null)
            {
                return NotFound($"Question version with ID {id} not found");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving question version {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while archiving the question version");
        }
    }

    /// <summary>
    /// Get published question versions
    /// </summary>
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetPublishedQuestionVersions()
    {
        try
        {
            var versions = await _questionVersionService.GetPublishedAsync();
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving published question versions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question versions");
        }
    }

    /// <summary>
    /// Get archived question versions
    /// </summary>
    [HttpGet("archived")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetArchivedQuestionVersions()
    {
        try
        {
            var versions = await _questionVersionService.GetArchivedAsync();
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving archived question versions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question versions");
        }
    }

    /// <summary>
    /// Get question version history
    /// </summary>
    [HttpGet("question/{questionId}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionVersionDto>>> GetQuestionVersionHistory(int questionId)
    {
        try
        {
            var versions = await _questionVersionService.GetHistoryAsync(questionId);
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving version history for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving version history");
        }
    }

    /// <summary>
    /// Compare two question versions
    /// </summary>
    [HttpGet("compare")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> CompareQuestionVersions([FromQuery] int version1Id, [FromQuery] int version2Id)
    {
        try
        {
            if (version1Id == version2Id)
            {
                return BadRequest("Cannot compare a version with itself");
            }

            var comparison = await _questionVersionService.CompareAsync(version1Id, version2Id);
            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing question versions {Version1Id} and {Version2Id}", version1Id, version2Id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while comparing question versions");
        }
    }

    /// <summary>
    /// Get question version statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionVersionStatistics()
    {
        try
        {
            var statistics = await _questionVersionService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question version statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Revert to a previous question version
    /// </summary>
    [HttpPost("question/{questionId}/revert/{versionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionVersionDto>> RevertToQuestionVersion(int questionId, int versionId)
    {
        try
        {
            var version = await _questionVersionService.RevertToVersionAsync(questionId, versionId);
            if (version == null)
            {
                return NotFound($"Question or version not found");
            }
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting question {QuestionId} to version {VersionId}", questionId, versionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while reverting to the previous version");
        }
    }
}
