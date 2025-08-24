using System.Data;
using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Generic base repository implementation using Dapper
/// Provides CRUD operations and query capabilities for all entities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected readonly ILogger<BaseRepository<T>> _logger;
    protected readonly string _tableName;
    protected readonly string _idColumnName;

    protected BaseRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<BaseRepository<T>> logger,
        string tableName,
        string idColumnName = "Id")
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        _idColumnName = idColumnName;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = $"SELECT * FROM {_tableName} WHERE {_idColumnName} = @Id AND IsDeleted = 0";
            var entity = await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
            
            _logger.LogDebug("GetByIdAsync for {EntityType} with ID {Id}: {Result}", 
                typeof(T).Name, id, entity != null ? "Found" : "Not found");
            
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {EntityType} by ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = $"SELECT * FROM {_tableName} WHERE IsDeleted = 0";
            
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }
            
            var entities = await connection.QueryAsync<T>(sql, parameters);
            
            _logger.LogDebug("GetAllAsync for {EntityType}: {Count} entities returned", 
                typeof(T).Name, entities.Count());
            
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? where = null, 
        object? parameters = null, 
        string? orderBy = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Build base SQL
            var baseWhere = "IsDeleted = 0";
            if (!string.IsNullOrEmpty(where))
            {
                baseWhere += $" AND {where}";
            }
            
            // Count query
            var countSql = $"SELECT COUNT(*) FROM {_tableName} WHERE {baseWhere}";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            
            // Data query with pagination
            var offset = (pageNumber - 1) * pageSize;
            var dataSql = $"SELECT * FROM {_tableName} WHERE {baseWhere}";
            
            if (!string.IsNullOrEmpty(orderBy))
            {
                dataSql += $" ORDER BY {orderBy}";
            }
            else
            {
                dataSql += $" ORDER BY {_idColumnName}";
            }
            
            dataSql += $" OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            
            var items = await connection.QueryAsync<T>(dataSql, parameters);
            
            _logger.LogDebug("GetPagedAsync for {EntityType}: Page {Page}, Size {Size}, Total {Total}, Returned {Count}", 
                typeof(T).Name, pageNumber, pageSize, totalCount, items.Count());
            
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Generate INSERT SQL dynamically based on entity properties
            var insertSql = GenerateInsertSql(entity);
            var id = await connection.ExecuteScalarAsync<int>(insertSql, entity);
            
            // Set the generated ID back to the entity
            var idProperty = typeof(T).GetProperty(_idColumnName);
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, id);
            }
            
            _logger.LogDebug("AddAsync for {EntityType}: Entity added with ID {Id}", typeof(T).Name, id);
            
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Generate UPDATE SQL dynamically
            var updateSql = GenerateUpdateSql(entity);
            var rowsAffected = await connection.ExecuteAsync(updateSql, entity);
            
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"No rows were updated for {typeof(T).Name}");
            }
            
            _logger.LogDebug("UpdateAsync for {EntityType}: {RowsAffected} rows updated", 
                typeof(T).Name, rowsAffected);
            
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Soft delete by setting IsDeleted = 1
            var sql = $"UPDATE {_tableName} SET IsDeleted = 1, DeletedAt = @DeletedAt WHERE {_idColumnName} = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow });
            
            _logger.LogDebug("DeleteAsync for {EntityType} with ID {Id}: {RowsAffected} rows affected", 
                typeof(T).Name, id, rowsAffected);
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = $"SELECT COUNT(*) FROM {_tableName} WHERE {_idColumnName} = @Id AND IsDeleted = 0";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }



    public virtual async Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var entities = await connection.QueryAsync<T>(sql, parameters);
            
            _logger.LogDebug("QueryAsync for {EntityType}: {Count} entities returned", 
                typeof(T).Name, entities.Count());
            
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query for {EntityType}: {Sql}", typeof(T).Name, sql);
            throw;
        }
    }

    public virtual async Task<T?> QueryFirstOrDefaultAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var entity = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            
            _logger.LogDebug("QueryFirstOrDefaultAsync for {EntityType}: {Result}", 
                typeof(T).Name, entity != null ? "Found" : "Not found");
            
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query for {EntityType}: {Sql}", typeof(T).Name, sql);
            throw;
        }
    }

    public virtual async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            
            _logger.LogDebug("ExecuteAsync for {EntityType}: {RowsAffected} rows affected", 
                typeof(T).Name, rowsAffected);
            
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command for {EntityType}: {Sql}", typeof(T).Name, sql);
            throw;
        }
    }

    public virtual async Task<IDisposable> BeginTransactionAsync()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();
        return new TransactionWrapper(connection, transaction);
    }

    public virtual async Task<int> GetCountAsync(string? where = null, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = $"SELECT COUNT(*) FROM {_tableName} WHERE IsDeleted = 0";
            
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }
            
            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            
            _logger.LogDebug("GetCountAsync for {EntityType}: {Count} entities", typeof(T).Name, count);
            
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting count for {EntityType}", typeof(T).Name);
            throw;
        }
    }

    #region Helper Methods

    protected virtual string GenerateInsertSql(T entity)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.Name != _idColumnName && p.Name != "RowVersion")
            .ToList();

        var columns = string.Join(", ", properties.Select(p => p.Name));
        var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        return $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters}); SELECT CAST(SCOPE_IDENTITY() as int)";
    }

    protected virtual string GenerateUpdateSql(T entity)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.Name != _idColumnName && p.Name != "RowVersion" && p.Name != "CreatedAt" && p.Name != "CreatedBy")
            .ToList();

        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        setClause += ", ModifiedAt = @ModifiedAt";

        return $"UPDATE {_tableName} SET {setClause} WHERE {_idColumnName} = @{_idColumnName} AND IsDeleted = 0";
    }

    #endregion

    #region Transaction Wrapper

    private class TransactionWrapper : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private bool _disposed;

        public TransactionWrapper(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }

    #endregion
}
