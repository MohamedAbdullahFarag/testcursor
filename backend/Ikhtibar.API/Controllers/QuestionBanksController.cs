using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using System.Security.Claims;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for managing question banks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionBanksController : ControllerBase
{
    private readonly IQuestionBankService _questionBankService;
    private readonly ILogger<QuestionBanksController> _logger;

    public QuestionBanksController(
        IQuestionBankService questionBankService,
        ILogger<QuestionBanksController> logger)
    {
        _questionBankService = questionBankService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question banks with filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionBankDto>>> GetQuestionBanks([FromQuery] QuestionBankFilterDto filter)
    {
        try
        {
            var questionBanks = await _questionBankService.GetQuestionBanksAsync(filter);
            return Ok(questionBanks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question banks");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question banks");
        }
    }

    /// <summary>
    /// Get a specific question bank by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankDto>> GetQuestionBank(int id)
    {
        try
        {
            var questionBank = await _questionBankService.GetQuestionBankAsync(id);
            return Ok(questionBank);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question bank");
        }
    }

    /// <summary>
    /// Create a new question bank
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankDto>> CreateQuestionBank(CreateQuestionBankDto createDto)
    {
        try
        {
            // Set the creator ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            createDto.CreatedBy = userId;

            var questionBank = await _questionBankService.CreateQuestionBankAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionBank), new { id = questionBank.Id }, questionBank);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question bank");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question bank");
        }
    }

    /// <summary>
    /// Update an existing question bank
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankDto>> UpdateQuestionBank(int id, UpdateQuestionBankDto updateDto)
    {
        try
        {
            var questionBank = await _questionBankService.UpdateQuestionBankAsync(id, updateDto);
            return Ok(questionBank);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question bank");
        }
    }

    /// <summary>
    /// Delete a question bank
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteQuestionBank(int id)
    {
        try
        {
            var result = await _questionBankService.DeleteQuestionBankAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question bank");
        }
    }

    /// <summary>
    /// Get question bank statistics
    /// </summary>
    [HttpGet("{id}/statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankStatisticsDto>> GetQuestionBankStatistics(int id)
    {
        try
        {
            var statistics = await _questionBankService.GetQuestionBankStatisticsAsync(id);
            return Ok(statistics);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question bank statistics");
        }
    }

    /// <summary>
    /// Get question bank analytics
    /// </summary>
    [HttpGet("{id}/analytics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankAnalyticsDto>> GetQuestionBankAnalytics(int id)
    {
        try
        {
            var analytics = await _questionBankService.GetQuestionBankAnalyticsAsync(id);
            return Ok(analytics);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics for question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question bank analytics");
        }
    }

    /// <summary>
    /// Get questions in a question bank
    /// </summary>
    [HttpGet("{id}/questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionBankQuestions(int id)
    {
        try
        {
            var questions = await _questionBankService.GetQuestionsInBankAsync(id);
            return Ok(questions);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions for question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question bank questions");
        }
    }

    /// <summary>
    /// Add questions to a question bank
    /// </summary>
    [HttpPost("{id}/questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> AddQuestionsToBank(int id, [FromBody] IEnumerable<int> questionIds)
    {
        try
        {
            var result = await _questionBankService.AddQuestionsToBankAsync(id, questionIds);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding questions to question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding questions to the question bank");
        }
    }

    /// <summary>
    /// Remove questions from a question bank
    /// </summary>
    [HttpDelete("{id}/questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> RemoveQuestionsFromBank(int id, [FromBody] IEnumerable<int> questionIds)
    {
        try
        {
            var result = await _questionBankService.RemoveQuestionsFromBankAsync(id, questionIds);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing questions from question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing questions from the question bank");
        }
    }

    /// <summary>
    /// Get question bank access permissions
    /// </summary>
    [HttpGet("{id}/access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionBankAccessDto>>> GetQuestionBankAccess(int id)
    {
        try
        {
            var access = await _questionBankService.GetQuestionBankAccessAsync(id);
            return Ok(access);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access for question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question bank access");
        }
    }

    /// <summary>
    /// Grant access to a question bank
    /// </summary>
    [HttpPost("{id}/access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> GrantQuestionBankAccess(int id, [FromBody] GrantQuestionBankAccessDto accessDto)
    {
        try
        {
            var result = await _questionBankService.GrantAccessAsync(id, accessDto);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting access to question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while granting access to the question bank");
        }
    }

    /// <summary>
    /// Revoke access from a question bank
    /// </summary>
    [HttpDelete("{id}/access/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> RevokeQuestionBankAccess(int id, int userId)
    {
        try
        {
            var result = await _questionBankService.RevokeAccessAsync(id, userId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank access not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking access from question bank {QuestionBankId} for user {UserId}", id, userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while revoking access from the question bank");
        }
    }

    /// <summary>
    /// Clone a question bank
    /// </summary>
    [HttpPost("{id}/clone")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankDto>> CloneQuestionBank(int id, [FromBody] CloneQuestionBankDto cloneDto)
    {
        try
        {
            var questionBank = await _questionBankService.CloneQuestionBankAsync(id, cloneDto);
            return CreatedAtAction(nameof(GetQuestionBank), new { id = questionBank.Id }, questionBank);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while cloning the question bank");
        }
    }

    /// <summary>
    /// Export a question bank
    /// </summary>
    [HttpGet("{id}/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionExportResultDto>> ExportQuestionBank(int id, [FromQuery] string format = "json")
    {
        try
        {
            var export = await _questionBankService.ExportQuestionBankAsync(id, format);
            return Ok(export);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting the question bank");
        }
    }

    /// <summary>
    /// Import questions into a question bank
    /// </summary>
    [HttpPost("{id}/import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankImportResultDto>> ImportQuestions(int id, [FromBody] QuestionBankImportDto importDto)
    {
        try
        {
            var result = await _questionBankService.ImportQuestionsAsync(id, importDto);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing questions into question bank {QuestionBankId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while importing questions into the question bank");
        }
    }
}
