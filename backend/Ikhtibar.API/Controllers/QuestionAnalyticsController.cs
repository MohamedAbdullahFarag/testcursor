using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question analytics operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionAnalyticsController : ControllerBase
{
    private readonly IQuestionAnalyticsService _questionAnalyticsService;
    private readonly ILogger<QuestionAnalyticsController> _logger;

    public QuestionAnalyticsController(
        IQuestionAnalyticsService questionAnalyticsService,
        ILogger<QuestionAnalyticsController> logger)
    {
        _questionAnalyticsService = questionAnalyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get question usage statistics
    /// </summary>
    [HttpGet("usage/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionUsageStatisticsDto>> GetQuestionUsage(int questionId)
    {
        try
        {
            var usage = await _questionAnalyticsService.GetQuestionUsageAsync(questionId);
            return Ok(usage);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usage statistics for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question usage statistics");
        }
    }

    /// <summary>
    /// Get question performance data
    /// </summary>
    [HttpGet("performance/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionPerformanceDto>> GetQuestionPerformance(int questionId)
    {
        try
        {
            var performance = await _questionAnalyticsService.GetQuestionPerformanceAsync(questionId);
            return Ok(performance);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance data for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question performance data");
        }
    }

    /// <summary>
    /// Get question performance trends
    /// </summary>
    [HttpGet("performance/{questionId}/trends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionPerformanceTrendDto>> GetQuestionPerformanceTrends(int questionId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var trends = await _questionAnalyticsService.GetQuestionPerformanceTrendsAsync(questionId, fromDate, toDate);
            return Ok(trends);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance trends for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question performance trends");
        }
    }

    /// <summary>
    /// Get question performance summary
    /// </summary>
    [HttpGet("performance/{questionId}/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionPerformanceSummaryDto>> GetQuestionPerformanceSummary(int questionId)
    {
        try
        {
            var summary = await _questionAnalyticsService.GetQuestionPerformanceSummaryAsync(questionId);
            return Ok(summary);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance summary for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question performance summary");
        }
    }

    /// <summary>
    /// Get similar questions
    /// </summary>
    [HttpGet("similar/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetSimilarQuestions(int questionId, [FromQuery] int limit = 10)
    {
        try
        {
            var similarQuestions = await _questionAnalyticsService.GetSimilarQuestionsAsync(questionId, limit);
            return Ok(similarQuestions);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving similar questions for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving similar questions");
        }
    }

    /// <summary>
    /// Get question quality metrics
    /// </summary>
    [HttpGet("quality/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionQualityDto>> GetQuestionQuality(int questionId)
    {
        try
        {
            var quality = await _questionAnalyticsService.GetQuestionQualityAsync(questionId);
            return Ok(quality);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quality metrics for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question quality metrics");
        }
    }

    /// <summary>
    /// Get question difficulty analysis
    /// </summary>
    [HttpGet("difficulty/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DifficultyAnalysisDto>> GetQuestionDifficultyAnalysis(int questionId)
    {
        try
        {
            var difficulty = await _questionAnalyticsService.GetQuestionDifficultyAnalysisAsync(questionId);
            return Ok(difficulty);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question with ID {questionId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving difficulty analysis for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question difficulty analysis");
        }
    }

    /// <summary>
    /// Get question type analytics
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionTypeCountDto>>> GetQuestionTypeAnalytics()
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetQuestionTypeAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question type analytics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question type analytics");
        }
    }

    /// <summary>
    /// Get difficulty level analytics
    /// </summary>
    [HttpGet("difficulty-levels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DifficultyLevelCountDto>>> GetDifficultyLevelAnalytics()
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetDifficultyLevelAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving difficulty level analytics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving difficulty level analytics");
        }
    }

    /// <summary>
    /// Get question usage trends
    /// </summary>
    [HttpGet("usage-trends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UsageTrendDto>>> GetQuestionUsageTrends([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var trends = await _questionAnalyticsService.GetQuestionUsageTrendsAsync(fromDate, toDate);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question usage trends");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question usage trends");
        }
    }

    /// <summary>
    /// Get performance trends
    /// </summary>
    [HttpGet("performance-trends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PerformanceTrendDto>>> GetPerformanceTrends([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var trends = await _questionAnalyticsService.GetPerformanceTrendsAsync(fromDate, toDate);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance trends");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving performance trends");
        }
    }

    /// <summary>
    /// Get popular tags
    /// </summary>
    [HttpGet("popular-tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PopularTagDto>>> GetPopularTags([FromQuery] int limit = 20)
    {
        try
        {
            var tags = await _questionAnalyticsService.GetPopularTagsAsync(limit);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving popular tags");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving popular tags");
        }
    }

    /// <summary>
    /// Get tag categories
    /// </summary>
    [HttpGet("tag-categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TagCategoryDto>>> GetTagCategories()
    {
        try
        {
            var categories = await _questionAnalyticsService.GetTagCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tag categories");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tag categories");
        }
    }

    /// <summary>
    /// Get question bank analytics
    /// </summary>
    [HttpGet("question-bank/{questionBankId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionBankAnalyticsDto>> GetQuestionBankAnalytics(int questionBankId)
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetQuestionBankAnalyticsAsync(questionBankId);
            return Ok(analytics);
        }
        catch (NotFoundException)
        {
            return NotFound($"Question bank with ID {questionBankId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics for question bank {QuestionBankId}", questionBankId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving question bank analytics");
        }
    }

    /// <summary>
    /// Get template quality analytics
    /// </summary>
    [HttpGet("template-quality")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TemplateQualityDto>>> GetTemplateQualityAnalytics()
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetTemplateQualityAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template quality analytics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving template quality analytics");
        }
    }

    /// <summary>
    /// Get version analytics
    /// </summary>
    [HttpGet("versions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VersionAnalyticsDto>>> GetVersionAnalytics()
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetVersionAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving version analytics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving version analytics");
        }
    }

    /// <summary>
    /// Get validation statistics
    /// </summary>
    [HttpGet("validation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ValidationStatisticsDto>>> GetValidationStatistics()
    {
        try
        {
            var statistics = await _questionAnalyticsService.GetValidationStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving validation statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving validation statistics");
        }
    }

    /// <summary>
    /// Get reporting analytics
    /// </summary>
    [HttpGet("reporting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportingDto>>> GetReportingAnalytics()
    {
        try
        {
            var analytics = await _questionAnalyticsService.GetReportingAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reporting analytics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving reporting analytics");
        }
    }

    /// <summary>
    /// Get custom analytics report
    /// </summary>
    [HttpPost("custom-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomAnalyticsReportDto>> GetCustomAnalyticsReport([FromBody] CustomAnalyticsRequestDto request)
    {
        try
        {
            var report = await _questionAnalyticsService.GetCustomAnalyticsReportAsync(request);
            return Ok(report);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating custom analytics report");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating custom analytics report");
        }
    }

    /// <summary>
    /// Export analytics data
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnalyticsExportDto>> ExportAnalyticsData([FromBody] AnalyticsExportRequestDto request)
    {
        try
        {
            var export = await _questionAnalyticsService.ExportAnalyticsDataAsync(request);
            return Ok(export);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting analytics data");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting analytics data");
        }
    }
}
