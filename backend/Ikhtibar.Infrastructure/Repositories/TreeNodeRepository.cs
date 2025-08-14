using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Data;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TreeNode entity operations
/// Handles hierarchical tree operations with materialized path pattern
/// </summary>
public class TreeNodeRepository : ITreeNodeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<TreeNodeRepository> _logger;

    public TreeNodeRepository(IDbConnectionFactory connectionFactory, ILogger<TreeNodeRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get tree node by ID
    /// </summary>
    /// <param name="treeNodeId">Tree node ID</param>
    /// <returns>Tree node if found, null otherwise</returns>
    public async Task<TreeNode?> GetByIdAsync(int treeNodeId)
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.TreeNodeId = @TreeNodeId AND t.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNode>(sql, new { TreeNodeId = treeNodeId });

            _logger.LogDebug("Retrieved tree node by ID: {TreeNodeId}, Found: {Found}", treeNodeId, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by ID: {TreeNodeId}", treeNodeId);
            throw;
        }
    }

    /// <summary>
    /// Get tree node by code
    /// </summary>
    /// <param name="code">Tree node code</param>
    /// <returns>Tree node if found, null otherwise</returns>
    public async Task<TreeNode?> GetByCodeAsync(string code)
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.Code = @Code AND t.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNode>(sql, new { Code = code });

            _logger.LogDebug("Retrieved tree node by code: {Code}, Found: {Found}", code, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Get all tree nodes
    /// </summary>
    /// <returns>Collection of all tree nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetAllAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql);

            _logger.LogDebug("Retrieved all tree nodes, Count: {Count}", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Get root nodes (nodes without parent)
    /// </summary>
    /// <returns>Collection of root tree nodes ordered by OrderIndex</returns>
    public async Task<IEnumerable<TreeNode>> GetRootNodesAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.ParentId IS NULL AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql);

            _logger.LogDebug("Retrieved root tree nodes, Count: {Count}", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Get direct children of a parent node
    /// </summary>
    /// <param name="parentId">Parent node ID</param>
    /// <returns>Collection of child nodes ordered by OrderIndex</returns>
    public async Task<IEnumerable<TreeNode>> GetChildrenAsync(int parentId)
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.ParentId = @ParentId AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { ParentId = parentId });

            _logger.LogDebug("Retrieved children for parent: {ParentId}, Count: {Count}", parentId, result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving children for parent: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Get all descendants of a node using materialized path
    /// </summary>
    /// <param name="nodeId">Node ID to get descendants for</param>
    /// <returns>Collection of descendant nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId)
    {
        try
        {
            // First get the node to extract its path
            var node = await GetByIdAsync(nodeId);
            if (node == null) return new List<TreeNode>();

            return await GetByPathAsync($"{node.Path}{nodeId}-");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendants for node: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get all ancestors of a node using materialized path
    /// </summary>
    /// <param name="nodeId">Node ID to get ancestors for</param>
    /// <returns>Collection of ancestor nodes ordered by path</returns>
    public async Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId)
    {
        try
        {
            // First get the node to extract its path
            var node = await GetByIdAsync(nodeId);
            if (node == null) return new List<TreeNode>();

            // Extract ancestor IDs from path
            var path = node.Path.Trim('-');
            if (string.IsNullOrEmpty(path)) return new List<TreeNode>();

            var ancestorIds = path.Split('-')
                .Where(id => !string.IsNullOrEmpty(id))
                .Select(int.Parse)
                .ToList();

            if (!ancestorIds.Any()) return new List<TreeNode>();

            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.TreeNodeId IN @AncestorIds AND t.IsDeleted = 0
                ORDER BY t.Path";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { AncestorIds = ancestorIds });

            _logger.LogDebug("Retrieved ancestors for node: {NodeId}, Count: {Count}", nodeId, result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestors for node: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get nodes by path pattern (for subtree queries)
    /// </summary>
    /// <param name="pathQuery">Path pattern to search</param>
    /// <returns>Collection of nodes matching the path pattern</returns>
    public async Task<IEnumerable<TreeNode>> GetByPathAsync(string pathQuery)
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.Path LIKE @PathQuery AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { PathQuery = $"{pathQuery}%" });

            _logger.LogDebug("Retrieved nodes by path: {PathQuery}, Count: {Count}", pathQuery, result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes by path: {PathQuery}", pathQuery);
            throw;
        }
    }

    /// <summary>
    /// Get nodes by tree node type
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID</param>
    /// <returns>Collection of nodes of the specified type</returns>
    public async Task<IEnumerable<TreeNode>> GetByTypeAsync(int treeNodeTypeId)
    {
        try
        {
            const string sql = @"
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.TreeNodeTypeId = @TreeNodeTypeId AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { TreeNodeTypeId = treeNodeTypeId });

            _logger.LogDebug("Retrieved nodes by type: {TreeNodeTypeId}, Count: {Count}", treeNodeTypeId, result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes by type: {TreeNodeTypeId}", treeNodeTypeId);
            throw;
        }
    }

    /// <summary>
    /// Create a new tree node
    /// </summary>
    /// <param name="treeNode">Tree node to create</param>
    /// <returns>Created tree node with generated ID</returns>
    public async Task<TreeNode> CreateAsync(TreeNode treeNode)
    {
        try
        {
            const string sql = @"
                INSERT INTO TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, IsActive, CreatedAt, IsDeleted)
                VALUES (@Name, @Code, @Description, @TreeNodeTypeId, @ParentId, @OrderIndex, @Path, @IsActive, @CreatedAt, 0);
                SELECT t.TreeNodeId, t.Name, t.Code, t.Description, t.TreeNodeTypeId, 
                       t.ParentId, t.OrderIndex, t.Path, t.IsActive,
                       t.CreatedAt, t.ModifiedAt, t.IsDeleted,
                       tt.Name as TreeNodeTypeName
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.TreeNodeTypeId
                WHERE t.TreeNodeId = last_insert_rowid();";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstAsync<TreeNode>(sql, new
            {
                treeNode.Name,
                treeNode.Code,
                treeNode.Description,
                treeNode.TreeNodeTypeId,
                treeNode.ParentId,
                treeNode.OrderIndex,
                treeNode.Path,
                treeNode.IsActive,
                treeNode.CreatedAt
            });

            _logger.LogInformation("Created tree node: {Name} with ID: {TreeNodeId}", result.Name, result.TreeNodeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node: {Name}", treeNode.Name);
            throw;
        }
    }

    /// <summary>
    /// Update an existing tree node
    /// </summary>
    /// <param name="treeNode">Tree node to update</param>
    /// <returns>Updated tree node</returns>
    public async Task<TreeNode> UpdateAsync(TreeNode treeNode)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodes 
                SET Name = @Name, Code = @Code, Description = @Description, 
                    TreeNodeTypeId = @TreeNodeTypeId, OrderIndex = @OrderIndex, 
                    IsActive = @IsActive, ModifiedAt = @ModifiedAt
                WHERE TreeNodeId = @TreeNodeId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                treeNode.Name,
                treeNode.Code,
                treeNode.Description,
                treeNode.TreeNodeTypeId,
                treeNode.OrderIndex,
                treeNode.IsActive,
                treeNode.TreeNodeId,
                ModifiedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Updated tree node: {TreeNodeId}", treeNode.TreeNodeId);
            return treeNode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node: {TreeNodeId}", treeNode.TreeNodeId);
            throw;
        }
    }

    /// <summary>
    /// Delete a tree node (soft delete)
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    public async Task<bool> DeleteAsync(int treeNodeId)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodes 
                SET IsDeleted = 1, DeletedAt = @DeletedAt
                WHERE TreeNodeId = @TreeNodeId";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                TreeNodeId = treeNodeId,
                DeletedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Soft deleted tree node: {TreeNodeId}, Success: {Success}", treeNodeId, rowsAffected > 0);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node: {TreeNodeId}", treeNodeId);
            throw;
        }
    }

    /// <summary>
    /// Move a node to a new parent (updates paths of node and descendants)
    /// </summary>
    /// <param name="nodeId">Node ID to move</param>
    /// <param name="newParentId">New parent ID (null for root)</param>
    /// <param name="newOrderIndex">New order index</param>
    /// <returns>True if moved successfully, false otherwise</returns>
    public async Task<bool> MoveNodeAsync(int nodeId, int? newParentId, int newOrderIndex)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Get the current node
            var node = await GetByIdAsync(nodeId);
            if (node == null) return false;

            // Calculate new path
            string newPath = "-";
            if (newParentId.HasValue)
            {
                var parent = await GetByIdAsync(newParentId.Value);
                if (parent == null) return false;
                newPath = $"{parent.Path}{newParentId.Value}-";
            }

            // Update paths for all descendants first
            var oldPath = $"{node.Path}{nodeId}-";
            var newDescendantPath = $"{newPath}{nodeId}-";
            await UpdatePathsAsync(oldPath, newDescendantPath);

            // Update the node itself
            const string updateNodeSql = @"
                UPDATE TreeNodes 
                SET ParentId = @NewParentId, OrderIndex = @NewOrderIndex, Path = @NewPath, ModifiedAt = @ModifiedAt
                WHERE TreeNodeId = @NodeId";

            await connection.ExecuteAsync(updateNodeSql, new
            {
                NewParentId = newParentId,
                NewOrderIndex = newOrderIndex,
                NewPath = newPath,
                NodeId = nodeId,
                ModifiedAt = DateTime.UtcNow
            }, transaction);

            transaction.Commit();
            _logger.LogInformation("Moved tree node: {NodeId} to parent: {NewParentId}", nodeId, newParentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tree node: {NodeId} to parent: {NewParentId}", nodeId, newParentId);
            throw;
        }
    }

    /// <summary>
    /// Update paths for all descendants when a node is moved
    /// </summary>
    /// <param name="oldPath">Old materialized path</param>
    /// <param name="newPath">New materialized path</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    public async Task<bool> UpdatePathsAsync(string oldPath, string newPath)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodes
                SET Path = REPLACE(Path, @OldPath, @NewPath), ModifiedAt = @ModifiedAt
                WHERE Path LIKE @PathQuery";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                OldPath = oldPath,
                NewPath = newPath,
                PathQuery = $"{oldPath}%",
                ModifiedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Updated paths from: {OldPath} to: {NewPath}, Rows affected: {RowsAffected}", oldPath, newPath, rowsAffected);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating paths from: {OldPath} to: {NewPath}", oldPath, newPath);
            throw;
        }
    }

    /// <summary>
    /// Get the maximum order index for children of a parent
    /// </summary>
    /// <param name="parentId">Parent ID (null for root level)</param>
    /// <returns>Maximum order index</returns>
    public async Task<int> GetMaxOrderIndexAsync(int? parentId)
    {
        try
        {
            const string sql = @"
                SELECT COALESCE(MAX(OrderIndex), 0)
                FROM TreeNodes
                WHERE ParentId = @ParentId
                AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(sql, new { ParentId = parentId });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting max order index for parent: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Reorder nodes within the same parent
    /// </summary>
    /// <param name="parentId">Parent ID</param>
    /// <param name="nodeOrders">Dictionary of node ID to new order index</param>
    /// <returns>True if reordered successfully, false otherwise</returns>
    public async Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            foreach (var order in nodeOrders)
            {
                const string updateQuery = @"
                    UPDATE TreeNodes
                    SET OrderIndex = @OrderIndex, ModifiedAt = @ModifiedAt
                    WHERE TreeNodeId = @TreeNodeId AND ParentId = @ParentId";

                await connection.ExecuteAsync(updateQuery, new
                {
                    TreeNodeId = order.Key,
                    OrderIndex = order.Value,
                    ParentId = parentId,
                    ModifiedAt = DateTime.UtcNow
                }, transaction);
            }

            transaction.Commit();
            _logger.LogInformation("Reordered nodes for parent: {ParentId}, Count: {Count}", parentId, nodeOrders.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering nodes for parent: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Check if node exists
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to check</param>
    /// <returns>True if exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int treeNodeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM TreeNodes 
                WHERE TreeNodeId = @TreeNodeId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TreeNodeId = treeNodeId });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node exists: {TreeNodeId}", treeNodeId);
            throw;
        }
    }

    /// <summary>
    /// Check if node has children
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to check</param>
    /// <returns>True if has children, false otherwise</returns>
    public async Task<bool> HasChildrenAsync(int treeNodeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM TreeNodes 
                WHERE ParentId = @TreeNodeId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TreeNodeId = treeNodeId });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node has children: {TreeNodeId}", treeNodeId);
            throw;
        }
    }

    /// <summary>
    /// Get tree statistics for a node
    /// </summary>
    /// <param name="nodeId">Node ID to get statistics for</param>
    /// <returns>Tree statistics including depth, children count, etc.</returns>
    public async Task<TreeNodeStatistics> GetStatisticsAsync(int nodeId)
    {
        try
        {
            var node = await GetByIdAsync(nodeId);
            if (node == null) throw new KeyNotFoundException($"Tree node with ID {nodeId} not found");

            var children = await GetChildrenAsync(nodeId);
            var descendants = await GetDescendantsAsync(nodeId);

            // Calculate level from path
            var level = node.Path.Count(c => c == '-') - 1;

            var stats = new TreeNodeStatistics
            {
                DirectChildrenCount = children.Count(),
                TotalDescendantsCount = descendants.Count(),
                Level = level,
                // TODO: Calculate max depth and questions count when needed
                MaxDepth = 0,
                TotalQuestionsCount = 0
            };

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for tree node: {NodeId}", nodeId);
            throw;
        }
    }
}
