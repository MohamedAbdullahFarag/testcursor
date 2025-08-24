using System.Linq.Expressions;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Generic repository interface for data access operations
/// Provides CRUD operations and query capabilities for all entities
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T>:IBaseRepository<T> where T : class
{
    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <returns>Transaction object</returns>
    Task<IDisposable> BeginTransactionAsync();

    /// <summary>
    /// Soft deletes an entity by ID
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <returns>True if soft deleted successfully, false otherwise</returns>
    Task<bool> SoftDeleteAsync(int id);
}
