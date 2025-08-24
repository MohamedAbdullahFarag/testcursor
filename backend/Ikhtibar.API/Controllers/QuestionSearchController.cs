using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for advanced question search and filtering
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionSearchController : ControllerBase
{
    private readonly IQuestionSearchService _questionSearchService;
    private readonly ILogger<QuestionSearchController> _logger;

    public QuestionSearchController(
        IQuestionSearchService questionSearchService,
        ILogger<QuestionSearchController> logger)
    {
        _questionSearchService = questionSearchService;
        _logger = logger;
    }

    /// <summary>
    /// Search questions with advanced filters
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchQuestions([FromBody] QuestionSearchDto searchDto)
    {
        try
        {
            var results = await _questionSearchService.SearchQuestionsAsync(searchDto);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions");
        }
    }

    /// <summary>
    /// Full-text search questions
    /// </summary>
    [HttpGet("fulltext")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> FullTextSearch([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var results = await _questionSearchService.FullTextSearchAsync(query, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing full-text search for query: {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while performing full-text search");
        }
    }

    /// <summary>
    /// Search questions by criteria
    /// </summary>
    [HttpPost("criteria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByCriteria([FromBody] QuestionCriteriaDto criteria)
    {
        try
        {
            var results = await _questionSearchService.SearchByCriteriaAsync(criteria);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by criteria");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by criteria");
        }
    }

    /// <summary>
    /// Search questions by tags
    /// </summary>
    [HttpPost("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByTags([FromBody] TagFilterDto tagFilter)
    {
        try
        {
            var results = await _questionSearchService.SearchByTagsAsync(tagFilter);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by tags");
        }
    }

    /// <summary>
    /// Search questions by difficulty level
    /// </summary>
    [HttpGet("difficulty/{difficultyLevel}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByDifficulty(int difficultyLevel, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByDifficultyAsync(difficultyLevel, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by difficulty level {DifficultyLevel}", difficultyLevel);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by difficulty level");
        }
    }

    /// <summary>
    /// Search questions by type
    /// </summary>
    [HttpGet("type/{questionType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByType(int questionType, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByTypeAsync(questionType, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by type {QuestionType}", questionType);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by type");
        }
    }

    /// <summary>
    /// Search questions by tree node
    /// </summary>
    [HttpGet("treenode/{treeNodeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByTreeNode(int treeNodeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByTreeNodeAsync(treeNodeId, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by tree node {TreeNodeId}", treeNodeId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by tree node");
        }
    }

    /// <summary>
    /// Search questions by date range
    /// </summary>
    [HttpGet("daterange")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByDateRange(
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByDateRangeAsync(fromDate, toDate, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by date range from {FromDate} to {ToDate}", fromDate, toDate);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by date range");
        }
    }

    /// <summary>
    /// Search questions by author
    /// </summary>
    [HttpGet("author/{authorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByAuthor(int authorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByAuthorAsync(authorId, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by author {AuthorId}", authorId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by author");
        }
    }

    /// <summary>
    /// Search questions by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<QuestionDto>>> SearchByStatus(int status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _questionSearchService.SearchByStatusAsync(status, page, pageSize);
            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by status {Status}", status);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching questions by status");
        }
    }

    /// <summary>
    /// Get search suggestions
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> GetSearchSuggestions([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var suggestions = await _questionSearchService.GetSearchSuggestionsAsync(query, limit);
            return Ok(suggestions);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions for query: {Query}", query);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting search suggestions");
        }
    }

    /// <summary>
    /// Get popular search terms
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PopularSearchTermDto>>> GetPopularSearchTerms([FromQuery] int limit = 20)
    {
        try
        {
            var popularTerms = await _questionSearchService.GetPopularSearchTermsAsync(limit);
            return Ok(popularTerms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular search terms");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting popular search terms");
        }
    }

    /// <summary>
    /// Get search statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchStatisticsDto>> GetSearchStatistics()
    {
        try
        {
            var statistics = await _questionSearchService.GetSearchStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting search statistics");
        }
    }

    /// <summary>
    /// Save search query
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> SaveSearchQuery([FromBody] SaveSearchQueryDto saveQueryDto)
    {
        try
        {
            var result = await _questionSearchService.SaveSearchQueryAsync(saveQueryDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving search query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving search query");
        }
    }

    /// <summary>
    /// Get saved search queries
    /// </summary>
    [HttpGet("saved")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SavedSearchQueryDto>>> GetSavedSearchQueries()
    {
        try
        {
            var savedQueries = await _questionSearchService.GetSavedSearchQueriesAsync();
            return Ok(savedQueries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved search queries");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting saved search queries");
        }
    }

    /// <summary>
    /// Delete saved search query
    /// </summary>
    [HttpDelete("saved/{queryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteSavedSearchQuery(int queryId)
    {
        try
        {
            var result = await _questionSearchService.DeleteSavedSearchQueryAsync(queryId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Saved search query with ID {queryId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search query {QueryId}", queryId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting saved search query");
        }
    }
}
