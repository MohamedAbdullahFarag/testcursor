using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for category order operations
/// </summary>
public class CategoryOrderDto
{
    public int CategoryId { get; set; }
    public int NewOrder { get; set; }
    public int? ParentId { get; set; }
}

/// <summary>
/// DTO for moving categories between parents
/// </summary>
public class MoveCategoryDto
{
    public int CategoryId { get; set; }
    public int? NewParentId { get; set; }
    public int? NewOrder { get; set; }
    public ChildHandlingStrategy ChildHandling { get; set; } = ChildHandlingStrategy.MoveWithParent;
    public bool PreserveOrder { get; set; } = true;
}

/// <summary>
/// DTO for copying categories
/// </summary>
public class CopyCategoryDto
{
    public int? NewParentId { get; set; }
    public string? NewName { get; set; }
    public bool IncludeChildren { get; set; } = true;
    public bool IncludeQuestions { get; set; } = false;
    public bool PreserveMetadata { get; set; } = true;
    public Dictionary<string, object>? Customizations { get; set; }
}

/// <summary>
/// DTO for reordering categories
/// </summary>
public class ReorderCategoriesDto
{
    public List<int> CategoryIds { get; set; } = new();
    public int? ParentId { get; set; }
    public bool PreserveExistingOrder { get; set; } = true;
    public Dictionary<int, int>? CustomOrder { get; set; }
}

/// <summary>
/// DTO for tree validation results
/// </summary>
public class TreeValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public int TotalNodesChecked { get; set; }
    public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? ValidationDetails { get; set; }
}

/// <summary>
/// DTO for tree statistics
/// </summary>
public class TreeStatisticsDto
{
    public int TotalCategories { get; set; }
    public int TotalRootCategories { get; set; }
    public int MaxDepth { get; set; }
    public int AvgChildrenPerCategory { get; set; }
    public int TotalQuestionsAssigned { get; set; }
    public int CategoriesWithoutQuestions { get; set; }
    public int CategoriesWithQuestions { get; set; }
    public Dictionary<int, int> CategoriesByDepth { get; set; } = new();
    public Dictionary<CategoryType, int> CategoriesByType { get; set; } = new();
    public Dictionary<CategoryLevel, int> CategoriesByLevel { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
