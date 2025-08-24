
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionValidationRepository : IBaseRepository<QuestionValidation>
    {
        // Validation retrieval
        Task<QuestionValidation?> GetByIdWithDetailsAsync(int validationId);
        Task<QuestionValidation?> GetLatestValidationAsync(int questionId);
        Task<IEnumerable<QuestionValidation>> GetValidationHistoryAsync(int questionId);
        Task<IEnumerable<QuestionValidation>> GetValidationsByStatusAsync(ValidationStatus status);
        Task<IEnumerable<QuestionValidation>> GetValidationsByValidatorAsync(int validatorId);
        
        // Validation management
        Task<QuestionValidation> CreateValidationAsync(QuestionValidation validation);
        Task<bool> UpdateValidationAsync(QuestionValidation validation);
        Task<bool> DeleteValidationAsync(int validationId);
        Task<bool> ArchiveValidationAsync(int validationId);
        
        // Validation operations
        Task<bool> ApproveValidationAsync(int validationId, int approvedBy, string? notes);
        Task<bool> RejectValidationAsync(int validationId, int rejectedBy, string reason);
        Task<bool> MarkValidationInProgressAsync(int validationId);
        Task<bool> MarkValidationCompletedAsync(int validationId);
        
        // Validation rules
        Task<IEnumerable<ValidationRuleDto>> GetValidationRulesAsync(int questionTypeId);
        Task<ValidationRuleDto?> GetValidationRuleAsync(int ruleId);
        Task<ValidationRuleDto> CreateValidationRuleAsync(ValidationRuleDto rule);
        Task<bool> UpdateValidationRuleAsync(ValidationRuleDto rule);
        Task<bool> DeleteValidationRuleAsync(int ruleId);
        
        // Validation results
        Task<IEnumerable<ValidationResult>> GetValidationResultsAsync(int questionId);
        Task<ValidationResult?> GetLatestValidationResultAsync(int questionId);
        Task<ValidationResult> CreateValidationResultAsync(ValidationResult result);
        Task<bool> UpdateValidationResultAsync(ValidationResult result);
        
        // Quality assessment
        Task<QuestionQualityScoreDto> GetQuestionQualityScoreAsync(int questionId);
        Task<IEnumerable<QuestionQualityIssueDto>> GetQualityIssuesAsync(int questionId);
        Task<QuestionQualityIssueDto> CreateQualityIssueAsync(QuestionQualityIssueDto issue);
        Task<bool> MarkQualityIssueResolvedAsync(int issueId, int resolvedBy, string resolution);
        
        // Validation analytics
        Task<ValidationStatisticsDto> GetValidationStatisticsAsync(ValidationStatisticsFilterDto filter);
        Task<IEnumerable<ValidationTrendDto>> GetValidationTrendsAsync(DateTime fromDate, DateTime toDate);
        Task<ValidationAnalyticsDto> GetValidationAnalyticsAsync(int questionId, DateTime fromDate, DateTime toDate);
        
        // Validation workflow
        Task<IEnumerable<QuestionValidation>> GetPendingValidationsAsync(int validatorId);
        Task<IEnumerable<QuestionValidation>> GetOverdueValidationsAsync();
        Task<bool> AssignValidationAsync(int validationId, int validatorId);
        Task<bool> ReassignValidationAsync(int validationId, int newValidatorId);
        
        // Validation cleanup
        Task<bool> CleanupOldValidationsAsync(DateTime olderThan);
        Task<bool> ArchiveCompletedValidationsAsync(DateTime olderThan);
        Task<bool> OptimizeValidationStorageAsync();
    }
}
