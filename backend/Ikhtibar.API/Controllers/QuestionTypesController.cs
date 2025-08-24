using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question type management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionTypesController : ControllerBase
{
    private readonly IQuestionTypeService _questionTypeService;
    private readonly ILogger<QuestionTypesController> _logger;

    public QuestionTypesController(
        IQuestionTypeService questionTypeService,
        ILogger<QuestionTypesController> logger)
    {
        _questionTypeService = questionTypeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetAllQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetAllAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question type by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTypeDto>> GetQuestionType(int id)
    {
        try
        {
            var questionType = await _questionTypeService.GetByIdAsync(id);
            if (questionType == null)
            {
                return NotFound($"Question type with ID {id} not found");
            }
            return Ok(questionType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question type");
        }
    }

    /// <summary>
    /// Get question type by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTypeDto>> GetQuestionTypeByCode(string code)
    {
        try
        {
            var questionType = await _questionTypeService.GetByCodeAsync(code);
            if (questionType == null)
            {
                return NotFound($"Question type with code '{code}' not found");
            }
            return Ok(questionType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question type by code {Code}", code);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question type");
        }
    }

    /// <summary>
    /// Create a new question type
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTypeDto>> CreateQuestionType([FromBody] CreateQuestionTypeDto createDto)
    {
        try
        {
            var questionType = await _questionTypeService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionType), new { id = questionType.Id }, questionType);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question type");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question type");
        }
    }

    /// <summary>
    /// Update an existing question type
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTypeDto>> UpdateQuestionType(int id, [FromBody] UpdateQuestionTypeDto updateDto)
    {
        try
        {
            var questionType = await _questionTypeService.UpdateAsync(id, updateDto);
            if (questionType == null)
            {
                return NotFound($"Question type with ID {id} not found");
            }
            return Ok(questionType);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question type");
        }
    }

    /// <summary>
    /// Delete a question type
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionType(int id)
    {
        try
        {
            var result = await _questionTypeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question type with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question type");
        }
    }

    /// <summary>
    /// Get question types by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetQuestionTypesByCategory(string category)
    {
        try
        {
            var questionTypes = await _questionTypeService.GetByCategoryAsync(category);
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question types by category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question types that support multiple choice
    /// </summary>
    [HttpGet("multiple-choice")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetMultipleChoiceQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetMultipleChoiceTypesAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple choice question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question types that support true/false answers
    /// </summary>
    [HttpGet("true-false")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetTrueFalseQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetTrueFalseTypesAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving true/false question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question types that support essay answers
    /// </summary>
    [HttpGet("essay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetEssayQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetEssayTypesAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving essay question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question types that support numeric answers
    /// </summary>
    [HttpGet("numeric")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetNumericQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetNumericTypesAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving numeric question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question types that support file uploads
    /// </summary>
    [HttpGet("file-upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetFileUploadQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetFileUploadTypesAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file upload question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get active question types
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeDto>>> GetActiveQuestionTypes()
    {
        try
        {
            var questionTypes = await _questionTypeService.GetActiveAsync();
            return Ok(questionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active question types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question types");
        }
    }

    /// <summary>
    /// Get question type statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionTypeStatistics()
    {
        try
        {
            var statistics = await _questionTypeService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question type statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }
}
