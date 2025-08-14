namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Interface for database initialization service
/// </summary>
public interface IDatabaseInitializationService
{
    /// <summary>
    /// Initializes the database with schema and seed data
    /// </summary>
    Task InitializeDatabaseAsync();

    /// <summary>
    /// Checks if the database has been initialized with seed data
    /// </summary>
    Task<bool> IsDatabaseInitializedAsync();
}
