using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionValidationService
    {
        // Content validation
        Task<ValidationResult> ValidateContentAsync(ValidateQuestionDto dto);
        Task<ValidationResult> ValidateAnswersAsync(ValidateQuestionDto dto);
        Task<ValidationResult> ValidateMediaAsync(ValidateQuestionDto dto);
        Task<ValidationResult> ValidateBusinessRulesAsync(ValidateQuestionDto dto);
        
        // Question validation
        Task<QuestionValidationResult> ValidateQuestionAsync(ValidateQuestionDto dto);
        Task<QuestionValidationResult> ValidateQuestionUpdateAsync(int questionId, UpdateQuestionDto dto);
        Task<QuestionValidationResult> ValidateQuestionDataAsync(CreateQuestionDto dto);
        
    // Validation rules
    // Returns all validation rules or rules filtered by question type when provided
    Task<IEnumerable<ValidationRuleDto>> GetValidationRulesAsync();
    Task<IEnumerable<ValidationRuleDto>> GetValidationRulesAsync(int questionTypeId);
    Task<ValidationRuleDto> GetValidationRuleAsync(int ruleId);
    Task<ValidationRuleDto> CreateValidationRuleAsync(CreateValidationRuleDto dto);
    Task<ValidationRuleDto> UpdateValidationRuleAsync(int ruleId, UpdateValidationRuleDto dto);
    Task<bool> DeleteValidationRuleAsync(int ruleId);
        
    // Validation history
    Task<IEnumerable<QuestionValidationDto>> GetValidationHistoryAsync(int questionId);
    Task<QuestionValidationDto> GetLatestValidationAsync(int questionId);
    Task<bool> ApproveValidationAsync(int validationId, int approvedBy, string? notes);
    Task<bool> RejectValidationAsync(int validationId, int rejectedBy, string reason);
    // API-friendly history accessor expected by controllers
    Task<IEnumerable<QuestionValidationHistoryDto>> GetQuestionValidationHistoryAsync(int questionId);
        
        // Quality assessment
        Task<QuestionQualityScoreDto> AssessQuestionQualityAsync(int questionId);
        Task<IEnumerable<QuestionQualityIssueDto>> GetQualityIssuesAsync(int questionId);
        Task<bool> MarkQualityIssueResolvedAsync(int issueId, int resolvedBy, string resolution);

    // API-friendly method names (accept specific DTOs used by controllers)
    Task<ValidationResult> ValidateQuestionContentAsync(ValidateQuestionContentDto dto);
    Task<ValidationResult> ValidateQuestionAnswersAsync(ValidateQuestionAnswersDto dto);
    Task<ValidationResult> ValidateQuestionMetadataAsync(ValidateQuestionMetadataDto dto);
    Task<BulkValidationResultDto> RunBulkValidationAsync(BulkValidationDto dto);
    Task<IEnumerable<ValidationStatisticsDto>> GetValidationStatisticsAsync();
    Task<IEnumerable<DuplicateQuestionDto>> CheckForDuplicatesAsync(CheckDuplicatesDto dto);
    Task<QuestionValidationResult> ValidateQuestionAccessibilityAsync(ValidateQuestionAccessibilityDto dto);
    Task<QuestionValidationResult> ValidateQuestionDifficultyAsync(ValidateQuestionDifficultyDto dto);
    }
}
