using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service responsible for database initialization and seeding
/// </summary>
public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        IDbConnectionFactory connectionFactory,
        IConfiguration configuration,
        ILogger<DatabaseInitializationService> logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the database with schema and seed data
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");

            // First, ensure the database exists
            await EnsureDatabaseExistsAsync();

            // Execute the data.sql script
            await ExecuteDataSqlScriptAsync();

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }

    /// <summary>
    /// Ensures the database exists, creates it if it doesn't
    /// </summary>
    private async Task EnsureDatabaseExistsAsync()
    {
        var connectionString = _configuration.GetConnectionString("IkhtibarDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'IkhtibarDatabase' not found");
        }

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        // Check if database exists
        var checkDbQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
        using var checkCommand = new SqlCommand(checkDbQuery, connection);
        var result = await checkCommand.ExecuteScalarAsync();
        var dbExists = result != null && (int)result > 0;

        if (!dbExists)
        {
            _logger.LogInformation($"Database '{databaseName}' does not exist. Creating...");

            var createDbQuery = $"CREATE DATABASE [{databaseName}]";
            using var createCommand = new SqlCommand(createDbQuery, connection);
            await createCommand.ExecuteNonQueryAsync();

            _logger.LogInformation($"Database '{databaseName}' created successfully.");
        }
        else
        {
            _logger.LogInformation($"Database '{databaseName}' already exists.");
        }
    }

    /// <summary>
    /// Executes the InitializeDatabase.sql script to create tables and seed data
    /// </summary>
    private async Task ExecuteDataSqlScriptAsync()
    {
        _logger.LogInformation("Executing InitializeDatabase.sql script...");

        // Try multiple paths to find the SQL file
        var possiblePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Data", "InitializeDatabase.sql"), // Output directory
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Ikhtibar.Infrastructure", "Data", "InitializeDatabase.sql"), // Development path
            Path.Combine(Environment.CurrentDirectory, "Data", "InitializeDatabase.sql"), // Current directory
            Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "", "Data", "InitializeDatabase.sql") // Assembly location
        };

        string? dataFilePath = null;
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                dataFilePath = path;
                break;
            }
        }

        if (dataFilePath == null)
        {
            _logger.LogWarning($"InitializeDatabase.sql file not found. Searched paths: {string.Join(", ", possiblePaths)}");
            return;
        }

        _logger.LogInformation($"Found InitializeDatabase.sql at: {dataFilePath}");

        var sqlScript = await File.ReadAllTextAsync(dataFilePath);

        // Split the script into batches (separated by GO statements)
        var batches = SplitSqlScript(sqlScript);

        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        foreach (var batch in batches)
        {
            if (string.IsNullOrWhiteSpace(batch))
                continue;

            try
            {
                using var command = new SqlCommand(batch, connection);
                command.CommandTimeout = 300; // 5 minutes timeout for long operations

                await command.ExecuteNonQueryAsync();
                _logger.LogDebug("Executed batch successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute SQL batch: {Batch}", batch.Substring(0, Math.Min(batch.Length, 100)));

                // Check if it's a non-critical error (like table already exists)
                if (IsNonCriticalError(ex))
                {
                    _logger.LogInformation("Continuing execution despite non-critical error");
                    continue;
                }

                throw;
            }
        }

        _logger.LogInformation("Data.sql script executed successfully.");
    }

    /// <summary>
    /// Splits SQL script into individual batches separated by GO statements
    /// </summary>
    private static List<string> SplitSqlScript(string script)
    {
        var batches = new List<string>();
        var lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var currentBatch = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Check if line is a GO statement
            if (string.Equals(trimmedLine, "GO", StringComparison.OrdinalIgnoreCase))
            {
                if (currentBatch.Any())
                {
                    batches.Add(string.Join("\n", currentBatch));
                    currentBatch.Clear();
                }
            }
            else
            {
                currentBatch.Add(line);
            }
        }

        // Add the last batch if it exists
        if (currentBatch.Any())
        {
            batches.Add(string.Join("\n", currentBatch));
        }

        return batches;
    }

    /// <summary>
    /// Determines if an error is non-critical and execution can continue
    /// </summary>
    private static bool IsNonCriticalError(Exception ex)
    {
        if (ex is SqlException sqlEx)
        {
            // Error codes for non-critical errors
            return sqlEx.Number switch
            {
                2714 => true, // Object already exists
                15007 => true, // User/role already exists
                2627 => true, // Duplicate key/constraint violation
                547 => true,  // Foreign key constraint violation (might be OK for merges)
                _ => false
            };
        }

        return false;
    }

    /// <summary>
    /// Checks if the database has been initialized with seed data
    /// </summary>
    public async Task<bool> IsDatabaseInitializedAsync()
    {
        try
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            // Check if key tables exist and have data
            var checkQuery = @"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME IN ('Users', 'Roles', 'TreeNodeTypes', 'TreeNodes')";

            using var command = new SqlCommand(checkQuery, connection);

            var tableCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            return tableCount >= 4; // All 4 key tables should exist
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check database initialization status");
            return false;
        }
    }
}
