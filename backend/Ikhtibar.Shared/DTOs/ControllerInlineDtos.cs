// Minimal compatibility DTOs expected by API and Core interfaces.
// These were small, controller-local DTOs moved here to avoid missing type errors.
namespace Ikhtibar.Shared.DTOs
{
	public class QuestionImportOptionsDto
	{
		public int? QuestionBankId { get; set; }
		public int? PrimaryTreeNodeId { get; set; }
		public bool SkipDuplicates { get; set; } = true;
		public bool ValidateBeforeImport { get; set; } = true;
		public string? DefaultQuestionType { get; set; }
		public string? DefaultDifficultyLevel { get; set; }
		public int? DefaultPoints { get; set; }
		public int? DefaultEstimatedTime { get; set; }
		public int ImportedBy { get; set; }
	}

	public class QuestionExportOptionsDto
	{
		public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
		public int? QuestionBankId { get; set; }
		public int? TreeNodeId { get; set; }
		public string? Format { get; set; } = "excel";
		public bool IncludeAnswers { get; set; } = true;
		public bool IncludeMetadata { get; set; } = true;
		public bool IncludeTags { get; set; } = true;
		public string? DateRange { get; set; }
	}
}

namespace Ikhtibar.Shared.DTOs
{
	// Validation DTOs used by API
	public class ValidateQuestionContentDto
	{
		public string QuestionText { get; set; } = string.Empty;
		public string? Solution { get; set; }
		public int QuestionTypeId { get; set; }
		public string? Language { get; set; } = "en";
	}

	public class ValidateQuestionAnswersDto
	{
		public int QuestionTypeId { get; set; }
		public IEnumerable<ValidateAnswerDto> Answers { get; set; } = new List<ValidateAnswerDto>();
		public int? CorrectAnswerCount { get; set; }
	}

	public class ValidateAnswerDto
	{
		public string Text { get; set; } = string.Empty;
		public bool IsCorrect { get; set; }
		public string? Explanation { get; set; }
		public int? Order { get; set; }
	}

	public class ValidateQuestionMetadataDto
	{
		public string? Tags { get; set; }
		public string? Metadata { get; set; }
		public int? DifficultyLevelId { get; set; }
		public int? EstimatedTimeSec { get; set; }
		public decimal? Points { get; set; }
	}

	public class CheckDuplicatesDto
	{
		public string QuestionText { get; set; } = string.Empty;
		public int? QuestionTypeId { get; set; }
		public int? DifficultyLevelId { get; set; }
		public string? Tags { get; set; }
		public double SimilarityThreshold { get; set; } = 0.8;
	}

	public class ValidateQuestionAccessibilityDto
	{
		public string QuestionText { get; set; } = string.Empty;
		public string? Solution { get; set; }
		public IEnumerable<string> RequiredFeatures { get; set; } = new List<string>();
		public string? Language { get; set; } = "en";
	}

	public class ValidateQuestionDifficultyDto
	{
		public string QuestionText { get; set; } = string.Empty;
		public int QuestionTypeId { get; set; }
		public int DifficultyLevelId { get; set; }
		public int? EstimatedTimeSec { get; set; }
		public decimal? Points { get; set; }
	}

	// Import-related DTOs
	public class QuestionImportValidationDto
	{
		public string Content { get; set; } = string.Empty;
		public string Format { get; set; } = string.Empty;
		public QuestionImportOptionsDto? Options { get; set; }
	}

	public class QuestionImportValidationResultDto
	{
		public bool IsValid { get; set; }
		public int TotalRows { get; set; }
		public int ValidRows { get; set; }
		public int InvalidRows { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
		public List<string> Warnings { get; set; } = new List<string>();
		public List<QuestionImportRowValidationDto> RowValidations { get; set; } = new List<QuestionImportRowValidationDto>();
	}

	public class QuestionImportRowValidationDto
	{
		public int RowNumber { get; set; }
		public bool IsValid { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
		public List<string> Warnings { get; set; } = new List<string>();
		public string? QuestionText { get; set; }
	}

	public class QuestionImportStatisticsDto
	{
		public int TotalImports { get; set; }
		public int SuccessfulImports { get; set; }
		public int FailedImports { get; set; }
		public int TotalQuestionsImported { get; set; }
		public DateTime LastImport { get; set; }
		public double AverageImportTime { get; set; }
		public Dictionary<string, int> ImportFormats { get; set; } = new();
	}

	public class QuestionValidationHistoryDto
	{
		public int Id { get; set; }
		public int QuestionId { get; set; }
		public string ValidationType { get; set; } = string.Empty;
		public bool IsValid { get; set; }
		public string? Details { get; set; }
		public DateTime ValidatedAt { get; set; }
		public int ValidatedBy { get; set; }
	}

	public class BulkValidationResultDto
	{
		public int TotalQuestions { get; set; }
		public int ValidQuestions { get; set; }
		public int InvalidQuestions { get; set; }
		public List<QuestionValidationResult> Results { get; set; } = new List<QuestionValidationResult>();
		public List<string> Errors { get; set; } = new List<string>();
	}
}
