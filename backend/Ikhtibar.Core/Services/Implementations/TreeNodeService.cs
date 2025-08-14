using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

using Ikhtibar.Shared.Models;
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

    public async Task<IEnumerable<TreeNodeDto>> GetAllAsync()
    {
        using var scope = _logger.BeginScope("Getting all tree nodes");

        try
        {
            _logger.LogInformation("Retrieving all tree nodes");

            var entities = await _treeNodeRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} tree nodes", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree nodes");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNodeDto>> GetRootNodesAsync()
    {
        using var scope = _logger.BeginScope("Getting root tree nodes");

        try
        {
            _logger.LogInformation("Retrieving root tree nodes");

            var entities = await _treeNodeRepository.GetRootNodesAsync();
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} root tree nodes", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root tree nodes");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(int parentId)
    {
        using var scope = _logger.BeginScope("Getting children for parent {ParentId}", parentId);

        try
        {
            _logger.LogInformation("Retrieving child tree nodes");

            var entities = await _treeNodeRepository.GetChildrenAsync(parentId);
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} child tree nodes", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child tree nodes");
            throw;
        }
    }

    public async Task<TreeNodeDetailDto?> GetTreeNodeAsync(int treeNodeId)
    {
        using var scope = _logger.BeginScope("Getting tree node {TreeNodeId}", treeNodeId);

        try
        {
            _logger.LogInformation("Retrieving tree node with details");

            var entity = await _treeNodeRepository.GetByIdAsync(treeNodeId);
            if (entity == null)
            {
                _logger.LogWarning("Tree node not found");
                return null;
            }

            var dto = _mapper.Map<TreeNodeDetailDto>(entity);

            // Load children
            var children = await _treeNodeRepository.GetChildrenAsync(treeNodeId);
            dto.Children = _mapper.Map<IEnumerable<TreeNodeDto>>(children);

            // TODO: Load curriculum alignments when CurriculumAlignmentRepository is available
            // TODO: Load questions count when QuestionRepository is available

            _logger.LogInformation("Tree node retrieved successfully with {ChildrenCount} children", dto.Children.Count());
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node");
            throw;
        }
    }

    public async Task<TreeNodeDto?> GetTreeNodeByCodeAsync(string code)
    {
        using var scope = _logger.BeginScope("Getting tree node by code {Code}", code);

        try
        {
            _logger.LogInformation("Retrieving tree node by code");

            var entity = await _treeNodeRepository.GetByCodeAsync(code);
            if (entity == null)
            {
                _logger.LogWarning("Tree node not found");
                return null;
            }

            var dto = _mapper.Map<TreeNodeDto>(entity);
            _logger.LogInformation("Tree node retrieved successfully");
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by code");
            throw;
        }
    }

    public async Task<TreeNodeDetailDto?> GetTreeStructureAsync(int rootId, int levels = 1)
    {
        using var scope = _logger.BeginScope("Getting tree structure from root {RootId} with {Levels} levels", rootId, levels);

        try
        {
            _logger.LogInformation("Retrieving tree structure");

            var root = await _treeNodeRepository.GetByIdAsync(rootId);
            if (root == null)
            {
                _logger.LogWarning("Root tree node not found");
                return null;
            }

            var result = _mapper.Map<TreeNodeDetailDto>(root);

            if (levels > 0)
            {
                await LoadChildrenRecursively(result, rootId, levels, 1);
            }

            _logger.LogInformation("Tree structure retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree structure");
            throw;
        }
    }

    private async Task LoadChildrenRecursively(TreeNodeDetailDto parentDto, int parentId, int maxLevels, int currentLevel)
    {
        var children = await _treeNodeRepository.GetChildrenAsync(parentId);
        var childrenDtos = _mapper.Map<IEnumerable<TreeNodeDetailDto>>(children);
        parentDto.Children = childrenDtos;

        if (currentLevel < maxLevels)
        {
            foreach (var childDto in childrenDtos)
            {
                await LoadChildrenRecursively(childDto, childDto.TreeNodeId, maxLevels, currentLevel + 1);
            }
        }
    }

    public async Task<IEnumerable<TreeNodeDto>> GetAncestorsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting ancestors for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Retrieving ancestor tree nodes");

            var entities = await _treeNodeRepository.GetAncestorsAsync(nodeId);
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} ancestor tree nodes", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestor tree nodes");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNodeDto>> GetDescendantsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting descendants for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Retrieving descendant tree nodes");

            var entities = await _treeNodeRepository.GetDescendantsAsync(nodeId);
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} descendant tree nodes", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendant tree nodes");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNodeDto>> GetNodesByTypeAsync(int treeNodeTypeId)
    {
        using var scope = _logger.BeginScope("Getting nodes by type {TreeNodeTypeId}", treeNodeTypeId);

        try
        {
            _logger.LogInformation("Retrieving tree nodes by type");

            var entities = await _treeNodeRepository.GetByTypeAsync(treeNodeTypeId);
            var dtos = _mapper.Map<IEnumerable<TreeNodeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} tree nodes of type", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree nodes by type");
            throw;
        }
    }

    public async Task<TreeNodeDto> CreateTreeNodeAsync(CreateTreeNodeDto createDto)
    {
        using var scope = _logger.BeginScope("Creating tree node {Name}", createDto.Name);

        try
        {
            _logger.LogInformation("Starting tree node creation process");

            // Business validation: Check if tree node type exists
            var nodeTypeExists = await _treeNodeTypeRepository.ExistsAsync(createDto.TreeNodeTypeId);
            if (!nodeTypeExists)
            {
                _logger.LogWarning("Tree node type not found");
                throw new KeyNotFoundException($"Tree node type with ID {createDto.TreeNodeTypeId} not found");
            }

            // Business validation: Check if parent exists (if provided)
            string path = "-";
            if (createDto.ParentId.HasValue)
            {
                var parent = await _treeNodeRepository.GetByIdAsync(createDto.ParentId.Value);
                if (parent == null)
                {
                    _logger.LogWarning("Parent tree node not found");
                    throw new KeyNotFoundException($"Parent tree node with ID {createDto.ParentId.Value} not found");
                }
                path = $"{parent.Path}{createDto.ParentId.Value}-";
            }

            // Business validation: Check if code is unique
            var existingNode = await _treeNodeRepository.GetByCodeAsync(createDto.Code);
            if (existingNode != null)
            {
                _logger.LogWarning("Tree node code already exists");
                throw new InvalidOperationException($"Tree node with code '{createDto.Code}' already exists");
            }

            // Set order index if not provided
            if (createDto.OrderIndex <= 0)
            {
                createDto.OrderIndex = await _treeNodeRepository.GetMaxOrderIndexAsync(createDto.ParentId) + 1;
            }

            // Map and create entity
            var entity = _mapper.Map<TreeNode>(createDto);
            entity.Path = path;
            entity.CreatedAt = DateTime.UtcNow;

            var createdEntity = await _treeNodeRepository.CreateAsync(entity);
            var resultDto = _mapper.Map<TreeNodeDto>(createdEntity);

            _logger.LogInformation("Tree node created successfully with ID {TreeNodeId}", resultDto.TreeNodeId);
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node");
            throw;
        }
    }

    public async Task<TreeNodeDto> UpdateTreeNodeAsync(int treeNodeId, UpdateTreeNodeDto updateDto)
    {
        using var scope = _logger.BeginScope("Updating tree node {TreeNodeId}", treeNodeId);

        try
        {
            _logger.LogInformation("Starting tree node update process");

            // Check if entity exists
            var existingEntity = await _treeNodeRepository.GetByIdAsync(treeNodeId);
            if (existingEntity == null)
            {
                _logger.LogWarning("Tree node not found for update");
                throw new KeyNotFoundException($"Tree node with ID {treeNodeId} not found");
            }

            // Business validation: Check if tree node type exists
            var nodeTypeExists = await _treeNodeTypeRepository.ExistsAsync(updateDto.TreeNodeTypeId);
            if (!nodeTypeExists)
            {
                _logger.LogWarning("Tree node type not found");
                throw new KeyNotFoundException($"Tree node type with ID {updateDto.TreeNodeTypeId} not found");
            }

            // Business validation: Check if new code conflicts with another node
            if (existingEntity.Code != updateDto.Code)
            {
                var conflictingNode = await _treeNodeRepository.GetByCodeAsync(updateDto.Code);
                if (conflictingNode != null && conflictingNode.TreeNodeId != treeNodeId)
                {
                    _logger.LogWarning("Tree node code conflicts with existing node");
                    throw new InvalidOperationException($"Tree node with code '{updateDto.Code}' already exists");
                }
            }

            // Update entity
            _mapper.Map(updateDto, existingEntity);
            existingEntity.ModifiedAt = DateTime.UtcNow;

            var updatedEntity = await _treeNodeRepository.UpdateAsync(existingEntity);
            var resultDto = _mapper.Map<TreeNodeDto>(updatedEntity);

            _logger.LogInformation("Tree node updated successfully");
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node");
            throw;
        }
    }

    public async Task<TreeNodeDto> MoveTreeNodeAsync(int treeNodeId, MoveTreeNodeDto moveDto)
    {
        using var scope = _logger.BeginScope("Moving tree node {TreeNodeId} to parent {NewParentId}", treeNodeId, moveDto.NewParentId);

        try
        {
            _logger.LogInformation("Starting tree node move process");

            // Check if entity exists
            var existingEntity = await _treeNodeRepository.GetByIdAsync(treeNodeId);
            if (existingEntity == null)
            {
                _logger.LogWarning("Tree node not found for move");
                throw new KeyNotFoundException($"Tree node with ID {treeNodeId} not found");
            }

            // Business validation: Check if new parent exists (if provided)
            if (moveDto.NewParentId.HasValue)
            {
                var newParent = await _treeNodeRepository.GetByIdAsync(moveDto.NewParentId.Value);
                if (newParent == null)
                {
                    _logger.LogWarning("New parent tree node not found");
                    throw new KeyNotFoundException($"New parent tree node with ID {moveDto.NewParentId.Value} not found");
                }

                // Business validation: Prevent moving node to its own descendant
                var descendants = await _treeNodeRepository.GetDescendantsAsync(treeNodeId);
                if (descendants.Any(d => d.TreeNodeId == moveDto.NewParentId.Value))
                {
                    _logger.LogWarning("Cannot move node to its own descendant");
                    throw new InvalidOperationException("Cannot move tree node to its own descendant");
                }
            }

            // Set order index if not provided
            if (moveDto.NewOrderIndex <= 0)
            {
                moveDto.NewOrderIndex = await _treeNodeRepository.GetMaxOrderIndexAsync(moveDto.NewParentId) + 1;
            }

            // Perform the move operation
            var result = await _treeNodeRepository.MoveNodeAsync(treeNodeId, moveDto.NewParentId, moveDto.NewOrderIndex);
            if (!result)
            {
                _logger.LogWarning("Tree node move operation failed");
                throw new InvalidOperationException("Failed to move tree node");
            }

            // Get updated entity
            var updatedEntity = await _treeNodeRepository.GetByIdAsync(treeNodeId);
            var resultDto = _mapper.Map<TreeNodeDto>(updatedEntity);

            _logger.LogInformation("Tree node moved successfully");
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tree node");
            throw;
        }
    }

    public async Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders)
    {
        using var scope = _logger.BeginScope("Reordering nodes for parent {ParentId}", parentId);

        try
        {
            _logger.LogInformation("Starting tree node reordering process for {Count} nodes", nodeOrders.Count);

            // Business validation: Check if parent exists (unless it's root level)
            if (parentId != 0)
            {
                var parentExists = await _treeNodeRepository.ExistsAsync(parentId);
                if (!parentExists)
                {
                    _logger.LogWarning("Parent tree node not found for reordering");
                    throw new KeyNotFoundException($"Parent tree node with ID {parentId} not found");
                }
            }

            // Perform the reorder operation
            var result = await _treeNodeRepository.ReorderNodesAsync(parentId, nodeOrders);

            _logger.LogInformation("Tree node reordering result: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering tree nodes");
            throw;
        }
    }

    public async Task<bool> DeleteTreeNodeAsync(int treeNodeId)
    {
        using var scope = _logger.BeginScope("Deleting tree node {TreeNodeId}", treeNodeId);

        try
        {
            _logger.LogInformation("Starting tree node deletion process");

            // Check if entity exists
            var existingEntity = await _treeNodeRepository.GetByIdAsync(treeNodeId);
            if (existingEntity == null)
            {
                _logger.LogWarning("Tree node not found for deletion");
                return false;
            }

            // Business validation: Check if node has children
            var hasChildren = await _treeNodeRepository.HasChildrenAsync(treeNodeId);
            if (hasChildren)
            {
                _logger.LogWarning("Cannot delete tree node with children");
                throw new InvalidOperationException($"Cannot delete tree node '{existingEntity.Name}' as it has child nodes");
            }

            // TODO: Check if node has associated questions in QuestionRepository before deletion

            // Delete entity (soft delete)
            var result = await _treeNodeRepository.DeleteAsync(treeNodeId);

            _logger.LogInformation("Tree node deletion result: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int treeNodeId)
    {
        try
        {
            var result = await _treeNodeRepository.ExistsAsync(treeNodeId);
            _logger.LogDebug("Tree node {TreeNodeId} exists: {Exists}", treeNodeId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node exists");
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(int treeNodeId)
    {
        try
        {
            var result = await _treeNodeRepository.HasChildrenAsync(treeNodeId);
            _logger.LogDebug("Tree node {TreeNodeId} has children: {HasChildren}", treeNodeId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node has children");
            throw;
        }
    }

    public async Task<TreeNodeStatistics> GetStatisticsAsync(int nodeId)
    {
        using var scope = _logger.BeginScope("Getting statistics for node {NodeId}", nodeId);

        try
        {
            _logger.LogInformation("Retrieving tree node statistics");

            var repoStatistics = await _treeNodeRepository.GetStatisticsAsync(nodeId);
            var statistics = _mapper.Map<TreeNodeStatistics>(repoStatistics);

            _logger.LogInformation("Tree node statistics retrieved successfully");
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node statistics");
            throw;
        }
    }

    public async Task<TreeStructureDto> GetCompleteTreeAsync()
    {
        using var scope = _logger.BeginScope("Getting complete tree structure");

        try
        {
            _logger.LogInformation("Retrieving complete tree structure");

            var rootNodes = await _treeNodeRepository.GetRootNodesAsync();
            var rootDtos = new List<TreeNodeDetailDto>();

            foreach (var root in rootNodes)
            {
                var rootDto = _mapper.Map<TreeNodeDetailDto>(root);
                await LoadChildrenRecursively(rootDto, root.TreeNodeId, int.MaxValue, 1);
                rootDtos.Add(rootDto);
            }

            var allNodes = await _treeNodeRepository.GetAllAsync();
            var maxDepth = allNodes.Any() ? allNodes.Max(n => n.Path.Count(c => c == '-')) - 1 : 0;

            var result = new TreeStructureDto
            {
                Roots = rootDtos,
                TotalNodesCount = allNodes.Count(),
                MaxDepth = maxDepth
            };

            _logger.LogInformation("Complete tree structure retrieved with {RootCount} roots and {TotalCount} total nodes",
                result.Roots.Count(), result.TotalNodesCount);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complete tree structure");
            throw;
        }
    }
}
