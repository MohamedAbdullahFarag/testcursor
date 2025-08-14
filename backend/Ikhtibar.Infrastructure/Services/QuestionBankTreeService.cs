using AutoMapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Microsoft.Extensions.Logging;

// Use aliases to resolve ambiguous references
using CoreCategoryOrderDto = Ikhtibar.Core.Services.Interfaces.CategoryOrderDto;
using CoreTreeValidationResultDto = Ikhtibar.Core.Services.Interfaces.TreeValidationResultDto;
using CoreTreeStatisticsDto = Ikhtibar.Core.Services.Interfaces.TreeStatisticsDto;
using CoreMoveCategoryDto = Ikhtibar.Core.Services.Interfaces.MoveCategoryDto;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for question bank tree operations
/// Provides business logic for tree navigation, manipulation, and maintenance
/// </summary>
public class QuestionBankTreeService : IQuestionBankTreeService
{
    private readonly IQuestionBankTreeRepository _treeRepository;
    private readonly IQuestionBankCategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<QuestionBankTreeService> _logger;

    public QuestionBankTreeService(
        IQuestionBankTreeRepository treeRepository,
        IQuestionBankCategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<QuestionBankTreeService> logger)
    {
        _treeRepository = treeRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<QuestionBankTreeDto> GetTreeAsync(int? rootCategoryId = null, int maxDepth = 10)
    {
        using var scope = _logger.BeginScope("Getting tree structure for root {RootId} with max depth {MaxDepth}", 
            rootCategoryId, maxDepth);

        try
        {
            var treeNode = await _treeRepository.GetTreeAsync(rootCategoryId, maxDepth);
            var categories = new List<QuestionBankCategoryDto>();
            
            if (treeNode != null)
            {
                // Flatten the tree structure into a list for the DTO
                categories = FlattenTreeNode(treeNode).ToList();
            }

            var lastModified = categories.Any() 
                ? categories.Max(c => c.ModifiedAt ?? c.CreatedAt)
                : DateTime.UtcNow;

            return new QuestionBankTreeDto
            {
                Categories = categories,
                TotalCategories = categories.Count,
                MaxDepth = categories.Any() ? categories.Max(c => c.TreeLevel) : 0,
                LastModified = lastModified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tree structure");
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategoryDto>> GetChildCategoriesAsync(int parentId)
    {
        using var scope = _logger.BeginScope("Getting child categories for parent {ParentId}", parentId);

        try
        {
            var subtree = await _treeRepository.GetSubtreeAsync(parentId, 1);
            var children = new List<QuestionBankCategoryDto>();
            
            if (subtree?.Children != null)
            {
                children = subtree.Children.Select(child => _mapper.Map<QuestionBankCategoryDto>(child.Category)).ToList();
            }
            
            return children;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get child categories for parent {ParentId}", parentId);
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto?> GetParentCategoryAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting parent category for {CategoryId}", categoryId);

        try
        {
            var breadcrumbs = await _treeRepository.GetBreadcrumbsAsync(categoryId);
            var parentBreadcrumb = breadcrumbs.Skip(breadcrumbs.Count() - 2).FirstOrDefault();
            
            if (parentBreadcrumb != null)
            {
                var parent = await _categoryRepository.GetByIdAsync(parentBreadcrumb.CategoryId);
                return parent != null ? _mapper.Map<QuestionBankCategoryDto>(parent) : null;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get parent category for {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategoryDto>> GetAncestorsAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting ancestors for category {CategoryId}", categoryId);

        try
        {
            var breadcrumbs = await _treeRepository.GetBreadcrumbsAsync(categoryId);
            var ancestors = new List<QuestionBankCategoryDto>();
            
            foreach (var breadcrumb in breadcrumbs.Take(breadcrumbs.Count() - 1))
            {
                var category = await _categoryRepository.GetByIdAsync(breadcrumb.CategoryId);
                if (category != null)
                {
                    ancestors.Add(_mapper.Map<QuestionBankCategoryDto>(category));
                }
            }
            
            return ancestors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ancestors for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategoryDto>> GetDescendantsAsync(int categoryId, int maxDepth = 10)
    {
        using var scope = _logger.BeginScope("Getting descendants for category {CategoryId} with max depth {MaxDepth}", 
            categoryId, maxDepth);

        try
        {
            var subtree = await _treeRepository.GetSubtreeAsync(categoryId, maxDepth);
            var descendants = new List<QuestionBankCategoryDto>();
            
            if (subtree != null)
            {
                descendants = FlattenTreeNode(subtree)
                    .Where(c => c.CategoryId != categoryId) // Exclude the root category itself
                    .ToList();
            }
            
            return descendants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get descendants for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategoryDto>> GetSiblingsAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting siblings for category {CategoryId}", categoryId);

        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return Enumerable.Empty<QuestionBankCategoryDto>();
            }

            // Get parent and then its children (siblings)
            var parentId = category.ParentId;
            var siblings = new List<QuestionBankCategoryDto>();
            
            if (parentId.HasValue)
            {
                var parentSubtree = await _treeRepository.GetSubtreeAsync(parentId.Value, 1);
                if (parentSubtree?.Children != null)
                {
                    siblings = parentSubtree.Children
                        .Where(child => child.Category.CategoryId != categoryId)
                        .Select(child => _mapper.Map<QuestionBankCategoryDto>(child.Category))
                        .ToList();
                }
            }
            
            return siblings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get siblings for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto> CreateCategoryAsync(CreateQuestionBankCategoryDto dto)
    {
        using var scope = _logger.BeginScope("Creating category with name {Name}", dto.Name);

        try
        {
            // Validate the category data
            await ValidateCreateCategoryAsync(dto);

            // Use the category repository for creation
            var category = _mapper.Map<QuestionBankCategory>(dto);
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = dto.IsActive;

            // Create the category using the category repository
            var createdCategory = await _categoryRepository.CreateAsync(category);
            
            _logger.LogInformation("Category {CategoryId} created successfully with name {Name}", 
                createdCategory.CategoryId, createdCategory.Name);

            return _mapper.Map<QuestionBankCategoryDto>(createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create category with name {Name}", dto.Name);
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto> UpdateCategoryAsync(int categoryId, UpdateQuestionBankCategoryDto dto)
    {
        using var scope = _logger.BeginScope("Updating category {CategoryId}", categoryId);

        try
        {
            // Get existing category
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                throw new ArgumentException($"Category with ID {categoryId} not found", nameof(categoryId));
            }

            // Validate the update
            await ValidateUpdateCategoryAsync(categoryId, dto);

            // Map updates
            _mapper.Map(dto, existingCategory);
            existingCategory.ModifiedAt = DateTime.UtcNow;

            // Update the category
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            
            _logger.LogInformation("Category {CategoryId} updated successfully", categoryId);
            return _mapper.Map<QuestionBankCategoryDto>(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId, bool cascade = false)
    {
        using var scope = _logger.BeginScope("Deleting category {CategoryId} with cascade={Cascade}", categoryId, cascade);

        try
        {
            // Validate deletion
            await ValidateDeleteCategoryAsync(categoryId, cascade);

            var strategy = cascade ? ChildHandlingStrategy.Delete : ChildHandlingStrategy.Prevent;
            var result = await _treeRepository.DeleteCategoryAsync(categoryId, strategy);
            
            _logger.LogInformation("Category {CategoryId} deleted successfully with cascade={Cascade}", categoryId, cascade);
            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> MoveCategoryAsync(int categoryId, int? newParentId, int? insertAtPosition = null)
    {
        using var scope = _logger.BeginScope("Moving category {CategoryId} to parent {NewParentId} at position {Position}", 
            categoryId, newParentId, insertAtPosition);

        try
        {
            // Validate the move
            await ValidateMoveCategoryAsync(categoryId, newParentId);

            var result = await _treeRepository.MoveCategoryAsync(categoryId, newParentId, insertAtPosition);
            
            _logger.LogInformation("Category {CategoryId} moved successfully to parent {NewParentId}", categoryId, newParentId);
            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to move category {CategoryId} to parent {NewParentId}", categoryId, newParentId);
            throw;
        }
    }

    public async Task<bool> ReorderCategoriesAsync(int parentId, IEnumerable<CoreCategoryOrderDto> categoryOrders)
    {
        using var scope = _logger.BeginScope("Reordering categories under parent {ParentId}", parentId);

        try
        {
            // For now, implement by optimizing sort orders for the parent
            // A more sophisticated implementation would handle individual positioning
            var result = await _treeRepository.OptimizeSortOrdersAsync(parentId);
            
            _logger.LogInformation("Categories reordered successfully under parent {ParentId}", parentId);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reorder categories under parent {ParentId}", parentId);
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto?> GetCategoryByIdAsync(int categoryId)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            return category != null ? _mapper.Map<QuestionBankCategoryDto>(category) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategoryDto>> SearchCategoriesAsync(string searchTerm, int maxResults = 50)
    {
        using var scope = _logger.BeginScope("Searching categories with term {SearchTerm}", searchTerm);

        try
        {
            var searchCriteria = new TreeSearchCriteria
            {
                SearchTerm = searchTerm,
                MaxResults = maxResults,
                SearchInDescriptions = true,
                SearchInCodes = true
            };
            
            var searchResults = await _treeRepository.SearchTreeAsync(searchCriteria);
            var categories = searchResults.Select(r => _mapper.Map<QuestionBankCategoryDto>(r.Category));
            
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search categories with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto?> FindCategoryByPathAsync(string treePath)
    {
        try
        {
            // This would need a more sophisticated implementation
            // For now, search by exact path match
            var searchCriteria = new TreeSearchCriteria
            {
                SearchTerm = treePath,
                MaxResults = 1
            };
            
            var searchResults = await _treeRepository.SearchTreeAsync(searchCriteria);
            var result = searchResults.FirstOrDefault();
            
            return result != null ? _mapper.Map<QuestionBankCategoryDto>(result.Category) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to find category by path {TreePath}", treePath);
            throw;
        }
    }

    public async Task<int> GetCategoryDepthAsync(int categoryId)
    {
        try
        {
            return await _treeRepository.GetCategoryDepthAsync(categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get depth for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> GetSubtreeQuestionCountAsync(int categoryId)
    {
        try
        {
            var subtree = await _treeRepository.GetSubtreeAsync(categoryId);
            return subtree?.QuestionCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get question count for subtree {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<CoreTreeValidationResultDto> ValidateTreeIntegrityAsync()
    {
        using var scope = _logger.BeginScope("Validating tree integrity");

        try
        {
            var validationResult = await _treeRepository.ValidateTreeIntegrityAsync();
            var errors = new List<string>();
            var warnings = new List<string>();

            if (!validationResult.IsValid)
            {
                errors.AddRange(validationResult.Issues.Where(i => i.Severity == "Error").Select(i => i.Description));
                warnings.AddRange(validationResult.Issues.Where(i => i.Severity == "Warning").Select(i => i.Description));
            }

            return new CoreTreeValidationResultDto
            {
                IsValid = validationResult.IsValid,
                Errors = errors,
                Warnings = warnings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate tree integrity");
            throw;
        }
    }

    public async Task<bool> RebuildTreePathsAsync()
    {
        using var scope = _logger.BeginScope("Rebuilding tree paths");

        try
        {
            var result = await _treeRepository.RebuildTreePathsAsync();
            _logger.LogInformation("Tree paths rebuilt successfully");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rebuild tree paths");
            throw;
        }
    }

    public async Task<bool> RecalculateQuestionCountsAsync()
    {
        using var scope = _logger.BeginScope("Recalculating question counts");

        try
        {
            // Use the tree maintenance operation
            var result = await _treeRepository.RebuildHierarchyTableAsync();
            _logger.LogInformation("Question counts recalculated successfully");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to recalculate question counts");
            throw;
        }
    }

    public async Task<CoreTreeStatisticsDto> GetTreeStatisticsAsync()
    {
        using var scope = _logger.BeginScope("Getting tree statistics");

        try
        {
            var stats = await _treeRepository.GetTreeStatisticsAsync();
            return _mapper.Map<CoreTreeStatisticsDto>(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tree statistics");
            throw;
        }
    }

    public async Task<bool> BulkCreateCategoriesAsync(IEnumerable<CreateQuestionBankCategoryDto> categories)
    {
        using var scope = _logger.BeginScope("Bulk creating {Count} categories", categories.Count());

        try
        {
            var result = true;
            foreach (var categoryDto in categories)
            {
                try
                {
                    await CreateCategoryAsync(categoryDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create category {Name} during bulk operation", categoryDto.Name);
                    result = false;
                }
            }
            
            _logger.LogInformation("Bulk created categories with success: {Success}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk create categories");
            throw;
        }
    }

    public async Task<bool> BulkMoveCategoriesAsync(IEnumerable<CoreMoveCategoryDto> moves)
    {
        using var scope = _logger.BeginScope("Bulk moving {Count} categories", moves.Count());

        try
        {
            var result = true;
            foreach (var move in moves)
            {
                result &= await MoveCategoryAsync(move.CategoryId, move.NewParentId, move.Position);
            }
            
            _logger.LogInformation("Bulk moved {Count} categories successfully", moves.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk move categories");
            throw;
        }
    }

    public async Task<bool> BulkDeleteCategoriesAsync(IEnumerable<int> categoryIds, bool cascade = false)
    {
        using var scope = _logger.BeginScope("Bulk deleting {Count} categories with cascade={Cascade}", 
            categoryIds.Count(), cascade);

        try
        {
            var result = true;
            foreach (var categoryId in categoryIds)
            {
                result &= await DeleteCategoryAsync(categoryId, cascade);
            }
            
            _logger.LogInformation("Bulk deleted {Count} categories successfully", categoryIds.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk delete categories");
            throw;
        }
    }

    public async Task<string> ExportTreeToJsonAsync(int? rootCategoryId = null)
    {
        using var scope = _logger.BeginScope("Exporting tree to JSON from root {RootId}", rootCategoryId);

        try
        {
            var treeExport = await _treeRepository.ExportTreeAsync(rootCategoryId, true);
            // Convert to JSON string - would need a JSON serializer
            var jsonResult = System.Text.Json.JsonSerializer.Serialize(treeExport);
            
            _logger.LogInformation("Tree exported to JSON successfully");
            return jsonResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export tree to JSON");
            throw;
        }
    }

    public async Task<bool> ImportTreeFromJsonAsync(string jsonData, int? parentCategoryId = null)
    {
        using var scope = _logger.BeginScope("Importing tree from JSON under parent {ParentId}", parentCategoryId);

        try
        {
            // Deserialize JSON to tree import structure
            var treeData = System.Text.Json.JsonSerializer.Deserialize<CategoryTreeImport>(jsonData);
            if (treeData == null)
            {
                throw new ArgumentException("Invalid JSON data", nameof(jsonData));
            }
            
            var result = await _treeRepository.ImportTreeAsync(treeData, parentCategoryId);
            
            _logger.LogInformation("Tree imported from JSON successfully");
            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import tree from JSON");
            throw;
        }
    }

    #region Private Helper Methods

    private IEnumerable<QuestionBankCategoryDto> FlattenTreeNode(CategoryTreeNode node)
    {
        var categories = new List<QuestionBankCategoryDto>();
        
        // Add current node
        var dto = _mapper.Map<QuestionBankCategoryDto>(node.Category);
        dto.QuestionCount = node.QuestionCount;
        dto.TreeLevel = node.Depth;
        categories.Add(dto);
        
        // Add children recursively
        foreach (var child in node.Children)
        {
            categories.AddRange(FlattenTreeNode(child));
        }
        
        return categories;
    }

    #endregion

    #region Private Validation Methods

    private async Task ValidateCreateCategoryAsync(CreateQuestionBankCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Category name is required", nameof(dto.Name));
        }

        // Check for duplicate names within the same parent
        if (dto.ParentCategoryId.HasValue)
        {
            var siblings = await GetChildCategoriesAsync(dto.ParentCategoryId.Value);
            if (siblings.Any(s => s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A category with the name '{dto.Name}' already exists in this location");
            }
        }
    }

    private async Task ValidateUpdateCategoryAsync(int categoryId, UpdateQuestionBankCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Category name is required", nameof(dto.Name));
        }

        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} not found", nameof(categoryId));
        }

        // Check for duplicate names within the same parent (excluding current category)
        if (category.ParentId.HasValue)
        {
            var siblings = await GetChildCategoriesAsync(category.ParentId.Value);
            if (siblings.Any(s => s.CategoryId != categoryId && s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A category with the name '{dto.Name}' already exists in this location");
            }
        }
    }

    private async Task ValidateDeleteCategoryAsync(int categoryId, bool cascade)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} not found", nameof(categoryId));
        }

        if (!cascade)
        {
            var children = await GetChildCategoriesAsync(categoryId);
            if (children.Any())
            {
                throw new InvalidOperationException($"Category has {children.Count()} child categories. Use cascade delete or move children first.");
            }

            var questionCount = await GetSubtreeQuestionCountAsync(categoryId);
            if (questionCount > 0)
            {
                throw new InvalidOperationException($"Category has {questionCount} questions. Move questions first or use cascade delete.");
            }
        }
    }

    private async Task ValidateMoveCategoryAsync(int categoryId, int? newParentId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} not found", nameof(categoryId));
        }

        if (newParentId.HasValue)
        {
            var newParent = await _categoryRepository.GetByIdAsync(newParentId.Value);
            if (newParent == null)
            {
                throw new ArgumentException($"Parent category with ID {newParentId.Value} not found", nameof(newParentId));
            }

            // Prevent moving a category to itself or its descendants
            var wouldCreateCircular = await _treeRepository.WouldCreateCircularReferenceAsync(categoryId, newParentId.Value);
            if (wouldCreateCircular)
            {
                throw new InvalidOperationException("Cannot move a category to itself or its descendants");
            }
        }
    }

    #endregion
}
