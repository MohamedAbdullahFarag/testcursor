using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question template information
/// </summary>
    public class QuestionTemplateDto
    {
        /// <summary>
        /// Unique identifier for the question template
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the question template
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the question template
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Code identifier for the question template
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Category of the question template
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// ID of the question type this template is based on
        /// </summary>
        public int QuestionTypeId { get; set; }

        /// <summary>
        /// Name of the question type
        /// </summary>
        public string? QuestionTypeName { get; set; }

        /// <summary>
        /// ID of the question difficulty level
        /// </summary>
        public int? DifficultyLevelId { get; set; }

        /// <summary>
        /// Name of the difficulty level
        /// </summary>
        public string? DifficultyLevelName { get; set; }

        /// <summary>
        /// ID of the question bank this template belongs to
        /// </summary>
        public int? QuestionBankId { get; set; }

        /// <summary>
        /// Name of the question bank
        /// </summary>
        public string? QuestionBankName { get; set; }

        /// <summary>
        /// ID of the curriculum this template aligns with
        /// </summary>
        public int? CurriculumId { get; set; }

        /// <summary>
        /// Name of the curriculum
        /// </summary>
        public string? CurriculumName { get; set; }

        /// <summary>
        /// ID of the subject this template belongs to
        /// </summary>
        public int? SubjectId { get; set; }

        /// <summary>
        /// Name of the subject
        /// </summary>
        public string? SubjectName { get; set; }

        /// <summary>
        /// ID of the grade level this template is designed for
        /// </summary>
        public int? GradeLevelId { get; set; }

        /// <summary>
        /// Name of the grade level
        /// </summary>
        public string? GradeLevelName { get; set; }

        /// <summary>
        /// Template content in JSON format
        /// </summary>
        [Required]
        public string TemplateContent { get; set; } = string.Empty;

        /// <summary>
        /// Template structure definition
        /// </summary>
        public string? StructureDefinition { get; set; }

        /// <summary>
        /// Validation rules for the template
        /// </summary>
        public string? ValidationRules { get; set; }

        /// <summary>
        /// Default points for questions created from this template
        /// </summary>
        [Range(0, 100)]
        public int DefaultPoints { get; set; } = 1;

        /// <summary>
        /// Estimated time in seconds to answer questions from this template
        /// </summary>
        [Range(1, 3600)]
        public int? EstimatedTimeSec { get; set; }

        /// <summary>
        /// Whether the template is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the template is publicly available
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the template is a system template
        /// </summary>
        public bool IsSystemTemplate { get; set; }

        /// <summary>
        /// Version number of the template
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// ID of the user who created this template
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Name of the user who created this template
        /// </summary>
        public string? CreatedByName { get; set; }

        /// <summary>
        /// When the template was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated this template
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// Name of the user who last updated this template
        /// </summary>
        public string? UpdatedByName { get; set; }

        /// <summary>
        /// When the template was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Additional metadata in JSON format
        /// </summary>
        public string? MetadataJson { get; set; }

        /// <summary>
        /// Number of questions created from this template
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// Average score for questions from this template
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// Pass rate for questions from this template
        /// </summary>
        public double PassRate { get; set; }

        /// <summary>
        /// Tags associated with this template
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class CreateQuestionFromTemplateDto
    {
        [Required]
        public int TemplateId { get; set; }
        
        [Required]
        public int QuestionBankId { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public string? CustomText { get; set; }
        
        public string? Tags { get; set; }
        
        public string? Metadata { get; set; }
    }
