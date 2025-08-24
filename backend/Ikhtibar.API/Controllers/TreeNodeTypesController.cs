using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers;    /// <summary>
                                       /// Controller for TreeNodeType management operations
                                       /// Following SRP: ONLY HTTP concerns for tree node types
                                       /// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TreeNodeTypesController : ControllerBase
{
    private readonly ITreeNodeTypeService _treeNodeTypeService;
    private readonly ITreeNodeService _treeNodeService;
    private readonly ILogger<TreeNodeTypesController> _logger;

    public TreeNodeTypesController(
        ITreeNodeTypeService treeNodeTypeService,
        ITreeNodeService treeNodeService,
        ILogger<TreeNodeTypesController> logger)
    {
        _treeNodeTypeService = treeNodeTypeService ?? throw new ArgumentNullException(nameof(treeNodeTypeService));
        _treeNodeService = treeNodeService ?? throw new ArgumentNullException(nameof(treeNodeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all tree node types
    /// </summary>
    /// <returns>List of tree node types</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeTypeDto>>> GetAllTreeNodeTypes()
    {
        try
        {
            var treeNodeTypes = await _treeNodeTypeService.GetAllAsync();
            return Ok(treeNodeTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree node types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tree node types");
        }
    }

    /// <summary>
    /// Get tree node type by ID
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <returns>Tree node type details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TreeNodeTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeTypeDto>> GetTreeNodeType(int id)
    {
        try
        {
            var treeNodeType = await _treeNodeTypeService.GetTreeNodeTypeAsync(id);
            if (treeNodeType == null)
            {
                return NotFound($"Tree node type with ID {id} not found");
            }

            return Ok(treeNodeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the tree node type");
        }
    }

    /// <summary>
    /// Get tree node type by name
    /// </summary>
    /// <param name="name">Tree node type name</param>
    /// <returns>Tree node type details</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(typeof(TreeNodeTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeTypeDto>> GetTreeNodeTypeByName(string name)
    {
        try
        {
            var treeNodeType = await _treeNodeTypeService.GetTreeNodeTypeByNameAsync(name);
            if (treeNodeType == null)
            {
                return NotFound($"Tree node type with name '{name}' not found");
            }

            return Ok(treeNodeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by name {Name}", name);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the tree node type");
        }
    }

    /// <summary>
    /// Create a new tree node type
    /// </summary>
    /// <param name="createDto">Tree node type creation data</param>
    /// <returns>Created tree node type</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TreeNodeTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeTypeDto>> CreateTreeNodeType([FromBody] CreateTreeNodeTypeDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var treeNodeType = await _treeNodeTypeService.CreateTreeNodeTypeAsync(createDto);
            return CreatedAtAction(
                nameof(GetTreeNodeType),
                new { id = treeNodeType.Id },
                treeNodeType);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while creating tree node type");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node type");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the tree node type");
        }
    }

    /// <summary>
    /// Update an existing tree node type
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <param name="updateDto">Tree node type update data</param>
    /// <returns>Updated tree node type</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TreeNodeTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreeNodeTypeDto>> UpdateTreeNodeType(int id, [FromBody] UpdateTreeNodeTypeDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var treeNodeType = await _treeNodeTypeService.UpdateTreeNodeTypeAsync(id, updateDto);
            return Ok(treeNodeType);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Tree node type not found for update");
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while updating tree node type");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the tree node type");
        }
    }

    /// <summary>
    /// Delete a tree node type
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTreeNodeType(int id)
    {
        try
        {
            var result = await _treeNodeTypeService.DeleteTreeNodeTypeAsync(id);
            if (!result)
            {
                return NotFound($"Tree node type with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict while deleting tree node type");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the tree node type");
        }
    }

    /// <summary>
    /// Check if tree node type exists
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TreeNodeTypeExists(int id)
    {
        try
        {
            var exists = await _treeNodeTypeService.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type exists {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking tree node type existence");
        }
    }

    /// <summary>
    /// Get tree nodes by type
    /// </summary>
    /// <param name="id">Tree node type ID</param>
    /// <returns>List of tree nodes of specified type</returns>
    [HttpGet("{id:int}/nodes")]
    [ProducesResponseType(typeof(IEnumerable<TreeNodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetTreeNodesByType(int id)
    {
        try
        {
            // First check if the type exists
            var typeExists = await _treeNodeTypeService.ExistsAsync(id);
            if (!typeExists)
            {
                return NotFound($"Tree node type with ID {id} not found");
            }

            var nodes = await _treeNodeService.GetNodesByTypeAsync(id);
            return Ok(nodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree nodes by type {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tree nodes");
        }
    }
}
