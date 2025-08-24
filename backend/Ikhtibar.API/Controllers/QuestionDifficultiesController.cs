using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question difficulty level management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionDifficultiesController : ControllerBase
{
    private readonly IQuestionDifficultyService _questionDifficultyService;
    private readonly ILogger<QuestionDifficultiesController> _logger;

    public QuestionDifficultiesController(
        IQuestionDifficultyService questionDifficultyService,
        ILogger<QuestionDifficultiesController> logger)
    {
        _questionDifficultyService = questionDifficultyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question difficulty levels
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDifficultyDto>>> GetAllQuestionDifficulties()
    {
        try
        {
            var difficulties = await _questionDifficultyService.GetAllAsync();
            return Ok(difficulties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulties");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question difficulties");
        }
    }

    /// <summary>
    /// Get question difficulty level by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDifficultyDto>> GetQuestionDifficulty(int id)
    {
        try
        {
            var difficulty = await _questionDifficultyService.GetByIdAsync(id);
            if (difficulty == null)
            {
                return NotFound($"Question difficulty with ID {id} not found");
            }
            return Ok(difficulty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulty {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question difficulty");
        }
    }

    /// <summary>
    /// Get question difficulty level by name
    /// </summary>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDifficultyDto>> GetQuestionDifficultyByName(string name)
    {
        try
        {
            var difficulty = await _questionDifficultyService.GetByNameAsync(name);
            if (difficulty == null)
            {
                return NotFound($"Question difficulty with name '{name}' not found");
            }
            return Ok(difficulty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulty by name {Name}", name);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question difficulty");
        }
    }

    /// <summary>
    /// Create a new question difficulty level
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDifficultyDto>> CreateQuestionDifficulty([FromBody] CreateQuestionDifficultyDto createDto)
    {
        try
        {
            var difficulty = await _questionDifficultyService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionDifficulty), new { id = difficulty.Id }, difficulty);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question difficulty");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question difficulty");
        }
    }

    /// <summary>
    /// Update an existing question difficulty level
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionDifficultyDto>> UpdateQuestionDifficulty(int id, [FromBody] UpdateQuestionDifficultyDto updateDto)
    {
        try
        {
            var difficulty = await _questionDifficultyService.UpdateAsync(id, updateDto);
            if (difficulty == null)
            {
                return NotFound($"Question difficulty with ID {id} not found");
            }
            return Ok(difficulty);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question difficulty {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question difficulty");
        }
    }

    /// <summary>
    /// Delete a question difficulty level
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionDifficulty(int id)
    {
        try
        {
            var result = await _questionDifficultyService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question difficulty with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question difficulty {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question difficulty");
        }
    }

    /// <summary>
    /// Get question difficulty levels by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDifficultyDto>>> GetQuestionDifficultiesByCategory(string category)
    {
        try
        {
            var difficulties = await _questionDifficultyService.GetByCategoryAsync(category);
            return Ok(difficulties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulties by category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question difficulties");
        }
    }

    /// <summary>
    /// Get question difficulty levels by level range
    /// </summary>
    [HttpGet("level-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDifficultyDto>>> GetQuestionDifficultiesByLevelRange([FromQuery] int minLevel, [FromQuery] int maxLevel)
    {
        try
        {
            if (minLevel < 1 || maxLevel > 10 || minLevel > maxLevel)
            {
                return BadRequest("Invalid level range. Levels must be between 1 and 10, and minLevel must be less than or equal to maxLevel.");
            }

            var difficulties = await _questionDifficultyService.GetByLevelRangeAsync(minLevel, maxLevel);
            return Ok(difficulties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulties by level range {MinLevel}-{MaxLevel}", minLevel, maxLevel);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question difficulties");
        }
    }

    /// <summary>
    /// Get active question difficulty levels
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDifficultyDto>>> GetActiveQuestionDifficulties()
    {
        try
        {
            var difficulties = await _questionDifficultyService.GetActiveAsync();
            return Ok(difficulties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active question difficulties");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question difficulties");
        }
    }

    /// <summary>
    /// Get question difficulty statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionDifficultyStatistics()
    {
        try
        {
            var statistics = await _questionDifficultyService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question difficulty statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }
}
