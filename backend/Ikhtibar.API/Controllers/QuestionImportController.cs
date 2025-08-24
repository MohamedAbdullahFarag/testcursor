using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using System.Security.Claims;
using System.IO;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question import and export operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionImportController : ControllerBase
{
    private readonly IQuestionImportService _questionImportService;
    private readonly ILogger<QuestionImportController> _logger;

    public QuestionImportController(
        IQuestionImportService questionImportService,
        ILogger<QuestionImportController> logger)
    {
        _questionImportService = questionImportService;
        _logger = logger;
    }

    /// <summary>
    /// Import questions from file
    /// </summary>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportResultDto>> ImportQuestions([FromBody] QuestionImportDto importDto)
    {
        try
        {
            // Set the importer ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            importDto.ImportedBy = userId;

            var result = await _questionImportService.ImportQuestionsAsync(importDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while importing questions");
        }
    }

    /// <summary>
    /// Import questions from Excel file
    /// </summary>
    [HttpPost("import/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportResultDto>> ImportFromExcel([FromForm] IFormFile file, [FromForm] QuestionImportOptionsDto options)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            // Set the importer ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            options.ImportedBy = userId;

            // Save uploaded file to a temporary path and convert to shared ImportFileDto
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            await using (var stream = System.IO.File.Create(tempPath))
            {
                await file.CopyToAsync(stream);
            }

            var importFile = new Ikhtibar.Shared.DTOs.ImportFileDto
            {
                FileName = file.FileName,
                FilePath = tempPath,
                FileSize = file.Length,
                FileType = file.ContentType ?? string.Empty,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = userId
            };

            var result = await _questionImportService.ImportFromExcelAsync(importFile, options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing questions from Excel file {FileName}", file?.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while importing questions from Excel");
        }
    }

    /// <summary>
    /// Import questions from CSV file
    /// </summary>
    [HttpPost("import/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportResultDto>> ImportFromCsv([FromForm] IFormFile file, [FromForm] QuestionImportOptionsDto options)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            // Set the importer ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            options.ImportedBy = userId;

            // Save uploaded file to a temporary path and convert to shared ImportFileDto
            var tempPathCsv = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            await using (var streamCsv = System.IO.File.Create(tempPathCsv))
            {
                await file.CopyToAsync(streamCsv);
            }

            var importFileCsv = new Ikhtibar.Shared.DTOs.ImportFileDto
            {
                FileName = file.FileName,
                FilePath = tempPathCsv,
                FileSize = file.Length,
                FileType = file.ContentType ?? string.Empty,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = userId
            };

            var result = await _questionImportService.ImportFromCsvAsync(importFileCsv, options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing questions from CSV file {FileName}", file?.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while importing questions from CSV");
        }
    }

    /// <summary>
    /// Import questions from JSON file
    /// </summary>
    [HttpPost("import/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportResultDto>> ImportFromJson([FromForm] IFormFile file, [FromForm] QuestionImportOptionsDto options)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            // Set the importer ID from the current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            options.ImportedBy = userId;

            // Save uploaded file to a temporary path and convert to shared ImportFileDto
            var tempPathJson = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            await using (var streamJson = System.IO.File.Create(tempPathJson))
            {
                await file.CopyToAsync(streamJson);
            }

            var importFileJson = new Ikhtibar.Shared.DTOs.ImportFileDto
            {
                FileName = file.FileName,
                FilePath = tempPathJson,
                FileSize = file.Length,
                FileType = file.ContentType ?? string.Empty,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = userId
            };

            var result = await _questionImportService.ImportFromJsonAsync(importFileJson, options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing questions from JSON file {FileName}", file?.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while importing questions from JSON");
        }
    }

    /// <summary>
    /// Export questions to file
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionExportResultDto>> ExportQuestions([FromBody] QuestionExportDto exportDto)
    {
        try
        {
            var result = await _questionImportService.ExportQuestionsAsync(exportDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting questions");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting questions");
        }
    }

    /// <summary>
    /// Export questions to Excel
    /// </summary>
    [HttpPost("export/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionExportResultDto>> ExportToExcel([FromBody] QuestionExportOptionsDto options)
    {
        try
        {
            var result = await _questionImportService.ExportToExcelAsync(options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting questions to Excel");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting questions to Excel");
        }
    }

    /// <summary>
    /// Export questions to CSV
    /// </summary>
    [HttpPost("export/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionExportResultDto>> ExportToCsv([FromBody] QuestionExportOptionsDto options)
    {
        try
        {
            var result = await _questionImportService.ExportToCsvAsync(options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting questions to CSV");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting questions to CSV");
        }
    }

    /// <summary>
    /// Export questions to JSON
    /// </summary>
    [HttpPost("export/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionExportResultDto>> ExportToJson([FromBody] QuestionExportOptionsDto options)
    {
        try
        {
            var result = await _questionImportService.ExportToJsonAsync(options);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting questions to JSON");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while exporting questions to JSON");
        }
    }

    /// <summary>
    /// Get import batch status
    /// </summary>
    [HttpGet("batch/{batchId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportBatchDto>> GetImportBatch(int batchId)
    {
        try
        {
            var batch = await _questionImportService.GetImportBatchAsync(batchId);
            return Ok(batch);
        }
        catch (NotFoundException)
        {
            return NotFound($"Import batch with ID {batchId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving import batch {BatchId}", batchId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving import batch");
        }
    }

    /// <summary>
    /// Get import batch logs
    /// </summary>
    [HttpGet("batch/{batchId}/logs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuestionImportLogDto>>> GetImportBatchLogs(int batchId)
    {
        try
        {
            var logs = await _questionImportService.GetImportBatchLogsAsync(batchId);
            return Ok(logs);
        }
        catch (NotFoundException)
        {
            return NotFound($"Import batch with ID {batchId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving import batch logs for batch {BatchId}", batchId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving import batch logs");
        }
    }

    /// <summary>
    /// Cancel import batch
    /// </summary>
    [HttpPost("batch/{batchId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> CancelImportBatch(int batchId)
    {
        try
        {
            var result = await _questionImportService.CancelImportBatchAsync(batchId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Import batch with ID {batchId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling import batch {BatchId}", batchId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while canceling import batch");
        }
    }

    /// <summary>
    /// Retry failed imports in batch
    /// </summary>
    [HttpPost("batch/{batchId}/retry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportResultDto>> RetryFailedImports(int batchId)
    {
        try
        {
            var result = await _questionImportService.RetryFailedImportsAsync(batchId);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound($"Import batch with ID {batchId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying failed imports for batch {BatchId}", batchId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrying failed imports");
        }
    }

    /// <summary>
    /// Get supported import formats
    /// </summary>
    [HttpGet("formats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SupportedImportFormatDto>>> GetSupportedFormats()
    {
        try
        {
            var formats = await _questionImportService.GetSupportedImportFormatsAsync();
            return Ok(formats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supported import formats");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving supported import formats");
        }
    }

    /// <summary>
    /// Get import template
    /// </summary>
    [HttpGet("template/{format}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportTemplateDto>> GetImportTemplate(string format)
    {
        try
        {
            var template = await _questionImportService.GetImportTemplateAsync(format);
            return Ok(template);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving import template for format {Format}", format);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving import template");
        }
    }

    /// <summary>
    /// Validate import data
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportValidationResultDto>> ValidateImportData([FromBody] QuestionImportValidationDto validationDto)
    {
        try
        {
            var result = await _questionImportService.ValidateImportDataAsync(validationDto);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating import data");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating import data");
        }
    }

    /// <summary>
    /// Get import statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionImportStatisticsDto>> GetImportStatistics()
    {
        try
        {
            var statistics = await _questionImportService.GetImportStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving import statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving import statistics");
        }
    }
}

// DTOs moved to Ikhtibar.Shared.DTOs.ControllerInlineDtos.cs
