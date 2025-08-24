using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    /// <summary>
    /// Generic base repository interface for CRUD and query operations.
    /// Provides CRUD operations and query capabilities for all entities.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? where = null,
            object? parameters = null,
            string? orderBy = null);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null);
        Task<T?> QueryFirstOrDefaultAsync(string sql, object? parameters = null);
        Task<int> ExecuteAsync(string sql, object? parameters = null);
        Task<int> GetCountAsync(string? where = null, object? parameters = null);
    }
}
