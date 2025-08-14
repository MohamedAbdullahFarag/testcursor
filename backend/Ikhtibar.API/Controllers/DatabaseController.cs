using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Infrastructure.Services;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for database management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseInitializationService _dbInitService;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(
        IDatabaseInitializationService dbInitService,
        ILogger<DatabaseController> logger)
    {
        _dbInitService = dbInitService;
        _logger = logger;
    }

    /// <summary>
    /// Initialize the database with schema and seed data
    /// </summary>
    /// <returns>Result of the initialization</returns>
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeDatabase()
    {
        try
        {
            _logger.LogInformation("Manual database initialization requested");

            await _dbInitService.InitializeDatabaseAsync();

            return Ok(new
            {
                Success = true,
                Message = "Database initialized successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");

            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to initialize database",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Check if the database is initialized
    /// </summary>
    /// <returns>Database initialization status</returns>
    [HttpGet("status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var isInitialized = await _dbInitService.IsDatabaseInitializedAsync();

            return Ok(new
            {
                IsInitialized = isInitialized,
                Message = isInitialized ? "Database is initialized" : "Database is not initialized"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check database status");

            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to check database status",
                Error = ex.Message
            });
        }
    }
}
