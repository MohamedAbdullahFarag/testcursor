using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations using Dapper
/// Provides async data access patterns for all entities
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all entities with optional filtering
    /// </summary>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Collection of entities matching the criteria</returns>
    Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null);

    /// <summary>
    /// Gets entities with pagination support
    /// </summary>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="orderBy">Optional ORDER BY clause</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Collection of entities matching the criteria</returns>
    Task<IEnumerable<T>> GetPagedAsync(int offset, int limit, string? where = null, string? orderBy = null, object? parameters = null);

    /// <summary>
    /// Gets the total count of entities with optional filtering
    /// </summary>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Total count of entities</returns>
    Task<int> CountAsync(string? where = null, object? parameters = null);

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <returns>The created entity with generated ID</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Performs a soft delete on the entity (sets IsDeleted = true)
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Permanently removes an entity from the database
    /// Use with caution - this is irreversible
    /// </summary>
    /// <param name="id">The unique identifier of the entity to permanently delete</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    Task<bool> HardDeleteAsync(int id);

    /// <summary>
    /// Checks if an entity exists with the given ID
    /// </summary>
    /// <param name="id">The unique identifier to check</param>
    /// <returns>True if the entity exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Executes a custom SQL query and returns entities
    /// </summary>
    /// <param name="sql">The SQL query to execute</param>
    /// <param name="parameters">Parameters for the SQL query</param>
    /// <returns>Collection of entities matching the query</returns>
    Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null);

    /// <summary>
    /// Executes a custom SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    /// <param name="sql">The SQL command to execute</param>
    /// <param name="parameters">Parameters for the SQL command</param>
    /// <returns>Number of affected rows</returns>
    Task<int> ExecuteAsync(string sql, object? parameters = null);
}
