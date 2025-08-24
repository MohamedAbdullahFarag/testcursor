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
    /// <param name="id">Tree node ID</param>
    /// <returns>Tree node if found, null otherwise</returns>
    public async Task<TreeNode?> GetByIdAsync(int id)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Id = @Id AND t.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNode>(sql, new { Id = id });

            _logger.LogDebug("Retrieved tree node by ID: {Id}, Found: {Found}", id, result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all tree nodes
    /// </summary>
    /// <returns>Collection of all tree nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetAllAsync(string? where = null, object? parameters = null)
    {
        try
        {
            var baseSql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.IsDeleted = 0";

            var sql = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }
            sql += " ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, parameters);

            _logger.LogDebug("Retrieved {Count} tree nodes", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Add a new tree node
    /// </summary>
    /// <param name="entity">Tree node to add</param>
    /// <returns>Added tree node with generated ID</returns>
    public async Task<TreeNode> AddAsync(TreeNode entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO TreeNodes (Name, Description, NodeType, ParentId, OrderIndex, Path, Level, 
                                     IsActive, IsVisible, Icon, Color, Metadata, CreatedAt, CreatedBy, Version)
                VALUES (@Name, @Description, @NodeType, @ParentId, @OrderIndex, @Path, @Level, 
                        @IsActive, @IsVisible, @Icon, @Color, @Metadata, @CreatedAt, @CreatedBy, @Version);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.QuerySingleAsync<int>(sql, entity);
            entity.Id = id;

            _logger.LogDebug("Added tree node with ID: {Id}", id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tree node: {Name}", entity.Name);
            throw;
        }
    }

    /// <summary>
    /// Update an existing tree node
    /// </summary>
    /// <param name="entity">Tree node to update</param>
    /// <returns>Updated tree node</returns>
    public async Task<TreeNode> UpdateAsync(TreeNode entity)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodes 
                SET Name = @Name, Description = @Description, NodeType = @NodeType, 
                    ParentId = @ParentId, OrderIndex = @OrderIndex, Path = @Path, Level = @Level,
                    IsActive = @IsActive, IsVisible = @IsVisible, Icon = @Icon, Color = @Color, 
                    Metadata = @Metadata, ModifiedAt = @ModifiedAt, ModifiedBy = @ModifiedBy, 
                    Version = @Version + 1
                WHERE Id = @Id AND Version = @Version";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Tree node with ID {entity.Id} was not found or has been modified by another user");
            }

            entity.Version++;
            _logger.LogDebug("Updated tree node with ID: {Id}", entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node: {Id}", entity.Id);
            throw;
        }
    }

    /// <summary>
    /// Delete a tree node
    /// </summary>
    /// <param name="id">Tree node ID to delete</param>
    /// <returns>True if deletion was successful</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            const string sql = @"
                UPDATE TreeNodes 
                SET IsDeleted = 1, DeletedAt = @DeletedAt, DeletedBy = @DeletedBy
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow, DeletedBy = "system" });

            _logger.LogDebug("Deleted tree node with ID: {Id}, Rows affected: {RowsAffected}", id, rowsAffected);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get root nodes (nodes without parent)
    /// </summary>
    /// <returns>Collection of root nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetRootNodesAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.ParentId IS NULL AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql);

            _logger.LogDebug("Retrieved {Count} root nodes", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root nodes");
            throw;
        }
    }

    /// <summary>
    /// Get children of a specific node
    /// </summary>
    /// <param name="parentId">Parent node ID</param>
    /// <returns>Collection of child nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetChildrenAsync(int parentId)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.ParentId = @ParentId AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { ParentId = parentId });

            _logger.LogDebug("Retrieved {Count} children for parent ID: {ParentId}", result.Count(), parentId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving children for parent ID: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Get descendants of a specific node
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>Collection of descendant nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Path LIKE @PathPattern AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            var node = await GetByIdAsync(nodeId);
            if (node == null)
                return Enumerable.Empty<TreeNode>();

            var pathPattern = $"{node.Path}/{nodeId}/%";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { PathPattern = pathPattern });

            _logger.LogDebug("Retrieved {Count} descendants for node ID: {NodeId}", result.Count(), nodeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendants for node ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get ancestors of a specific node
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>Collection of ancestor nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId)
    {
        try
        {
            var node = await GetByIdAsync(nodeId);
            if (node == null || string.IsNullOrEmpty(node.Path))
                return Enumerable.Empty<TreeNode>();

            var ancestorIds = node.Path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList();

            if (ancestorIds.Count == 0)
                return Enumerable.Empty<TreeNode>();

            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Id IN @AncestorIds AND t.IsDeleted = 0
                ORDER BY t.Level";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { AncestorIds = ancestorIds });

            _logger.LogDebug("Retrieved {Count} ancestors for node ID: {NodeId}", result.Count(), nodeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestors for node ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get nodes by path
    /// </summary>
    /// <param name="path">Materialized path</param>
    /// <returns>Collection of nodes matching the path</returns>
    public async Task<IEnumerable<TreeNode>> GetByPathAsync(string path)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Path = @Path AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { Path = path });

            _logger.LogDebug("Retrieved {Count} nodes by path: {Path}", result.Count(), path);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes by path: {Path}", path);
            throw;
        }
    }

    /// <summary>
    /// Get nodes by type
    /// </summary>
    /// <param name="nodeType">Node type</param>
    /// <returns>Collection of nodes of the specified type</returns>
    public async Task<IEnumerable<TreeNode>> GetByTypeAsync(string nodeType)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.NodeType = @NodeType AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { NodeType = nodeType });

            _logger.LogDebug("Retrieved {Count} nodes by type: {NodeType}", result.Count(), nodeType);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes by type: {NodeType}", nodeType);
            throw;
        }
    }

    /// <summary>
    /// Get nodes at a specific depth
    /// </summary>
    /// <param name="depth">Depth level</param>
    /// <returns>Collection of nodes at the specified depth</returns>
    public async Task<IEnumerable<TreeNode>> GetByDepthAsync(int depth)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Level = @Depth AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { Depth = depth });

            _logger.LogDebug("Retrieved {Count} nodes at depth: {Depth}", result.Count(), depth);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes at depth: {Depth}", depth);
            throw;
        }
    }

    /// <summary>
    /// Search nodes by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>Collection of matching nodes</returns>
    public async Task<IEnumerable<TreeNode>> SearchAsync(string searchTerm)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE (t.Name LIKE @SearchPattern OR t.Description LIKE @SearchPattern) 
                      AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            var searchPattern = $"%{searchTerm}%";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { SearchPattern = searchPattern });

            _logger.LogDebug("Retrieved {Count} nodes matching search term: {SearchTerm}", result.Count(), searchTerm);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching nodes with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Get next available order index for a parent
    /// </summary>
    /// <param name="parentId">Parent node ID</param>
    /// <returns>Next available order index</returns>
    public async Task<int> GetNextOrderIndexAsync(int? parentId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(MAX(OrderIndex), -1) + 1
                FROM TreeNodes
                WHERE ParentId = @ParentId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QuerySingleAsync<int>(sql, new { ParentId = parentId });

            _logger.LogDebug("Next order index for parent ID {ParentId}: {OrderIndex}", parentId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next order index for parent ID: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Update child order
    /// </summary>
    /// <param name="parentId">Parent node ID</param>
    /// <param name="childOrders">Dictionary of child ID to new order</param>
    /// <returns>True if successful</returns>
    public async Task<bool> UpdateChildOrderAsync(int parentId, IDictionary<int, int> childOrders)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var kvp in childOrders)
                {
                    const string sql = @"
                        UPDATE TreeNodes 
                        SET OrderIndex = @OrderIndex, ModifiedAt = @ModifiedAt, ModifiedBy = @ModifiedBy
                        WHERE Id = @Id AND ParentId = @ParentId";

                    await connection.ExecuteAsync(sql, new 
                    { 
                        Id = kvp.Key, 
                        OrderIndex = kvp.Value, 
                        ParentId = parentId,
                        ModifiedAt = DateTime.UtcNow,
                        ModifiedBy = "system"
                    }, transaction);
                }

                transaction.Commit();
                _logger.LogDebug("Updated child order for parent ID: {ParentId}", parentId);
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating child order for parent ID: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Get depth of a node
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>Depth level</returns>
    public async Task<int> GetDepthAsync(int nodeId)
    {
        try
        {
            var node = await GetByIdAsync(nodeId);
            return node?.Level ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting depth for node ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Check if a node has children
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>True if the node has children</returns>
    public async Task<bool> HasChildrenAsync(int nodeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM TreeNodes 
                WHERE ParentId = @NodeId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new { NodeId = nodeId });

            _logger.LogDebug("Node ID {NodeId} has children: {HasChildren}", nodeId, count > 0);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if node ID {NodeId} has children", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get child count for a node
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>Child count</returns>
    public async Task<int> GetChildCountAsync(int nodeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM TreeNodes 
                WHERE ParentId = @NodeId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new { NodeId = nodeId });

            _logger.LogDebug("Child count for node ID {NodeId}: {Count}", nodeId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting child count for node ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get descendant count for a node
    /// </summary>
    /// <param name="nodeId">Node ID</param>
    /// <returns>Descendant count</returns>
    public async Task<int> GetDescendantCountAsync(int nodeId)
    {
        try
        {
            var descendants = await GetDescendantsAsync(nodeId);
            return descendants.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting descendant count for node ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Get nodes by parent path and type
    /// </summary>
    /// <param name="parentPath">Parent path</param>
    /// <param name="nodeType">Node type</param>
    /// <returns>Collection of matching nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetByParentPathAndTypeAsync(string parentPath, string nodeType)
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.Path = @ParentPath AND t.NodeType = @NodeType AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, new { ParentPath = parentPath, NodeType = nodeType });

            _logger.LogDebug("Retrieved {Count} nodes by parent path and type: {ParentPath}, {NodeType}", result.Count(), parentPath, nodeType);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nodes by parent path and type: {ParentPath}, {NodeType}", parentPath, nodeType);
            throw;
        }
    }

    /// <summary>
    /// Get active nodes
    /// </summary>
    /// <returns>Collection of active nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetActiveNodesAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.IsActive = 1 AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql);

            _logger.LogDebug("Retrieved {Count} active nodes", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active nodes");
            throw;
        }
    }

    /// <summary>
    /// Get visible nodes
    /// </summary>
    /// <returns>Collection of visible nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetVisibleNodesAsync()
    {
        try
        {
            const string sql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.IsVisible = 1 AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql);

            _logger.LogDebug("Retrieved {Count} visible nodes", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving visible nodes");
            throw;
        }
    }

    #region IBaseRepository Implementation

    /// <summary>
    /// Get paged tree nodes
    /// </summary>
    public async Task<(IEnumerable<TreeNode> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? where = null, object? parameters = null, string? orderBy = null)
    {
        try
        {
            var baseSql = @"
                SELECT t.Id, t.Name, t.Description, t.NodeType, 
                       t.ParentId, t.OrderIndex, t.Path, t.Level, t.IsActive, t.IsVisible,
                       t.Icon, t.Color, t.Metadata, t.CreatedAt, t.CreatedBy, 
                       t.ModifiedAt, t.ModifiedBy, t.Version, t.IsDeleted, t.DeletedAt, t.DeletedBy
                FROM TreeNodes t
                WHERE t.IsDeleted = 0";

            var whereClause = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                whereClause += $" AND {where}";
            }

            var countSql = $"SELECT COUNT(*) FROM TreeNodes t WHERE t.IsDeleted = 0{(string.IsNullOrEmpty(where) ? "" : $" AND {where}")}";
            
            var orderByClause = string.IsNullOrEmpty(orderBy) ? "ORDER BY t.Path, t.OrderIndex" : $"ORDER BY {orderBy}";
            var sql = $"{whereClause} {orderByClause} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            
            var totalCount = await connection.QueryFirstOrDefaultAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<TreeNode>(sql, parameters);

            _logger.LogDebug("Retrieved {Count} tree nodes (page {Page}, size {PageSize})", items.Count(), pageNumber, pageSize);
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Check if a tree node exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            const string sql = "SELECT COUNT(1) FROM TreeNodes WHERE Id = @Id AND IsDeleted = 0";
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node exists: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Execute custom query
    /// </summary>
    public async Task<IEnumerable<TreeNode>> QueryAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<TreeNode>(sql, parameters);
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
    public async Task<TreeNode?> QueryFirstOrDefaultAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TreeNode>(sql, parameters);
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
    /// Get count of tree nodes
    /// </summary>
    public async Task<int> GetCountAsync(string? where = null, object? parameters = null)
    {
        try
        {
            var baseSql = "SELECT COUNT(*) FROM TreeNodes t WHERE t.IsDeleted = 0";
            var sql = baseSql;
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" AND {where}";
            }

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
            _logger.LogDebug("Retrieved count of tree nodes: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting count of tree nodes");
            throw;
        }
    }

    #endregion
}
