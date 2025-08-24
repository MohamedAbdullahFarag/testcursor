using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question validation operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionValidationController : ControllerBase
{
    private readonly IQuestionValidationService _questionValidationService;
    private readonly ILogger<QuestionValidationController> _logger;

    public QuestionValidationController(
        IQuestionValidationService questionValidationService,
        ILogger<QuestionValidationController> logger)
    {
        _questionValidationService = questionValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Validate a question
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestion([FromBody] ValidateQuestionDto validateDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionAsync(validateDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating the question");
        }
    }

    /// <summary>
    /// Validate question data
    /// </summary>
    [HttpPost("validate-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionData([FromBody] CreateQuestionDto questionDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionDataAsync(questionDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question data");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question data");
        }
    }

    /// <summary>
    /// Validate question content
    /// </summary>
    [HttpPost("validate-content")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionContent([FromBody] ValidateQuestionContentDto contentDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionContentAsync(contentDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question content");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question content");
        }
    }

    /// <summary>
    /// Validate question answers
    /// </summary>
    [HttpPost("validate-answers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionAnswers([FromBody] ValidateQuestionAnswersDto answersDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionAnswersAsync(answersDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question answers");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question answers");
        }
    }

    /// <summary>
    /// Validate question metadata
    /// </summary>
    [HttpPost("validate-metadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionMetadata([FromBody] ValidateQuestionMetadataDto metadataDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionMetadataAsync(metadataDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question metadata");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question metadata");
        }
    }

    /// <summary>
    /// Check for duplicate questions
    /// </summary>
    [HttpPost("check-duplicates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DuplicateQuestionDto>>> CheckForDuplicates([FromBody] CheckDuplicatesDto checkDto)
    {
        try
        {
            var duplicates = await _questionValidationService.CheckForDuplicatesAsync(checkDto);
            return Ok(duplicates);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for duplicate questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking for duplicate questions");
        }
    }

    /// <summary>
    /// Validate question accessibility
    /// </summary>
    [HttpPost("validate-accessibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionAccessibility([FromBody] ValidateQuestionAccessibilityDto accessibilityDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionAccessibilityAsync(accessibilityDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question accessibility");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question accessibility");
        }
    }

    /// <summary>
    /// Validate question difficulty
    /// </summary>
    [HttpPost("validate-difficulty")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionValidationResult>> ValidateQuestionDifficulty([FromBody] ValidateQuestionDifficultyDto difficultyDto)
    {
        try
        {
            var result = await _questionValidationService.ValidateQuestionDifficultyAsync(difficultyDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question difficulty");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating question difficulty");
        }
    }

    /// <summary>
    /// Get validation rules
    /// </summary>
    [HttpGet("rules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ValidationRuleDto>>> GetValidationRules()
    {
        try
        {
            var rules = await _questionValidationService.GetValidationRulesAsync();
            return Ok(rules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving validation rules");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving validation rules");
        }
    }

    /// <summary>
    /// Get validation rule by ID
    /// </summary>
    [HttpGet("rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ValidationRuleDto>> GetValidationRule(int ruleId)
    {
        try
        {
            var rule = await _questionValidationService.GetValidationRuleAsync(ruleId);
            return Ok(rule);
        }
        catch (NotFoundException)
        {
            return NotFound($"Validation rule with ID {ruleId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving validation rule {RuleId}", ruleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving validation rule");
        }
    }

    /// <summary>
    /// Create validation rule
    /// </summary>
    [HttpPost("rules")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ValidationRuleDto>> CreateValidationRule([FromBody] CreateValidationRuleDto createRuleDto)
    {
        try
        {
            var rule = await _questionValidationService.CreateValidationRuleAsync(createRuleDto);
            return CreatedAtAction(nameof(GetValidationRule), new { ruleId = rule.Id }, rule);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating validation rule");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating validation rule");
        }
    }

    /// <summary>
    /// Update validation rule
    /// </summary>
    [HttpPut("rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ValidationRuleDto>> UpdateValidationRule(int ruleId, [FromBody] UpdateValidationRuleDto updateRuleDto)
    {
        try
        {
            var rule = await _questionValidationService.UpdateValidationRuleAsync(ruleId, updateRuleDto);
            return Ok(rule);
        }
        catch (NotFoundException)
        {
            return NotFound($"Validation rule with ID {ruleId} not found");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating validation rule {RuleId}", ruleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating validation rule");
        }
    }

    /// <summary>
    /// Delete validation rule
    /// </summary>
    [HttpDelete("rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteValidationRule(int ruleId)
    {
        try
        {
            var result = await _questionValidationService.DeleteValidationRuleAsync(ruleId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Validation rule with ID {ruleId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting validation rule {RuleId}", ruleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting validation rule");
        }
    }

    /// <summary>
    /// Get validation statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ValidationStatisticsDto>> GetValidationStatistics()
    {
        try
        {
            var statistics = await _questionValidationService.GetValidationStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving validation statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving validation statistics");
        }
    }

    /// <summary>
    /// Get validation history for a question
    /// </summary>
    [HttpGet("history/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionValidationHistoryDto>>> GetQuestionValidationHistory(int questionId)
    {
        try
        {
            var history = await _questionValidationService.GetQuestionValidationHistoryAsync(questionId);
            return Ok(history);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving validation history for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving validation history");
        }
    }

    /// <summary>
    /// Run bulk validation
    /// </summary>
    [HttpPost("bulk-validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkValidationResultDto>> RunBulkValidation([FromBody] BulkValidationDto bulkValidationDto)
    {
        try
        {
            var result = await _questionValidationService.RunBulkValidationAsync(bulkValidationDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running bulk validation");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while running bulk validation");
        }
    }
}

// DTOs moved to Ikhtibar.Shared.DTOs.ControllerInlineDtos.cs
