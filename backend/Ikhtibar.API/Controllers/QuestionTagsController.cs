using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question tag management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionTagsController : ControllerBase
{
    private readonly IQuestionTagService _questionTagService;
    private readonly ILogger<QuestionTagsController> _logger;

    public QuestionTagsController(
        IQuestionTagService questionTagService,
        ILogger<QuestionTagsController> logger)
    {
        _questionTagService = questionTagService;
        _logger = logger;
    }

    /// <summary>
    /// Get all question tags
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> GetAllQuestionTags()
    {
        try
        {
            var tags = await _questionTagService.GetAllAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question tags");
        }
    }

    /// <summary>
    /// Get question tag by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTagDto>> GetQuestionTag(int id)
    {
        try
        {
            var tag = await _questionTagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound($"Question tag with ID {id} not found");
            }
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question tag {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question tag");
        }
    }

    /// <summary>
    /// Get question tag by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTagDto>> GetQuestionTagByCode(string code)
    {
        try
        {
            var tag = await _questionTagService.GetByCodeAsync(code);
            if (tag == null)
            {
                return NotFound($"Question tag with code '{code}' not found");
            }
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question tag by code {Code}", code);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the question tag");
        }
    }

    /// <summary>
    /// Create a new question tag
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTagDto>> CreateQuestionTag([FromBody] CreateQuestionTagDto createDto)
    {
        try
        {
            var tag = await _questionTagService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetQuestionTag), new { id = tag.Id }, tag);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question tag");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the question tag");
        }
    }

    /// <summary>
    /// Update an existing question tag
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionTagDto>> UpdateQuestionTag(int id, [FromBody] UpdateQuestionTagDto updateDto)
    {
        try
        {
            var tag = await _questionTagService.UpdateAsync(id, updateDto);
            if (tag == null)
            {
                return NotFound($"Question tag with ID {id} not found");
            }
            return Ok(tag);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question tag {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the question tag");
        }
    }

    /// <summary>
    /// Delete a question tag
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteQuestionTag(int id)
    {
        try
        {
            var result = await _questionTagService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Question tag with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question tag {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the question tag");
        }
    }

    /// <summary>
    /// Get question tags by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> GetQuestionTagsByCategory(string category)
    {
        try
        {
            var tags = await _questionTagService.GetByCategoryAsync(category);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question tags by category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question tags");
        }
    }

    /// <summary>
    /// Get active question tags
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> GetActiveQuestionTags()
    {
        try
        {
            var tags = await _questionTagService.GetActiveAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active question tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question tags");
        }
    }

    /// <summary>
    /// Get system question tags
    /// </summary>
    [HttpGet("system")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> GetSystemQuestionTags()
    {
        try
        {
            var tags = await _questionTagService.GetSystemTagsAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system question tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question tags");
        }
    }

    /// <summary>
    /// Search question tags
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> SearchQuestionTags([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var tags = await _questionTagService.SearchAsync(query, limit);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching question tags with query {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching question tags");
        }
    }

    /// <summary>
    /// Get question tag suggestions
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> GetQuestionTagSuggestions([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required");
            }

            var suggestions = await _questionTagService.GetSuggestionsAsync(query, limit);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question tag suggestions for query {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting tag suggestions");
        }
    }

    /// <summary>
    /// Get question tag statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetQuestionTagStatistics()
    {
        try
        {
            var statistics = await _questionTagService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question tag statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Get popular question tags
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTagDto>>> GetPopularQuestionTags([FromQuery] int limit = 20)
    {
        try
        {
            var tags = await _questionTagService.GetPopularAsync(limit);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving popular question tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question tags");
        }
    }
}
