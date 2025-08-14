using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.Models;

/// <summary>
/// Represents a node in the category tree
/// </summary>
public class CategoryTreeNode
{
    public QuestionBankCategory Category { get; set; } = null!;
    public List<CategoryTreeNode> Children { get; set; } = new();
    public int QuestionCount { get; set; }
    public int DirectQuestionCount { get; set; }
    public int Depth { get; set; }
    public bool HasChildren { get; set; }
}

/// <summary>
/// Breadcrumb item for navigation
/// </summary>
public class CategoryBreadcrumb
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public int Level { get; set; }
}

/// <summary>
/// Result of tree move operation
/// </summary>
public class TreeMoveResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int CategoriesAffected { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Result of tree copy operation
/// </summary>
public class TreeCopyResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int NewCategoryId { get; set; }
    public int CategoriesCopied { get; set; }
    public Dictionary<int, int> IdMapping { get; set; } = new(); // Old ID -> New ID
}

/// <summary>
/// Result of tree delete operation
/// </summary>
public class TreeDeleteResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int CategoriesDeleted { get; set; }
    public int QuestionsReassigned { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Tree validation result
/// </summary>
public class TreeValidationResult
{
    public bool IsValid { get; set; }
    public List<TreeValidationIssue> Issues { get; set; } = new();
    public int OrphanedCategories { get; set; }
    public int InvalidPaths { get; set; }
    public int CircularReferences { get; set; }
}

/// <summary>
/// Tree validation issue
/// </summary>
public class TreeValidationIssue
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string Severity { get; set; } = "Warning";
}

/// <summary>
/// Tree statistics
/// </summary>
public class TreeStatistics
{
    public int TotalCategories { get; set; }
    public int MaxDepth { get; set; }
    public int RootCategories { get; set; }
    public int LeafCategories { get; set; }
    public double AverageChildrenPerNode { get; set; }
    public double AverageDepth { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Tree import data structure
/// </summary>
public class CategoryTreeImport
{
    public List<CategoryImportNode> Categories { get; set; } = new();
}

/// <summary>
/// Category import node
/// </summary>
public class CategoryImportNode
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CategoryType Type { get; set; }
    public CategoryLevel Level { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AllowQuestions { get; set; } = true;
    public int SortOrder { get; set; }
    public List<CategoryImportNode> Children { get; set; } = new();
}

/// <summary>
/// Tree import result
/// </summary>
public class TreeImportResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int CategoriesCreated { get; set; }
    public int CategoriesSkipped { get; set; }
    public int CategoriesUpdated { get; set; }
    public Dictionary<string, int> CodeToIdMapping { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Tree export structure
/// </summary>
public class CategoryTreeExport
{
    public List<CategoryExportNode> Categories { get; set; } = new();
    public DateTime ExportedAt { get; set; }
    public string Version { get; set; } = "1.0";
}

/// <summary>
/// Category export node
/// </summary>
public class CategoryExportNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CategoryType Type { get; set; }
    public CategoryLevel Level { get; set; }
    public bool IsActive { get; set; }
    public bool AllowQuestions { get; set; }
    public int SortOrder { get; set; }
    public int QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public List<CategoryExportNode> Children { get; set; } = new();
}

/// <summary>
/// Tree search criteria
/// </summary>
public class TreeSearchCriteria
{
    public string? SearchTerm { get; set; }
    public CategoryType? Type { get; set; }
    public CategoryLevel? Level { get; set; }
    public bool SearchInDescriptions { get; set; } = true;
    public bool SearchInCodes { get; set; } = true;
    public int? WithinCategoryId { get; set; } // Search within specific subtree
    public bool IncludeInactive { get; set; } = false;
    public int MaxResults { get; set; } = 100;
}

/// <summary>
/// Category search result with tree context
/// </summary>
public class CategorySearchResult
{
    public QuestionBankCategory Category { get; set; } = null!;
    public List<CategoryBreadcrumb> Breadcrumbs { get; set; } = new();
    public string MatchType { get; set; } = string.Empty; // Name, Description, Code
    public double RelevanceScore { get; set; }
}

/// <summary>
/// Question with category details for hierarchy operations
/// </summary>
public class QuestionCategoryDetail
{
    public Question Question { get; set; } = null!;
    public QuestionBankCategory Category { get; set; } = null!;
    public QuestionCategorization Categorization { get; set; } = null!;
    public List<CategoryBreadcrumb> CategoryPath { get; set; } = new();
}

/// <summary>
/// Search criteria for questions by category
/// </summary>
public class QuestionCategorySearchCriteria
{
    public int? CategoryId { get; set; }
    public bool IncludeDescendants { get; set; } = false;
    public CategoryType? CategoryType { get; set; }
    public CategoryLevel? CategoryLevel { get; set; }
    public bool? IsPrimary { get; set; }
    public string? QuestionText { get; set; }
    public int? QuestionTypeId { get; set; }
    public int? DifficultyLevelId { get; set; }
    public decimal? MinWeight { get; set; }
    public decimal? MinConfidenceScore { get; set; }
    public DateTime? AssignedAfter { get; set; }
    public DateTime? AssignedBefore { get; set; }
    public int? AssignedByUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "Question.CreatedAt";
    public bool SortDescending { get; set; } = true;
}

/// <summary>
/// Categorization validation result
/// </summary>
public class CategorizationValidationResult
{
    public bool IsValid { get; set; }
    public List<CategorizationValidationIssue> Issues { get; set; } = new();
    public int OrphanedCategorizations { get; set; }
    public int DuplicateCategorizations { get; set; }
    public int InvalidHierarchyEntries { get; set; }
}

/// <summary>
/// Categorization validation issue
/// </summary>
public class CategorizationValidationIssue
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? QuestionId { get; set; }
    public int? CategoryId { get; set; }
    public string Severity { get; set; } = "Warning";
}

/// <summary>
/// Categorization statistics
/// </summary>
public class CategorizationStatistics
{
    public int TotalQuestions { get; set; }
    public int CategorizedQuestions { get; set; }
    public int UncategorizedQuestions { get; set; }
    public int QuestionsWithPrimaryCategory { get; set; }
    public int QuestionsWithMultipleCategories { get; set; }
    public double AverageCategoriesPerQuestion { get; set; }
    public int TotalCategorizations { get; set; }
    public int AutomaticCategorizations { get; set; }
    public int ManualCategorizations { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Category usage analytics
/// </summary>
public class CategoryUsageAnalytics
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryPath { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int NewQuestionsCount { get; set; }
    public int RemovedQuestionsCount { get; set; }
    public DateTime AnalysisPeriodStart { get; set; }
    public DateTime AnalysisPeriodEnd { get; set; }
    public double UsageGrowthRate { get; set; }
}

/// <summary>
/// Paged result wrapper for hierarchical data
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
