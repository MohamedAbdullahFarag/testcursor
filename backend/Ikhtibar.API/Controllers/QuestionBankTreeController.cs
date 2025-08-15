using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for question bank tree management operations
/// Provides comprehensive tree structure management, category operations, and hierarchy management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionBankTreeController : ControllerBase
{
    private readonly IQuestionBankTreeService _treeService;
    private readonly ILogger<QuestionBankTreeController> _logger;

    public QuestionBankTreeController(
        IQuestionBankTreeService treeService,
        ILogger<QuestionBankTreeController> logger)
    {
        _treeService = treeService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete question bank tree structure
    /// </summary>
    /// <param name="rootCategoryId">Optional root category ID to limit tree scope</param>
    /// <param name="maxDepth">Maximum depth to retrieve (default: 10)</param>
    /// <returns>Complete tree structure with categories and hierarchy</returns>
    [HttpGet("tree")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankTreeDto>> GetTreeAsync(
        [FromQuery] int? rootCategoryId = null,
        [FromQuery] int maxDepth = 10)
    {
        try
        {
            _logger.LogInformation("Retrieving question bank tree structure. Root: {RootId}, MaxDepth: {MaxDepth}", 
                rootCategoryId, maxDepth);

            var tree = await _treeService.GetTreeAsync(rootCategoryId, maxDepth);
            return Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question bank tree structure");
            return StatusCode(500, "Failed to retrieve tree structure");
        }
    }

    /// <summary>
    /// Get child categories for a specific parent
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <returns>List of child categories</returns>
    [HttpGet("categories/{parentId}/children")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<IEnumerable<QuestionBankCategoryDto>>> GetChildCategoriesAsync(int parentId)
    {
        try
        {
            var children = await _treeService.GetChildCategoriesAsync(parentId);
            return Ok(children);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child categories for parent {ParentId}", parentId);
            return StatusCode(500, "Failed to retrieve child categories");
        }
    }

    /// <summary>
    /// Get parent category for a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Parent category information</returns>
    [HttpGet("categories/{categoryId}/parent")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankCategoryDto>> GetParentCategoryAsync(int categoryId)
    {
        try
        {
            var parent = await _treeService.GetParentCategoryAsync(categoryId);
            if (parent == null)
                return NotFound($"Parent category not found for category {categoryId}");

            return Ok(parent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parent category for category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to retrieve parent category");
        }
    }

    /// <summary>
    /// Get ancestor categories for a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of ancestor categories from root to parent</returns>
    [HttpGet("categories/{categoryId}/ancestors")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<IEnumerable<QuestionBankCategoryDto>>> GetAncestorsAsync(int categoryId)
    {
        try
        {
            var ancestors = await _treeService.GetAncestorsAsync(categoryId);
            return Ok(ancestors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestors for category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to retrieve ancestor categories");
        }
    }

    /// <summary>
    /// Get descendant categories for a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="maxDepth">Maximum depth to retrieve (default: 10)</param>
    /// <returns>List of descendant categories</returns>
    [HttpGet("categories/{categoryId}/descendants")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<IEnumerable<QuestionBankCategoryDto>>> GetDescendantsAsync(
        int categoryId, 
        [FromQuery] int maxDepth = 10)
    {
        try
        {
            var descendants = await _treeService.GetDescendantsAsync(categoryId, maxDepth);
            return Ok(descendants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendants for category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to retrieve descendant categories");
        }
    }

    /// <summary>
    /// Get sibling categories for a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of sibling categories</returns>
    [HttpGet("categories/{categoryId}/siblings")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<IEnumerable<QuestionBankCategoryDto>>> GetSiblingsAsync(int categoryId)
    {
        try
        {
            var siblings = await _treeService.GetSiblingsAsync(categoryId);
            return Ok(siblings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving siblings for category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to retrieve sibling categories");
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Category information</returns>
    [HttpGet("categories/{categoryId}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankCategoryDto>> GetCategoryByIdAsync(int categoryId)
    {
        try
        {
            var category = await _treeService.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return NotFound($"Category with ID {categoryId} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to retrieve category");
        }
    }

    /// <summary>
    /// Search categories by search term
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="maxResults">Maximum number of results (default: 50)</param>
    /// <returns>List of matching categories</returns>
    [HttpGet("categories/search")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<IEnumerable<QuestionBankCategoryDto>>> SearchCategoriesAsync(
        [FromQuery] string searchTerm,
        [FromQuery] int maxResults = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var categories = await _treeService.SearchCategoriesAsync(searchTerm, maxResults);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories with term '{SearchTerm}'", searchTerm);
            return StatusCode(500, "Failed to search categories");
        }
    }

    /// <summary>
    /// Find category by tree path
    /// </summary>
    /// <param name="treePath">Tree path to search for</param>
    /// <returns>Category information if found</returns>
    [HttpGet("categories/find-by-path")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankCategoryDto>> FindCategoryByPathAsync(
        [FromQuery] string treePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(treePath))
                return BadRequest("Tree path is required");

            var category = await _treeService.FindCategoryByPathAsync(treePath);
            if (category == null)
                return NotFound($"Category with path '{treePath}' not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding category by path '{TreePath}'", treePath);
            return StatusCode(500, "Failed to find category by path");
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="dto">Category creation data</param>
    /// <returns>Created category information</returns>
    [HttpPost("categories")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankCategoryDto>> CreateCategoryAsync(
        [FromBody] CreateQuestionBankCategoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _treeService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { categoryId = category.CategoryId }, category);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating category: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {Name}", dto.Name);
            return StatusCode(500, "Failed to create category");
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="categoryId">Category ID to update</param>
    /// <param name="dto">Category update data</param>
    /// <returns>Updated category information</returns>
    [HttpPut("categories/{categoryId}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<QuestionBankCategoryDto>> UpdateCategoryAsync(
        int categoryId,
        [FromBody] UpdateQuestionBankCategoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _treeService.UpdateCategoryAsync(categoryId, dto);
            return Ok(category);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating category {CategoryId}: {Message}", categoryId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to update category");
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="categoryId">Category ID to delete</param>
    /// <param name="cascade">Whether to cascade delete child categories</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("categories/{categoryId}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<bool>> DeleteCategoryAsync(
        int categoryId,
        [FromQuery] bool cascade = false)
    {
        try
        {
            var result = await _treeService.DeleteCategoryAsync(categoryId, cascade);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error deleting category {CategoryId}: {Message}", categoryId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to delete category");
        }
    }

    /// <summary>
    /// Move a category to a new parent
    /// </summary>
    /// <param name="categoryId">Category ID to move</param>
    /// <param name="dto">Move operation data</param>
    /// <returns>Move operation result</returns>
    [HttpPut("categories/{categoryId}/move")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<bool>> MoveCategoryAsync(
        int categoryId,
        [FromBody] Ikhtibar.Shared.DTOs.MoveCategoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _treeService.MoveCategoryAsync(categoryId, dto.NewParentId, dto.NewOrder);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error moving category {CategoryId}: {Message}", categoryId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving category {CategoryId}", categoryId);
            return StatusCode(500, "Failed to move category");
        }
    }



    /// <summary>
    /// Reorder categories within a parent
    /// </summary>
    /// <param name="dto">Reorder operation data</param>
    /// <returns>Reorder operation result</returns>
    [HttpPut("categories/reorder")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<bool>> ReorderCategoriesAsync(
        [FromBody] Ikhtibar.Shared.DTOs.ReorderCategoriesDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryOrders = dto.CategoryIds.Select((id, index) => new Ikhtibar.Core.Services.Interfaces.CategoryOrderDto
            {
                CategoryId = id,
                Position = index + 1
            });

            var result = await _treeService.ReorderCategoriesAsync(dto.ParentId ?? 0, categoryOrders);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error reordering categories: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering categories");
            return StatusCode(500, "Failed to reorder categories");
        }
    }

    /// <summary>
    /// Get tree statistics
    /// </summary>
    /// <returns>Tree statistics information</returns>
    [HttpGet("statistics")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<Ikhtibar.Shared.DTOs.TreeStatisticsDto>> GetTreeStatisticsAsync()
    {
        try
        {
            var statistics = await _treeService.GetTreeStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree statistics");
            return StatusCode(500, "Failed to retrieve tree statistics");
        }
    }

    /// <summary>
    /// Validate tree structure integrity
    /// </summary>
    /// <returns>Tree validation results</returns>
    [HttpGet("validate/structure")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<Ikhtibar.Shared.DTOs.TreeValidationResultDto>> ValidateTreeStructureAsync()
    {
        try
        {
            var result = await _treeService.ValidateTreeIntegrityAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating tree structure");
            return StatusCode(500, "Failed to validate tree structure");
        }
    }



    /// <summary>
    /// Bulk create categories
    /// </summary>
    /// <param name="categories">List of categories to create</param>
    /// <returns>Bulk creation result</returns>
    [HttpPost("categories/bulk")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<bool>> BulkCreateCategoriesAsync(
        [FromBody] IEnumerable<CreateQuestionBankCategoryDto> categories)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _treeService.BulkCreateCategoriesAsync(categories);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error in bulk category creation: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk category creation");
            return StatusCode(500, "Failed to bulk create categories");
        }
    }


}
