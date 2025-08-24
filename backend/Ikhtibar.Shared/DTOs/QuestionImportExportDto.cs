using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class ExportResultDto
    {
        public bool IsSuccess { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string? ErrorMessage { get; set; }
        public int ExportedQuestions { get; set; }
        public DateTime ExportedAt { get; set; }
        public string ExportFormat { get; set; } = string.Empty;
    }

    public class ExportQuestionsDto
    {
        public List<int> QuestionIds { get; set; } = new List<int>();
        public int? QuestionBankId { get; set; }
        public string ExportFormat { get; set; } = "Excel"; // Excel, CSV, JSON, XML
        public bool IncludeAnswers { get; set; } = true;
        public bool IncludeMetadata { get; set; } = true;
        public bool IncludeTags { get; set; } = true;
        public string? CustomFileName { get; set; }
        public Dictionary<string, object>? ExportOptions { get; set; }
    }

    public class ExportTemplateDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty; // PDF, Word, Excel, HTML
        public string? CustomizationOptions { get; set; }
        public bool IncludeMetadata { get; set; }
        public bool IncludeInstructions { get; set; }
        public string? ExportPath { get; set; }
    }

    public class ImportPreviewDto
    {
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
        public List<string> Headers { get; set; } = new List<string>();
        public List<Dictionary<string, object>> SampleData { get; set; } = new List<Dictionary<string, object>>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class SupportedFormatDto
    {
        public string FormatName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSupported { get; set; }
        public int MaxFileSize { get; set; } // in bytes
        public string? ValidationRules { get; set; }
    }

    public class FormatValidationResult
    {
        public bool IsValid { get; set; }
        public string Format { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public string? SuggestedFormat { get; set; }
    }

    public class ImportConfigurationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileFormat { get; set; } = string.Empty;
        public string? FieldMapping { get; set; } // JSON mapping
        public string? ValidationRules { get; set; } // JSON rules
        public bool AutoValidate { get; set; } = true;
        public bool AutoPublish { get; set; } = false;
        public string? DefaultTags { get; set; }
        public int? DefaultQuestionBankId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateImportConfigurationDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? FieldMapping { get; set; }
        public string? ValidationRules { get; set; }
        public bool? AutoValidate { get; set; }
        public bool? AutoPublish { get; set; }
        public string? DefaultTags { get; set; }
        public int? DefaultQuestionBankId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ExportConfigurationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExportFormat { get; set; } = string.Empty;
        public string? FieldSelection { get; set; } // JSON field selection
        public string? FormattingOptions { get; set; } // JSON formatting
        public bool IncludeMetadata { get; set; } = true;
        public bool IncludeAnswers { get; set; } = true;
        public bool IncludeTags { get; set; } = true;
        public string? DefaultFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateExportConfigurationDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? FieldSelection { get; set; }
        public string? FormattingOptions { get; set; }
        public bool? IncludeMetadata { get; set; }
        public bool? IncludeAnswers { get; set; }
        public bool? IncludeTags { get; set; }
        public string? DefaultFileName { get; set; }
        public bool? IsActive { get; set; }
    }
}
