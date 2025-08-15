using System.Linq.Expressions;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Generic repository interface for data access operations
/// Provides CRUD operations and query capabilities for all entities
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its primary key ID
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all entities with optional filtering
    /// </summary>
    /// <param name="where">Optional WHERE clause</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null);

    /// <summary>
    /// Gets entities with pagination support
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="where">Optional WHERE clause</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <param name="orderBy">Optional ORDER BY clause</param>
    /// <returns>Paged result with items and total count</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? where = null, 
        object? parameters = null, 
        string? orderBy = null);

    /// <summary>
    /// Adds a new entity to the repository
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity with generated ID</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <returns>Updated entity</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by ID (soft delete if supported)
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Checks if an entity exists by ID
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <returns>True if entity exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Executes a custom SQL query
    /// </summary>
    /// <param name="sql">SQL query string</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null);

    /// <summary>
    /// Executes a custom SQL query and returns the first result
    /// </summary>
    /// <param name="sql">SQL query string</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>First entity if found, null otherwise</returns>
    Task<T?> QueryFirstOrDefaultAsync(string sql, object? parameters = null);

    /// <summary>
    /// Executes a custom SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    /// <param name="sql">SQL command string</param>
    /// <param name="parameters">Command parameters</param>
    /// <returns>Number of affected rows</returns>
    Task<int> ExecuteAsync(string sql, object? parameters = null);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <returns>Transaction object</returns>
    Task<IDisposable> BeginTransactionAsync();

    /// <summary>
    /// Gets the total count of entities
    /// </summary>
    /// <param name="where">Optional WHERE clause</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Total count</returns>
    Task<int> GetCountAsync(string? where = null, object? parameters = null);
}
