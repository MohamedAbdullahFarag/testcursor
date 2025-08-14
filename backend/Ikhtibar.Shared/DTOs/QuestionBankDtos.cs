using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question bank category
/// </summary>
public class QuestionBankCategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? TreePath { get; set; }
    public int TreeLevel { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string? Metadata { get; set; }
    public int QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ParentCategoryName { get; set; }
    public IEnumerable<QuestionBankCategoryDto> Children { get; set; } = new List<QuestionBankCategoryDto>();
}

/// <summary>
/// DTO for creating a question bank category
/// </summary>
public class CreateQuestionBankCategoryDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(20)]
    public string? Color { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    [StringLength(1000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// DTO for updating a question bank category
/// </summary>
public class UpdateQuestionBankCategoryDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(20)]
    public string? Color { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    [StringLength(1000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// DTO for question bank tree structure
/// </summary>
public class QuestionBankTreeDto
{
    public IEnumerable<QuestionBankCategoryDto> Categories { get; set; } = new List<QuestionBankCategoryDto>();
    public int TotalCategories { get; set; }
    public int MaxDepth { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// DTO for question categorization
/// </summary>
public class QuestionCategorizationDto
{
    public int QuestionCategorizationId { get; set; }
    public int QuestionId { get; set; }
    public int CategoryId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedBy { get; set; }
    public decimal? Weight { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public string? QuestionText { get; set; }
    public string? CategoryName { get; set; }
    public string? AssignedByName { get; set; }
}

/// <summary>
/// DTO for creating question categorization
/// </summary>
public class CreateQuestionCategorizationDto
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsPrimary { get; set; } = false;

    public decimal? Weight { get; set; }

    public decimal? ConfidenceScore { get; set; }
}

/// <summary>
/// DTO for bulk question assignment
/// </summary>
public class BulkAssignQuestionsDto
{
    [Required]
    public IEnumerable<int> QuestionIds { get; set; } = new List<int>();

    [Required]
    public int CategoryId { get; set; }

    public bool SetAsPrimary { get; set; } = false;
}

/// <summary>
/// DTO for moving questions between categories
/// </summary>
public class MoveQuestionsDto
{
    [Required]
    public IEnumerable<int> QuestionIds { get; set; } = new List<int>();

    [Required]
    public int FromCategoryId { get; set; }

    [Required]
    public int ToCategoryId { get; set; }
}

/// <summary>
/// DTO for question hierarchy analytics
/// </summary>
public class QuestionHierarchyAnalyticsDto
{
    public IDictionary<int, int> CategoryQuestionCounts { get; set; } = new Dictionary<int, int>();
    public IDictionary<int, int> DifficultyDistribution { get; set; } = new Dictionary<int, int>();
    public IDictionary<int, int> QuestionTypeDistribution { get; set; } = new Dictionary<int, int>();
    public decimal AverageDifficulty { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime AnalysisDate { get; set; }
}
