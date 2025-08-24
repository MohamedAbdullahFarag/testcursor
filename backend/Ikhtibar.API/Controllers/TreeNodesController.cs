using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for TreeNode management operations
/// Following SRP: ONLY HTTP concerns for tree nodes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TreeNodesController : ControllerBase
{
    private readonly ITreeNodeService _treeNodeService;
    private readonly ILogger<TreeNodesController> _logger;

    public TreeNodesController(
        ITreeNodeService treeNodeService,
        ILogger<TreeNodesController> logger)
    {
        _treeNodeService = treeNodeService ?? throw new ArgumentNullException(nameof(treeNodeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all tree nodes
    /// </summary>
    /// <returns>List of tree nodes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetAllTreeNodes()
    {
        try
        {
            var treeNodes = await _treeNodeService.GetAllAsync();
            return Ok(treeNodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree nodes");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tree nodes");
        }
    }

    /// <summary>
    /// Get root tree nodes
    /// </summary>
    /// <returns>List of root tree nodes</returns>
    [HttpGet("roots")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetRootNodes()
    {
        try
        {
            var rootNodes = await _treeNodeService.GetRootNodesAsync();
            return Ok(rootNodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root tree nodes");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving root tree nodes");
        }
    }

    /// <summary>
    /// Get tree node by ID
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>Tree node details with children</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TreeNodeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDetailDto>> GetTreeNode(int id)
    {
        try
        {
            var treeNode = await _treeNodeService.GetTreeNodeAsync(id);
            if (treeNode == null)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            return Ok(treeNode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the tree node");
        }
    }

    /// <summary>
    /// Get tree node by code
    /// </summary>
    /// <param name="code">Tree node code</param>
    /// <returns>Tree node details</returns>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(TreeNodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDto>> GetTreeNodeByCode(string code)
    {
        try
        {
            var treeNode = await _treeNodeService.GetTreeNodeByCodeAsync(code);
            if (treeNode == null)
            {
                return NotFound($"Tree node with code '{code}' not found");
            }

            return Ok(treeNode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node by code {Code}", code);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the tree node");
        }
    }

    /// <summary>
    /// Get children of a tree node
    /// </summary>
    /// <param name="id">Parent tree node ID</param>
    /// <returns>List of child tree nodes</returns>
    [HttpGet("{id:int}/children")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetChildren(int id)
    {
        try
        {
            // Check if parent exists
            var parentExists = await _treeNodeService.ExistsAsync(id);
            if (!parentExists)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            var children = await _treeNodeService.GetChildrenAsync(id);
            return Ok(children);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving children for tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving child tree nodes");
        }
    }

    /// <summary>
    /// Get ancestors of a tree node
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>List of ancestor tree nodes</returns>
    [HttpGet("{id:int}/ancestors")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetAncestors(int id)
    {
        try
        {
            // Check if node exists
            var nodeExists = await _treeNodeService.ExistsAsync(id);
            if (!nodeExists)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            var ancestors = await _treeNodeService.GetAncestorsAsync(id);
            return Ok(ancestors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestors for tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving ancestor tree nodes");
        }
    }

    /// <summary>
    /// Get descendants of a tree node
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>List of descendant tree nodes</returns>
    [HttpGet("{id:int}/descendants")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetDescendants(int id)
    {
        try
        {
            // Check if node exists
            var nodeExists = await _treeNodeService.ExistsAsync(id);
            if (!nodeExists)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            var descendants = await _treeNodeService.GetDescendantsAsync(id);
            return Ok(descendants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendants for tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving descendant tree nodes");
        }
    }

    /// <summary>
    /// Get tree structure starting from a root node
    /// </summary>
    /// <param name="id">Root tree node ID</param>
    /// <param name="levels">Number of levels to load (default: 1)</param>
    /// <returns>Tree structure with nested children</returns>
    [HttpGet("{id:int}/structure")]
    [ProducesResponseType(typeof(TreeNodeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDetailDto>> GetTreeStructure(int id, [FromQuery] int levels = 1)
    {
        if (levels < 0 || levels > 10) // Prevent excessive recursion
        {
            return BadRequest("Levels must be between 0 and 10");
        }

        try
        {
            var treeStructure = await _treeNodeService.GetTreeStructureAsync(id, levels);
            if (treeStructure == null)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            return Ok(treeStructure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree structure for node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the tree structure");
        }
    }

    /// <summary>
    /// Get complete tree structure
    /// </summary>
    /// <returns>Complete tree with all nodes</returns>
    [HttpGet("structure/complete")]
    [ProducesResponseType(typeof(TreeStructureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeStructureDto>> GetCompleteTree()
    {
        try
        {
            var completeTree = await _treeNodeService.GetCompleteTreeAsync();
            return Ok(completeTree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complete tree structure");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the complete tree structure");
        }
    }

    /// <summary>
    /// Get tree nodes by type
    /// </summary>
    /// <param name="typeId">Tree node type ID</param>
    /// <returns>List of tree nodes of specified type</returns>
    [HttpGet("by-type/{typeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetNodesByType(int typeId)
    {
        try
        {
            var nodes = await _treeNodeService.GetNodesByTypeAsync(typeId);
            return Ok(nodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree nodes by type {TypeId}", typeId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tree nodes by type");
        }
    }

    /// <summary>
    /// Get tree node statistics
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>Tree node statistics</returns>
    [HttpGet("{id:int}/statistics")]
    [ProducesResponseType(typeof(TreeNodeStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeStatistics>> GetStatistics(int id)
    {
        try
        {
            // Check if node exists
            var nodeExists = await _treeNodeService.ExistsAsync(id);
            if (!nodeExists)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            var statistics = await _treeNodeService.GetStatisticsAsync(id);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tree node statistics");
        }
    }

    /// <summary>
    /// Create a new tree node
    /// </summary>
    /// <param name="createDto">Tree node creation data</param>
    /// <returns>Created tree node</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TreeNodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDto>> CreateTreeNode([FromBody] CreateTreeNodeDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var treeNode = await _treeNodeService.CreateTreeNodeAsync(createDto);
            return CreatedAtAction(
                nameof(GetTreeNode),
                new { id = treeNode.Id },
                treeNode);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Referenced entity not found while creating tree node");
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while creating tree node");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the tree node");
        }
    }

    /// <summary>
    /// Update an existing tree node
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <param name="updateDto">Tree node update data</param>
    /// <returns>Updated tree node</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TreeNodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDto>> UpdateTreeNode(int id, [FromBody] UpdateTreeNodeDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var treeNode = await _treeNodeService.UpdateTreeNodeAsync(id, updateDto);
            return Ok(treeNode);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Tree node not found for update");
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while updating tree node");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the tree node");
        }
    }

    /// <summary>
    /// Move a tree node to a new parent
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <param name="moveDto">Move operation data</param>
    /// <returns>Updated tree node</returns>
    [HttpPatch("{id:int}/move")]
    [ProducesResponseType(typeof(TreeNodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeDto>> MoveTreeNode(int id, [FromBody] MoveTreeNodeDto moveDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var treeNode = await _treeNodeService.MoveTreeNodeAsync(id, moveDto);
            return Ok(treeNode);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Tree node not found for move");
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid move operation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while moving the tree node");
        }
    }

    /// <summary>
    /// Reorder nodes within a parent
    /// </summary>
    /// <param name="parentId">Parent tree node ID (0 for root level)</param>
    /// <param name="reorderDto">Reorder operation data</param>
    /// <returns>Success indicator</returns>
    [HttpPatch("reorder/{parentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReorderNodes(int parentId, [FromBody] ReorderTreeNodesDto reorderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _treeNodeService.ReorderNodesAsync(parentId, reorderDto.NodeOrders);
            if (!result)
            {
                return BadRequest("Failed to reorder nodes");
            }

            return Ok(new { Success = true, Message = "Nodes reordered successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent not found for reorder operation");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering tree nodes for parent {ParentId}", parentId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while reordering tree nodes");
        }
    }

    /// <summary>
    /// Delete a tree node
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTreeNode(int id)
    {
        try
        {
            var result = await _treeNodeService.DeleteTreeNodeAsync(id);
            if (!result)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while deleting tree node");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the tree node");
        }
    }

    /// <summary>
    /// Check if tree node exists
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TreeNodeExists(int id)
    {
        try
        {
            var exists = await _treeNodeService.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node exists {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking tree node existence");
        }
    }

    /// <summary>
    /// Check if tree node has children
    /// </summary>
    /// <param name="id">Tree node ID</param>
    /// <returns>Boolean indicating if node has children</returns>
    [HttpGet("{id:int}/has-children")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> HasChildren(int id)
    {
        try
        {
            // Check if node exists
            var nodeExists = await _treeNodeService.ExistsAsync(id);
            if (!nodeExists)
            {
                return NotFound($"Tree node with ID {id} not found");
            }

            var hasChildren = await _treeNodeService.HasChildrenAsync(id);
            return Ok(hasChildren);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node has children {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking tree node children");
        }
    }
}
