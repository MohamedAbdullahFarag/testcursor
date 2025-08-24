
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionUsageRepository : IBaseRepository<QuestionUsageHistory>
    {
        // Usage retrieval
        Task<QuestionUsageHistory?> GetByIdWithDetailsAsync(int usageId);
        Task<IEnumerable<QuestionUsageHistory>> GetByQuestionAsync(int questionId);
        Task<IEnumerable<QuestionUsageHistory>> GetByUserAsync(int userId);
        Task<IEnumerable<QuestionUsageHistory>> GetByExamAsync(int examId);
        Task<IEnumerable<QuestionUsageHistory>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        
        // Usage listing
        Task<(IEnumerable<QuestionUsageHistory> UsageHistory, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null);
        Task<IEnumerable<QuestionUsageHistory>> GetByFilterAsync(UsageFilterDto filter);
        Task<IEnumerable<QuestionUsageHistory>> GetRecentUsageAsync(int limit = 100);
        Task<IEnumerable<QuestionUsageHistory>> GetUsageByTypeAsync(string usageType);
        
        // Usage tracking
        Task<QuestionUsageHistory> TrackUsageAsync(QuestionUsageHistory usage);
        Task<bool> UpdateUsageAsync(QuestionUsageHistory usage);
        Task<bool> DeleteUsageAsync(int usageId);
        Task<bool> BulkTrackUsageAsync(IEnumerable<QuestionUsageHistory> usageRecords);
        
        // Usage statistics
        Task<QuestionUsageStatisticsDto> GetUsageStatisticsAsync(int questionId, DateTime fromDate, DateTime toDate);
        Task<int> GetUsageCountAsync(int questionId);
        Task<int> GetUniqueUserCountAsync(int questionId);
        Task<double> GetAverageScoreAsync(int questionId);
        Task<double> GetAverageTimeSpentAsync(int questionId);
        
        // Usage analytics
        Task<QuestionUsageDto> GetUsageAnalyticsAsync(int questionId);
        Task<IEnumerable<UsageTrendDto>> GetUsageTrendsAsync(int questionId, string period);
        Task<IEnumerable<QuestionUsageDto>> GetMostUsedQuestionsAsync(int limit = 20);
        Task<IEnumerable<QuestionUsageDto>> GetLeastUsedQuestionsAsync(int limit = 20);
        
        // User analytics
        Task<UserUsageAnalyticsDto> GetUserUsageAnalyticsAsync(int userId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<QuestionUsageDto>> GetUserQuestionUsageAsync(int userId);
        Task<IEnumerable<UserPerformanceDto>> GetUserPerformanceAsync(int userId);
        Task<IEnumerable<UserTrendDto>> GetUserUsageTrendsAsync(int userId);
        
        // Exam analytics
        Task<ExamUsageAnalyticsDto> GetExamUsageAnalyticsAsync(int examId);
        Task<IEnumerable<QuestionUsageDto>> GetExamQuestionUsageAsync(int examId);
        Task<IEnumerable<ExamPerformanceDto>> GetExamPerformanceAsync(int examId);
        Task<IEnumerable<ExamTrendDto>> GetExamUsageTrendsAsync(int examId);
        
        // Advanced analytics
        Task<IEnumerable<QuestionPerformanceDto>> GetQuestionPerformanceAsync(PerformanceFilterDto filter);
        Task<IEnumerable<QuestionPerformanceSummaryDto>> GetPerformanceSummaryAsync(PerformanceSummaryFilterDto filter);
        Task<IEnumerable<QuestionPerformanceTrendDto>> GetPerformanceTrendsAsync(int questionId, string period);
        Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(AdvancedAnalyticsFilterDto filter);
        
        // Reporting
        Task<UsageReportDto> GenerateUsageReportAsync(UsageReportFilterDto filter);
        Task<PerformanceReportDto> GeneratePerformanceReportAsync(PerformanceReportFilterDto filter);
        Task<AnalyticsReportDto> GenerateAnalyticsReportAsync(AnalyticsReportFilterDto filter);
        Task<bool> ExportUsageDataAsync(ExportOptionsDto options, string exportPath);
        
        // Usage cleanup
        Task<int> CleanupOldUsageDataAsync(int daysOld = 1095); // 3 years
        Task<int> ArchiveUsageDataAsync(DateTime cutoffDate);
        Task<bool> RestoreUsageDataAsync(int usageId);
        Task<int> CompressUsageDataAsync(int daysOld = 365);
    }
}
