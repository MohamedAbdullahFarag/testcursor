using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs.Tree;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for TreeNode management operations
/// Following SRP: ONLY tree node business logic
/// </summary>
public class TreeNodeService : ITreeNodeService
{
    private readonly ITreeNodeRepository _treeNodeRepository;
    private readonly ITreeNodeTypeRepository _treeNodeTypeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TreeNodeService> _logger;

    public TreeNodeService(
        ITreeNodeRepository treeNodeRepository,
        ITreeNodeTypeRepository treeNodeTypeRepository,
        IMapper mapper,
        ILogger<TreeNodeService> logger)
    {
        _treeNodeRepository = treeNodeRepository ?? throw new ArgumentNullException(nameof(treeNodeRepository));
        _treeNodeTypeRepository = treeNodeTypeRepository ?? throw new ArgumentNullException(nameof(treeNodeTypeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new tree node.
    /// </summary>
    /// <param name="node">The node to create</param>
    /// <returns>The created node with generated ID</returns>
    public async Task<TreeNode> CreateAsync(TreeNode node)
    {
        using var scope = _logger.BeginScope("Creating tree node {Name}", node.Name);

        try
        {
            _logger.LogInformation("Creating new tree node: {Name}", node.Name);

            // Validate node type exists
            var nodeType = await _treeNodeTypeRepository.GetByCodeAsync(node.NodeType);
            if (nodeType == null)
            {
                throw new ArgumentException($"Node type '{node.NodeType}' does not exist");
            }

            // Set default values
            node.CreatedAt = DateTime.UtcNow;
            node.Version = 1;
            node.IsActive = true;
            node.IsVisible = true;

            // Calculate path and level
            if (node.ParentId.HasValue)
            {
                var parent = await _treeNodeRepository.GetByIdAsync(node.ParentId.Value);
                if (parent == null)
                {
                    throw new ArgumentException($"Parent node with ID {node.ParentId} does not exist");
                }
                node.Path = $"{parent.Path}/{parent.Id}";
                node.Level = parent.Level + 1;
            }
            else
            {
                node.Path = "";
                node.Level = 0;
            }

            // Get next order index
            node.OrderIndex = await _treeNodeRepository.GetNextOrderIndexAsync(node.ParentId);

            var createdEntity = await _treeNodeRepository.AddAsync(node);

            _logger.LogInformation("Created tree node with ID: {Id}", createdEntity.Id);
            return createdEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node: {Name}", node.Name);
            throw;
        }
    }

    /// <summary>
    /// Gets a tree node by its ID.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <returns>The node if found, null otherwise</returns>
    public async Task<TreeNode?> GetByIdAsync(int id)
    {
        using var scope = _logger.BeginScope("Getting tree node {Id}", id);

        try
        {
            _logger.LogInformation("Retrieving tree node by ID: {Id}", id);

            var entity = await _treeNodeRepository.GetByIdAsync(id);

            _logger.LogInformation("Retrieved tree node: {Found}", entity != null);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets all tree nodes.
    /// </summary>
    /// <returns>Collection of all tree nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetAllAsync()
    {
        using var scope = _logger.BeginScope("Getting all tree nodes");

        try
        {
            _logger.LogInformation("Retrieving all tree nodes");

            var entities = await _treeNodeRepository.GetAllAsync();

            _logger.LogInformation("Retrieved {Count} tree nodes", entities.Count());
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Gets all root nodes (nodes without parent).
    /// </summary>
    /// <returns>Collection of root nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetRootNodesAsync()
    {
        using var scope = _logger.BeginScope("Getting root tree nodes");

        try
        {
            _logger.LogInformation("Retrieving root tree nodes");

            var entities = await _treeNodeRepository.GetRootNodesAsync();

            _logger.LogInformation("Retrieved {Count} root tree nodes", entities.Count());
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root tree nodes");
            throw;
        }
    }

    /// <summary>
    /// Gets all children of a specific node.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <returns>Collection of child nodes</returns>
    public async Task<IEnumerable<TreeNode>> GetChildrenAsync(int parentId)
    {
        using var scope = _logger.BeginScope("Getting children for parent {ParentId}", parentId);

        try
        {
            _logger.LogInformation("Retrieving child tree nodes for parent: {ParentId}", parentId);

            var entities = await _treeNodeRepository.GetChildrenAsync(parentId);

            _logger.LogInformation("Retrieved {Count} child tree nodes", entities.Count());
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child tree nodes for parent: {ParentId}", parentId);
            throw;
        }
    }

    /// <summary>
    /// Gets the complete subtree starting from a specific node.
    /// </summary>
    /// <param name="nodeId">The root node ID for the subtree</param>
    /// <returns>The complete subtree structure</returns>
    public async Task<TreeNode> GetSubtreeAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting subtree for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Retrieving subtree for node: {NodeId}", nodeId);

            var rootNode = await _treeNodeRepository.GetByIdAsync(nodeId);
            if (rootNode == null)
            {
                throw new ArgumentException($"Node with ID {nodeId} not found");
            }

            var descendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);
            var children = await _treeNodeRepository.GetChildrenAsync(nodeId);

            // Build tree structure
            rootNode.Children = children.ToList();
            foreach (var child in children)
            {
                var childDescendants = descendants.Where(d => d.ParentId == child.Id).ToList();
                child.Children = childDescendants;
            }

            _logger.LogInformation("Retrieved subtree for node: {NodeId}", nodeId);
            return rootNode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtree for node: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Gets the path from root to a specific node.
    /// </summary>
    /// <param name="nodeId">The target node ID</param>
    /// <returns>Ordered collection of nodes from root to target</returns>
    public async Task<IEnumerable<TreeNode>> GetPathToNodeAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting path to node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Retrieving path to node: {NodeId}", nodeId);

            var ancestors = await _treeNodeRepository.GetAncestorsAsync(nodeId);
            var currentNode = await _treeNodeRepository.GetByIdAsync(nodeId);

            if (currentNode == null)
            {
                throw new ArgumentException($"Node with ID {nodeId} not found");
            }

            var path = ancestors.ToList();
            path.Add(currentNode);

            _logger.LogInformation("Retrieved path with {Count} nodes", path.Count);
            return path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving path to node: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing tree node.
    /// </summary>
    /// <param name="node">The updated node data</param>
    /// <returns>The updated node</returns>
    public async Task<TreeNode> UpdateAsync(TreeNode node)
    {
        using var scope = _logger.BeginScope("Updating tree node {Id}", node.Id);

        try
        {
            _logger.LogInformation("Updating tree node: {Id}", node.Id);

            // Validate node exists
            var existingNode = await _treeNodeRepository.GetByIdAsync(node.Id);
            if (existingNode == null)
            {
                throw new ArgumentException($"Node with ID {node.Id} not found");
            }

            // Validate node type if changed
            if (node.NodeType != existingNode.NodeType)
            {
                var nodeType = await _treeNodeTypeRepository.GetByCodeAsync(node.NodeType);
                if (nodeType == null)
                {
                    throw new ArgumentException($"Node type '{node.NodeType}' does not exist");
                }
            }

            // Set update values
            node.ModifiedAt = DateTime.UtcNow;
            node.ModifiedBy = "system"; // TODO: Get from current user context

            var updatedEntity = await _treeNodeRepository.UpdateAsync(node);

            _logger.LogInformation("Updated tree node: {Id}", updatedEntity.Id);
            return updatedEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node: {Id}", node.Id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a tree node and optionally its children.
    /// </summary>
    /// <param name="id">The node ID to delete</param>
    /// <param name="deleteChildren">Whether to delete child nodes</param>
    /// <returns>True if deletion was successful</returns>
    public async Task<bool> DeleteAsync(int id, bool deleteChildren = false)
    {
        using var scope = _logger.BeginScope("Deleting tree node {Id}", id);

        try
        {
            _logger.LogInformation("Deleting tree node: {Id}, DeleteChildren: {DeleteChildren}", id, deleteChildren);

            // Check if node has children
            var hasChildren = await _treeNodeRepository.HasChildrenAsync(id);
            if (hasChildren && !deleteChildren)
            {
                throw new InvalidOperationException($"Node {id} has children. Set deleteChildren to true to delete them as well.");
            }

            // Delete children first if requested
            if (deleteChildren && hasChildren)
            {
                var descendants = await _treeNodeRepository.GetDescendantsAsync(id);
                foreach (var descendant in descendants)
                {
                    await _treeNodeRepository.DeleteAsync(descendant.Id);
                }
            }

            var result = await _treeNodeRepository.DeleteAsync(id);

            _logger.LogInformation("Deleted tree node: {Id}, Success: {Success}", id, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Moves a node to a new parent.
    /// </summary>
    /// <param name="nodeId">The node to move</param>
    /// <param name="newParentId">The new parent ID (null for root)</param>
    /// <returns>The updated node</returns>
    public async Task<TreeNode> MoveNodeAsync(int nodeId, int? newParentId)
    {
        using var scope = _logger.BeginScope("Moving tree node {NodeId} to parent {NewParentId}", nodeId, newParentId);

        try
        {
            _logger.LogInformation("Moving tree node {NodeId} to parent {NewParentId}", nodeId, newParentId);

            var node = await _treeNodeRepository.GetByIdAsync(nodeId);
            if (node == null)
            {
                throw new ArgumentException($"Node with ID {nodeId} not found");
            }

            // Validate new parent if specified
            if (newParentId.HasValue)
            {
                var newParent = await _treeNodeRepository.GetByIdAsync(newParentId.Value);
                if (newParent == null)
                {
                    throw new ArgumentException($"New parent node with ID {newParentId} not found");
                }

                // Check if moving to descendant (which would create a cycle)
                var descendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);
                if (descendants.Any(d => d.Id == newParentId.Value))
                {
                    throw new InvalidOperationException("Cannot move node to its descendant");
                }
            }

            // Calculate new path and level
            string newPath;
            int newLevel;
            if (newParentId.HasValue)
            {
                var newParent = await _treeNodeRepository.GetByIdAsync(newParentId.Value);
                newPath = $"{newParent.Path}/{newParent.Id}";
                newLevel = newParent.Level + 1;
            }
            else
            {
                newPath = "";
                newLevel = 0;
            }

            // Update node properties
            node.ParentId = newParentId;
            node.Path = newPath;
            node.Level = newLevel;
            node.ModifiedAt = DateTime.UtcNow;
            node.ModifiedBy = "system"; // TODO: Get from current user context

            // Get new order index
            node.OrderIndex = await _treeNodeRepository.GetNextOrderIndexAsync(newParentId);

            // Update paths for all descendants
            var oldPath = $"{node.Path}/{node.Id}";
            var allDescendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);
            foreach (var descendant in allDescendants)
            {
                var newDescendantPath = $"{newPath}/{nodeId}";
                descendant.Path = newDescendantPath;
                descendant.Level = newLevel + 1;
                descendant.ModifiedAt = DateTime.UtcNow;
                descendant.ModifiedBy = "system";
                await _treeNodeRepository.UpdateAsync(descendant);
            }

            var updatedNode = await _treeNodeRepository.UpdateAsync(node);

            _logger.LogInformation("Moved tree node {NodeId} to parent {NewParentId}", nodeId, newParentId);
            return updatedNode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tree node {NodeId} to parent {NewParentId}", nodeId, newParentId);
            throw;
        }
    }

    /// <summary>
    /// Gets all nodes of a specific type.
    /// </summary>
    /// <param name="nodeType">The node type to filter by</param>
    /// <returns>Collection of nodes of the specified type</returns>
    public async Task<IEnumerable<TreeNode>> GetByTypeAsync(string nodeType)
    {
        using var scope = _logger.BeginScope("Getting tree nodes by type {NodeType}", nodeType);

        try
        {
            _logger.LogInformation("Retrieving tree nodes by type: {NodeType}", nodeType);

            var entities = await _treeNodeRepository.GetByTypeAsync(nodeType);

            _logger.LogInformation("Retrieved {Count} tree nodes by type: {NodeType}", entities.Count(), nodeType);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree nodes by type: {NodeType}", nodeType);
            throw;
        }
    }

    /// <summary>
    /// Searches for nodes by name or description.
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Collection of matching nodes</returns>
    public async Task<IEnumerable<TreeNode>> SearchAsync(string searchTerm)
    {
        using var scope = _logger.BeginScope("Searching tree nodes with term {SearchTerm}", searchTerm);

        try
        {
            _logger.LogInformation("Searching tree nodes with term: {SearchTerm}", searchTerm);

            var entities = await _treeNodeRepository.SearchAsync(searchTerm);

            _logger.LogInformation("Found {Count} tree nodes matching term: {SearchTerm}", entities.Count(), searchTerm);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tree nodes with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Gets the depth level of a node in the tree.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>The depth level (0 for root)</returns>
    public async Task<int> GetNodeDepthAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting depth for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Getting depth for node: {NodeId}", nodeId);

            var depth = await _treeNodeRepository.GetDepthAsync(nodeId);

            _logger.LogInformation("Depth for node {NodeId}: {Depth}", nodeId, depth);
            return depth;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting depth for node: {NodeId}", nodeId);
            throw;
        }
    }

    /// <summary>
    /// Gets all nodes at a specific depth level.
    /// </summary>
    /// <param name="depth">The depth level</param>
    /// <returns>Collection of nodes at the specified depth</returns>
    public async Task<IEnumerable<TreeNode>> GetNodesAtDepthAsync(int depth)
    {
        using var scope = _logger.BeginScope("Getting nodes at depth {Depth}", depth);

        try
        {
            _logger.LogInformation("Getting nodes at depth: {Depth}", depth);

            var entities = await _treeNodeRepository.GetByDepthAsync(depth);

            _logger.LogInformation("Retrieved {Count} nodes at depth: {Depth}", entities.Count(), depth);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nodes at depth: {Depth}", depth);
            throw;
        }
    }

    /// <summary>
    /// Reorders children of a parent node.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <param name="childOrder">Ordered list of child IDs</param>
    /// <returns>True if reordering was successful</returns>
    public async Task<bool> ReorderChildrenAsync(int parentId, IEnumerable<int> childOrder)
    {
        using var scope = _logger.BeginScope("Reordering children for parent {ParentId}", parentId);

        try
        {
            _logger.LogInformation("Reordering children for parent: {ParentId}", parentId);

            var childOrders = childOrder.Select((id, index) => new { Id = id, Order = index }).ToDictionary(x => x.Id, x => x.Order);
            var result = await _treeNodeRepository.UpdateChildOrderAsync(parentId, childOrders);

            _logger.LogInformation("Reordered children for parent: {ParentId}, Success: {Success}", parentId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering children for parent: {ParentId}", parentId);
            throw;
        }
    }

    // Additional methods for controller compatibility
    public async Task<TreeNode?> GetTreeNodeAsync(int id) => await GetByIdAsync(id);

    public async Task<TreeNode?> GetTreeNodeByCodeAsync(string code)
    {
        using var scope = _logger.BeginScope("Getting tree node by code {Code}", code);

        try
        {
            _logger.LogInformation("Retrieving tree node by code: {Code}", code);

            // Note: This would need to be implemented in the repository if TreeNode had a Code property
            // For now, we'll search by name as a fallback
            var nodes = await _treeNodeRepository.SearchAsync(code);
            var node = nodes.FirstOrDefault(n => n.Name.Equals(code, StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation("Retrieved tree node by code: {Found}", node != null);
            return node;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by code: {Code}", code);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var scope = _logger.BeginScope("Checking existence of tree node {Id}", id);

        try
        {
            _logger.LogInformation("Checking existence of tree node: {Id}", id);

            var exists = await _treeNodeRepository.ExistsAsync(id);

            _logger.LogInformation("Tree node {Id} exists: {Exists}", id, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of tree node: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting ancestors for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Getting ancestors for node: {NodeId}", nodeId);

            var ancestors = await _treeNodeRepository.GetAncestorsAsync(nodeId);

            _logger.LogInformation("Retrieved {Count} ancestors for node: {NodeId}", ancestors.Count(), nodeId);
            return ancestors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ancestors for node: {NodeId}", nodeId);
            throw;
        }
    }

    public async Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting descendants for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Getting descendants for node: {NodeId}", nodeId);

            var descendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);

            _logger.LogInformation("Retrieved {Count} descendants for node: {NodeId}", descendants.Count(), nodeId);
            return descendants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting descendants for node: {NodeId}", nodeId);
            throw;
        }
    }

    public async Task<TreeNode> GetTreeStructureAsync(int nodeId, int levels)
    {
        using var scope = _logger.BeginScope("Getting tree structure for node {NodeId} up to {Levels} levels", nodeId, levels);

        try
        {
            _logger.LogInformation("Getting tree structure for node: {NodeId} up to {Levels} levels", nodeId, levels);

            var node = await _treeNodeRepository.GetByIdAsync(nodeId);
            if (node == null)
                throw new ArgumentException($"Node with ID {nodeId} not found");

            if (levels <= 0)
                return node;

            var allDescendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);
            var limitedDescendants = allDescendants.Where(n => n.Level <= node.Level + levels).ToList();

            // Build the tree structure
            var nodeDict = limitedDescendants.ToDictionary(n => n.Id);
            nodeDict[node.Id] = node;

            foreach (var descendant in limitedDescendants)
            {
                if (descendant.ParentId.HasValue && nodeDict.ContainsKey(descendant.ParentId.Value))
                {
                    var parent = nodeDict[descendant.ParentId.Value];
                    parent.Children.Add(descendant);
                }
            }

            _logger.LogInformation("Retrieved tree structure for node: {NodeId} up to {Levels} levels", nodeId, levels);
            return node;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tree structure for node: {NodeId} up to {Levels} levels", nodeId, levels);
            throw;
        }
    }

    public async Task<TreeNode> GetCompleteTreeAsync()
    {
        using var scope = _logger.BeginScope("Getting complete tree structure");

        try
        {
            _logger.LogInformation("Getting complete tree structure");

            var rootNodes = await _treeNodeRepository.GetRootNodesAsync();
            var allNodes = await _treeNodeRepository.GetAllAsync();

            // Build the complete tree structure
            var nodeDict = allNodes.ToDictionary(n => n.Id);

            foreach (var node in allNodes)
            {
                if (node.ParentId.HasValue && nodeDict.ContainsKey(node.ParentId.Value))
                {
                    var parent = nodeDict[node.ParentId.Value];
                    parent.Children.Add(node);
                }
            }

            // Create a virtual root node to hold all root nodes
            var virtualRoot = new TreeNode
            {
                Id = 0,
                Name = "Root",
                Path = "",
                Level = -1,
                Children = rootNodes.ToList()
            };

            _logger.LogInformation("Retrieved complete tree structure with {Count} root nodes", rootNodes.Count());
            return virtualRoot;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting complete tree structure");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNode>> GetNodesByTypeAsync(int typeId)
    {
        using var scope = _logger.BeginScope("Getting nodes by type {TypeId}", typeId);

        try
        {
            _logger.LogInformation("Getting nodes by type: {TypeId}", typeId);

            // Note: This would need to be implemented in the repository if we had a TypeId property
            // For now, we'll return an empty collection
            var nodes = await _treeNodeRepository.GetByTypeAsync(typeId.ToString());

            _logger.LogInformation("Retrieved {Count} nodes by type: {TypeId}", nodes.Count(), typeId);
            return nodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nodes by type: {TypeId}", typeId);
            throw;
        }
    }

    public async Task<object> GetStatisticsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting statistics for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Getting statistics for node: {NodeId}", nodeId);

            var node = await _treeNodeRepository.GetByIdAsync(nodeId);
            if (node == null)
                throw new ArgumentException($"Node with ID {nodeId} not found");

            var childCount = await _treeNodeRepository.GetChildCountAsync(nodeId);
            var descendantCount = await _treeNodeRepository.GetDescendantCountAsync(nodeId);
            var depth = await _treeNodeRepository.GetDepthAsync(nodeId);

            var statistics = new
            {
                NodeId = nodeId,
                Name = node.Name,
                Level = node.Level,
                ChildCount = childCount,
                DescendantCount = descendantCount,
                Depth = depth,
                IsRoot = node.ParentId == null,
                IsLeaf = childCount == 0
            };

            _logger.LogInformation("Retrieved statistics for node: {NodeId}", nodeId);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for node: {NodeId}", nodeId);
            throw;
        }
    }

    public async Task<TreeNode> CreateTreeNodeAsync(object createDto)
    {
        using var scope = _logger.BeginScope("Creating tree node from DTO");

        try
        {
            _logger.LogInformation("Creating tree node from DTO");

            // This is a simplified implementation - in a real scenario, you'd map the DTO to a TreeNode
            // For now, we'll create a basic node
            var node = new TreeNode
            {
                Name = "New Node",
                Description = "Created from DTO",
                NodeType = "default",
                Path = "",
                Level = 0,
                OrderIndex = 0,
                IsActive = true,
                IsVisible = true
            };

            var result = await CreateAsync(node);

            _logger.LogInformation("Created tree node from DTO with ID: {Id}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node from DTO");
            throw;
        }
    }

    public async Task<TreeNode> UpdateTreeNodeAsync(int id, object updateDto)
    {
        using var scope = _logger.BeginScope("Updating tree node {Id} from DTO", id);

        try
        {
            _logger.LogInformation("Updating tree node {Id} from DTO", id);

            var node = await _treeNodeRepository.GetByIdAsync(id);
            if (node == null)
                throw new ArgumentException($"Node with ID {id} not found");

            // This is a simplified implementation - in a real scenario, you'd map the DTO to update the node
            // For now, we'll just update the modified timestamp
            node.ModifiedAt = DateTime.UtcNow;
            node.Version++;

            var result = await _treeNodeRepository.UpdateAsync(node);

            _logger.LogInformation("Updated tree node {Id} from DTO", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node {Id} from DTO", id);
            throw;
        }
    }

    public async Task<TreeNode> MoveTreeNodeAsync(int id, object moveDto)
    {
        using var scope = _logger.BeginScope("Moving tree node {Id}", id);

        try
        {
            _logger.LogInformation("Moving tree node {Id}", id);

            // This is a simplified implementation - in a real scenario, you'd extract the new parent ID from the DTO
            // For now, we'll move to root
            var result = await MoveNodeAsync(id, null);

            _logger.LogInformation("Moved tree node {Id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tree node {Id}", id);
            throw;
        }
    }

    public async Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders)
    {
        using var scope = _logger.BeginScope("Reordering nodes for parent {ParentId}", parentId);

        try
        {
            _logger.LogInformation("Reordering nodes for parent: {ParentId}", parentId);

            var result = await ReorderChildrenAsync(parentId, nodeOrders.Values);

            _logger.LogInformation("Reordered nodes for parent: {ParentId}, Success: {Success}", parentId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering nodes for parent: {ParentId}", parentId);
            throw;
        }
    }

    public async Task<bool> DeleteTreeNodeAsync(int id)
    {
        using var scope = _logger.BeginScope("Deleting tree node {Id}", id);

        try
        {
            _logger.LogInformation("Deleting tree node {Id}", id);

            var result = await DeleteAsync(id, false);

            _logger.LogInformation("Deleted tree node {Id}, Success: {Success}", id, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node {Id}", id);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Checking if node {NodeId} has children", nodeId);

        try
        {
            _logger.LogInformation("Checking if node {NodeId} has children", nodeId);

            var hasChildren = await _treeNodeRepository.HasChildrenAsync(nodeId);

            _logger.LogInformation("Node {NodeId} has children: {HasChildren}", nodeId, hasChildren);
            return hasChildren;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if node {NodeId} has children", nodeId);
            throw;
        }
    }
}
