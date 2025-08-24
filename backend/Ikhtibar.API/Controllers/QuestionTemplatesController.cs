using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question template management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionTemplatesController : ControllerBase
{
    private readonly IQuestionTemplateService _questionTemplateService;
    private readonly ILogger<QuestionTemplatesController> _logger;

    public QuestionTemplatesController(
        IQuestionTemplateService questionTemplateService,
        ILogger<QuestionTemplatesController> logger)
    {
        _questionTemplateService = questionTemplateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetAllQuestionTemplates()
    {
        try
        {
            var templates = await _questionTemplateService.GetAllAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question template by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTemplateDto>> GetQuestionTemplate(int id)
    {
        try
        {
            var template = await _questionTemplateService.GetByIdAsync(id);
            if (template == null)
            {
                return NotFound($"Question template with ID {id} not found");
            }
            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question template {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question template");
        }
    }

    /// <summary>
    /// Get question template by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTemplateDto>> GetQuestionTemplateByCode(string code)
    {
        try
        {
            var template = await _questionTemplateService.GetByCodeAsync(code);
            if (template == null)
            {
                return NotFound($"Question template with code '{code}' not found");
            }
            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question template by code {Code}", code);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question template");
        }
    }

    /// <summary>
    /// Create a new question template
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTemplateDto>> CreateQuestionTemplate([FromBody] CreateQuestionTemplateDto createDto)
    {
        try
        {
            var template = await _questionTemplateService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionTemplate), new { id = template.Id }, template);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question template");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question template");
        }
    }

    /// <summary>
    /// Update an existing question template
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTemplateDto>> UpdateQuestionTemplate(int id, [FromBody] UpdateQuestionTemplateDto updateDto)
    {
        try
        {
            var template = await _questionTemplateService.UpdateAsync(id, updateDto);
            if (template == null)
            {
                return NotFound($"Question template with ID {id} not found");
            }
            return Ok(template);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question template {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question template");
        }
    }

    /// <summary>
    /// Delete a question template
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionTemplate(int id)
    {
        try
        {
            var result = await _questionTemplateService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question template with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question template {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question template");
        }
    }

    /// <summary>
    /// Get question templates by question type
    /// </summary>
    [HttpGet("question-type/{questionTypeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByType(int questionTypeId)
    {
        try
        {
            var templates = await _questionTemplateService.GetByQuestionTypeAsync(questionTypeId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by type {QuestionTypeId}", questionTypeId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByCategory(string category)
    {
        try
        {
            var templates = await _questionTemplateService.GetByCategoryAsync(category);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by difficulty level
    /// </summary>
    [HttpGet("difficulty/{difficultyLevelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByDifficulty(int difficultyLevelId)
    {
        try
        {
            var templates = await _questionTemplateService.GetByDifficultyLevelAsync(difficultyLevelId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by difficulty {DifficultyLevelId}", difficultyLevelId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by question bank
    /// </summary>
    [HttpGet("question-bank/{questionBankId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByBank(int questionBankId)
    {
        try
        {
            var templates = await _questionTemplateService.GetByQuestionBankAsync(questionBankId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by bank {QuestionBankId}", questionBankId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by curriculum
    /// </summary>
    [HttpGet("curriculum/{curriculumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByCurriculum(int curriculumId)
    {
        try
        {
            var templates = await _questionTemplateService.GetByCurriculumAsync(curriculumId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by curriculum {CurriculumId}", curriculumId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by subject
    /// </summary>
    [HttpGet("subject/{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesBySubject(int subjectId)
    {
        try
        {
            var templates = await _questionTemplateService.GetBySubjectAsync(subjectId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by subject {SubjectId}", subjectId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get question templates by grade level
    /// </summary>
    [HttpGet("grade-level/{gradeLevelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetQuestionTemplatesByGradeLevel(int gradeLevelId)
    {
        try
        {
            var templates = await _questionTemplateService.GetByGradeLevelAsync(gradeLevelId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question templates by grade level {GradeLevelId}", gradeLevelId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get active question templates
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetActiveQuestionTemplates()
    {
        try
        {
            var templates = await _questionTemplateService.GetActiveAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active question templates");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Get system question templates
    /// </summary>
    [HttpGet("system")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> GetSystemQuestionTemplates()
    {
        try
        {
            var templates = await _questionTemplateService.GetSystemTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system question templates");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question templates");
        }
    }

    /// <summary>
    /// Search question templates
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTemplateDto>>> SearchQuestionTemplates([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var templates = await _questionTemplateService.SearchAsync(query, limit);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching question templates with query {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching question templates");
        }
    }

    /// <summary>
    /// Get question template statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionTemplateStatistics()
    {
        try
        {
            var statistics = await _questionTemplateService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question template statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Clone question template
    /// </summary>
    [HttpPost("{id}/clone")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTemplateDto>> CloneQuestionTemplate(int id, [FromBody] CreateQuestionTemplateDto cloneDto)
    {
        try
        {
            var template = await _questionTemplateService.CloneAsync(id, cloneDto);
            if (template == null)
            {
                return NotFound($"Question template with ID {id} not found");
            }
            return CreatedAtAction(nameof(GetQuestionTemplate), new { id = template.Id }, template);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning question template {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while cloning the question template");
        }
    }
}
