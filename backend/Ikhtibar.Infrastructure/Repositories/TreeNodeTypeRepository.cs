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
    /// <param name="treeNodeTypeId">Tree node type ID</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    public async Task<TreeNodeType?> GetByIdAsync(int treeNodeTypeId)
    {
        try
        {
            const string sql = @"
                SELECT TreeNodeTypeId, Name 
                FROM TreeNodeTypes 
                WHERE TreeNodeTypeId = @TreeNodeTypeId";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNodeType>(sql, new { TreeNodeTypeId = treeNodeTypeId });

            _logger.LogDebug("Retrieved tree node type by ID: {TreeNodeTypeId}, Found: {Found}", treeNodeTypeId, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by ID: {TreeNodeTypeId}", treeNodeTypeId);
            throw;
        }
    }

    /// <summary>
    /// Get tree node type by name
    /// </summary>
    /// <param name="name">Tree node type name</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    public async Task<TreeNodeType?> GetByNameAsync(string name)
    {
        try
        {
            const string sql = @"
                SELECT TreeNodeTypeId, Name 
                FROM TreeNodeTypes 
                WHERE Name = @Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNodeType>(sql, new { Name = name });

            _logger.LogDebug("Retrieved tree node type by name: {Name}, Found: {Found}", name, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by name: {Name}", name);
            throw;
        }
    }

    /// <summary>
    /// Get all tree node types
    /// </summary>
    /// <returns>Collection of all tree node types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetAllAsync()
    {
        try
        {
            const string sql = @"
                SELECT TreeNodeTypeId, Name 
                FROM TreeNodeTypes 
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved all tree node types, Count: {Count}", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree node types");
            throw;
        }
    }

    /// <summary>
    /// Get active tree node types ordered by display order
    /// </summary>
    /// <returns>Collection of active tree node types</returns>
    public async Task<IEnumerable<TreeNodeType>> GetActiveAsync()
    {
        try
        {
            const string sql = @"
                SELECT TreeNodeTypeId, Name 
                FROM TreeNodeTypes 
                ORDER BY Name";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNodeType>(sql);

            _logger.LogDebug("Retrieved active tree node types, Count: {Count}", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tree node types");
            throw;
        }
    }

    /// <summary>
    /// Create a new tree node type
    /// </summary>
    /// <param name="treeNodeType">Tree node type to create</param>
    /// <returns>Created tree node type with generated ID</returns>
    public async Task<TreeNodeType> CreateAsync(TreeNodeType treeNodeType)
    {
        try
        {
            const string sql = @"
                INSERT INTO TreeNodeTypes (Name)
                VALUES (@Name);
                SELECT TreeNodeTypeId, Name 
                FROM TreeNodeTypes 
                WHERE TreeNodeTypeId = last_insert_rowid();";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstAsync<TreeNodeType>(sql, new { treeNodeType.Name });

            _logger.LogInformation("Created tree node type: {Name} with ID: {TreeNodeTypeId}", result.Name, result.TreeNodeTypeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node type: {Name}", treeNodeType.Name);
            throw;
        }
    }

    /// <summary>
    /// Update an existing tree node type
    /// </summary>
    /// <param name="treeNodeType">Tree node type to update</param>
    /// <returns>Updated tree node type</returns>
    public async Task<TreeNodeType> UpdateAsync(TreeNodeType treeNodeType)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodeTypes 
                SET Name = @Name
                WHERE TreeNodeTypeId = @TreeNodeTypeId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { treeNodeType.Name, treeNodeType.TreeNodeTypeId });

            _logger.LogInformation("Updated tree node type: {TreeNodeTypeId}", treeNodeType.TreeNodeTypeId);
            return treeNodeType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node type: {TreeNodeTypeId}", treeNodeType.TreeNodeTypeId);
            throw;
        }
    }

    /// <summary>
    /// Delete a tree node type (hard delete for now, could be soft delete)
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    public async Task<bool> DeleteAsync(int treeNodeTypeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM TreeNodeTypes 
                WHERE TreeNodeTypeId = @TreeNodeTypeId";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new { TreeNodeTypeId = treeNodeTypeId });

            _logger.LogInformation("Deleted tree node type: {TreeNodeTypeId}, Success: {Success}", treeNodeTypeId, rowsAffected > 0);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node type: {TreeNodeTypeId}", treeNodeTypeId);
            throw;
        }
    }

    /// <summary>
    /// Check if tree node type exists
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to check</param>
    /// <returns>True if exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int treeNodeTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM TreeNodeTypes 
                WHERE TreeNodeTypeId = @TreeNodeTypeId";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TreeNodeTypeId = treeNodeTypeId });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type exists: {TreeNodeTypeId}", treeNodeTypeId);
            throw;
        }
    }

    /// <summary>
    /// Check if tree node type is in use by any nodes
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to check</param>
    /// <returns>True if in use, false otherwise</returns>
    public async Task<bool> IsInUseAsync(int treeNodeTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM TreeNodes 
                WHERE TreeNodeTypeId = @TreeNodeTypeId 
                AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TreeNodeTypeId = treeNodeTypeId });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type is in use: {TreeNodeTypeId}", treeNodeTypeId);
            throw;
        }
    }
}
