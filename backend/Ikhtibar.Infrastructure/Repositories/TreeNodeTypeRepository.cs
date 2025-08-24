using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TreeNodeType entity operations
/// Handles all tree node type-related data access operations
/// </summary>
public class TreeNodeTypeRepository : ITreeNodeTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<TreeNodeTypeRepository> _logger;

    public TreeNodeTypeRepository(IDbConnectionFactory connectionFactory, ILogger<TreeNodeTypeRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get tree node type by ID
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    public async Task<TreeNodeType?> GetByIdAsync(int id)
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE Id = @Id AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNodeType>(sql, new { Id = id });

            _logger.LogDebug("Retrieved tree node type by ID: {Id}, Found: {Found}", id, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get tree node type by code
    /// </summary>
    /// <param name="code">Tree node type code</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    public async Task<TreeNodeType?> GetByCodeAsync(string code)
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE Code = @Code AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNodeType>(sql, new { Code = code });

            _logger.LogDebug("Retrieved tree node type by code: {Code}, Found: {Found}", code, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Get all tree node types
    /// </summary>
    /// <returns>Collection of all tree node types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetAllAsync(string? where = null, object? parameters = null)
    {
        try
        {
            var baseSql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsDeleted = 0";

            var sql = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }
            sql += " ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql, parameters);

            _logger.LogDebug("Retrieved {Count} tree node types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree node types");
            throw;
        }
    }

    /// <summary>
    /// Add a new tree node type
    /// </summary>
    /// <param name="entity">Tree node type to add</param>
    /// <returns>Added tree node type with generated ID</returns>
    public async Task<TreeNodeType> AddAsync(TreeNodeType entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO TreeNodeTypes (Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                                         IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, Version)
                VALUES (@Name, @Code, @Description, @Icon, @Color, @AllowsChildren, @MaxChildren, @MaxDepth,
                        @IsSystem, @IsActive, @IsVisible, @Metadata, @CreatedAt, @CreatedBy, @Version);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.QuerySingleAsync<int>(sql, entity);
            entity.Id = id;

            _logger.LogDebug("Added tree node type with ID: {Id}", id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tree node type: {Name}", entity.Name);
            throw;
        }
    }

    /// <summary>
    /// Update an existing tree node type
    /// </summary>
    /// <param name="entity">Tree node type to update</param>
    /// <returns>Updated tree node type</returns>
    public async Task<TreeNodeType> UpdateAsync(TreeNodeType entity)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodeTypes 
                SET Name = @Name, Code = @Code, Description = @Description, Icon = @Icon, Color = @Color,
                    AllowsChildren = @AllowsChildren, MaxChildren = @MaxChildren, MaxDepth = @MaxDepth,
                    IsActive = @IsActive, IsVisible = @IsVisible, Metadata = @Metadata, 
                    ModifiedAt = @ModifiedAt, ModifiedBy = @ModifiedBy, Version = @Version + 1
                WHERE Id = @Id AND Version = @Version";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Tree node type with ID {entity.Id} was not found or has been modified by another user");
            }

            entity.Version++;
            _logger.LogDebug("Updated tree node type with ID: {Id}", entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node type: {Id}", entity.Id);
            throw;
        }
    }

    /// <summary>
    /// Delete a tree node type
    /// </summary>
    /// <param name="id">Tree node type ID to delete</param>
    /// <returns>True if deletion was successful</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodeTypes 
                SET IsDeleted = 1, DeletedAt = @DeletedAt, DeletedBy = @DeletedBy
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow, DeletedBy = "system" });

            _logger.LogDebug("Deleted tree node type with ID: {Id}, Rows affected: {RowsAffected}", id, rowsAffected);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node type: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get active tree node types
    /// </summary>
    /// <returns>Collection of active types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetActiveTypesAsync()
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsActive = 1 AND IsDeleted = 0
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} active tree node types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tree node types");
            throw;
        }
    }

    /// <summary>
    /// Get visible tree node types
    /// </summary>
    /// <returns>Collection of visible types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetVisibleTypesAsync()
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsVisible = 1 AND IsDeleted = 0
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} visible tree node types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving visible tree node types");
            throw;
        }
    }

    /// <summary>
    /// Get tree node types that allow children
    /// </summary>
    /// <returns>Collection of types that allow children</returns>
    public async Task<IEnumerable<TreeNodeType>> GetTypesThatAllowChildrenAsync()
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE AllowsChildren = 1 AND IsDeleted = 0
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} tree node types that allow children", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node types that allow children");
            throw;
        }
    }

    /// <summary>
    /// Search tree node types by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>Collection of matching types</returns>
    public async Task<IEnumerable<TreeNodeType>> SearchAsync(string searchTerm)
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE (Name LIKE @SearchPattern OR Description LIKE @SearchPattern) 
                      AND IsDeleted = 0
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql, new { SearchPattern = searchPattern });

            _logger.LogDebug("Retrieved {Count} tree node types matching search term: {SearchTerm}", result.Count(), searchTerm);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tree node types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Get system-defined tree node types
    /// </summary>
    /// <returns>Collection of system types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetSystemTypesAsync()
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsSystem = 1 AND IsDeleted = 0
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} system tree node types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system tree node types");
            throw;
        }
    }

    /// <summary>
    /// Get user-defined tree node types
    /// </summary>
    /// <returns>Collection of user-defined types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetUserDefinedTypesAsync()
    {
        try
        {
            const string sql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsSystem = 0 AND IsDeleted = 0
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} user-defined tree node types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user-defined tree node types");
            throw;
        }
    }

    /// <summary>
    /// Check if a type code is unique
    /// </summary>
    /// <param name="code">Type code to check</param>
    /// <param name="excludeId">ID to exclude from uniqueness check</param>
    /// <returns>True if the code is unique</returns>
    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM TreeNodeTypes 
                WHERE Code = @Code AND IsDeleted = 0";

            object parameters;
            string countSql;
                
            if (excludeId.HasValue)
            {
                countSql = @"
                    SELECT COUNT(1) FROM TreeNodeTypes 
                    WHERE Code = @Code AND Id != @ExcludeId AND IsDeleted = 0";
                parameters = new { Code = code, ExcludeId = excludeId.Value };
            }
            else
            {
                countSql = @"
                    SELECT COUNT(1) FROM TreeNodeTypes 
                    WHERE Code = @Code AND IsDeleted = 0";
                parameters = new { Code = code };
            }

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(countSql, parameters);

            _logger.LogDebug("Code uniqueness check for '{Code}': {IsUnique}", code, count == 0);
            return count == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking code uniqueness for: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Get the count of tree nodes using a specific type
    /// </summary>
    /// <param name="typeId">Type ID</param>
    /// <returns>Count of nodes using this type</returns>
    public async Task<int> GetNodeCountByTypeAsync(int typeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM TreeNodes 
                WHERE NodeType = @TypeId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new { TypeId = typeId });

            _logger.LogDebug("Node count for type ID {TypeId}: {Count}", typeId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting node count for type ID: {TypeId}", typeId);
            throw;
        }
    }

    /// <summary>
    /// Get tree node types with their usage statistics
    /// </summary>
    /// <returns>Collection of types with node counts</returns>
    public async Task<IEnumerable<TreeNodeType>> GetTypesWithUsageStatsAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Code, t.Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy,
                       COUNT(n.Id) as NodeCount
                FROM TreeNodeTypes t
                LEFT JOIN TreeNodes n ON t.Id = n.NodeType AND n.IsDeleted = 0
                WHERE t.IsDeleted = 0
                GROUP BY t.Id, t.Name, t.Code, t.Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                         IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                         ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                ORDER BY t.Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved {Count} tree node types with usage stats", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node types with usage stats");
            throw;
        }
    }

    #region IBaseRepository Implementation

    /// <summary>
    /// Get paged tree node types
    /// </summary>
    public async Task<(IEnumerable<TreeNodeType> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? where = null, object? parameters = null, string? orderBy = null)
    {
        try
        {
            var baseSql = @"
                SELECT Id, Name, Code, Description, Icon, Color, AllowsChildren, MaxChildren, MaxDepth,
                       IsSystem, IsActive, IsVisible, Metadata, CreatedAt, CreatedBy, 
                       ModifiedAt, ModifiedBy, Version, IsDeleted, DeletedAt, DeletedBy
                FROM TreeNodeTypes 
                WHERE IsDeleted = 0";

            var whereClause = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                whereClause += $" AND {where}";
            }

            var countSql = $"SELECT COUNT(*) FROM TreeNodeTypes WHERE IsDeleted = 0{(string.IsNullOrEmpty(where) ? "" : $" AND {where}")}";
            
            var orderByClause = string.IsNullOrEmpty(orderBy) ? "ORDER BY Name" : $"ORDER BY {orderBy}";
            var sql = $"{whereClause} {orderByClause} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            
            var totalCount = await connection.QueryFirstOrDefaultAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<TreeNodeType>(sql, parameters);

            _logger.LogDebug("Retrieved {Count} tree node types (page {Page}, size {PageSize})", items.Count(), pageNumber, pageSize);
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged tree node types");
            throw;
        }
    }

    /// <summary>
    /// Check if a tree node type exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            const string sql = "SELECT COUNT(1) FROM TreeNodeTypes WHERE Id = @Id AND IsDeleted = 0";
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type exists: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Execute custom query
    /// </summary>
    public async Task<IEnumerable<TreeNodeType>> QueryAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql, parameters);
            _logger.LogDebug("Executed custom query, returned {Count} results", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing custom query");
            throw;
        }
    }

    /// <summary>
    /// Execute custom query and return first result
    /// </summary>
    public async Task<TreeNodeType?> QueryFirstOrDefaultAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNodeType>(sql, parameters);
            _logger.LogDebug("Executed custom query, returned first result: {Found}", result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing custom query for first result");
            throw;
        }
    }

    /// <summary>
    /// Execute custom SQL command
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, parameters);
            _logger.LogDebug("Executed custom SQL command, affected {Count} rows", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing custom SQL command");
            throw;
        }
    }

    /// <summary>
    /// Get count of tree node types
    /// </summary>
    public async Task<int> GetCountAsync(string? where = null, object? parameters = null)
    {
        try
        {
            var baseSql = "SELECT COUNT(*) FROM TreeNodeTypes WHERE IsDeleted = 0";
            var sql = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
            _logger.LogDebug("Retrieved count of tree node types: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting count of tree node types");
            throw;
        }
    }

    #endregion
}
