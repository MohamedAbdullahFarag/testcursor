using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionAnalyticsService
    {
        // Question performance analytics
        Task<QuestionPerformanceDto> GetQuestionPerformanceAsync(int questionId, DateTime fromDate, DateTime toDate);
        // Overload without explicit date range (API callers may use this)
        Task<QuestionPerformanceDto> GetQuestionPerformanceAsync(int questionId);
        Task<IEnumerable<QuestionPerformanceDto>> GetQuestionsPerformanceAsync(IEnumerable<int> questionIds, DateTime fromDate, DateTime toDate);
        Task<QuestionPerformanceSummaryDto> GetPerformanceSummaryAsync(PerformanceSummaryFilterDto filter);
    Task<QuestionPerformanceSummaryDto> GetQuestionPerformanceSummaryAsync(int questionId);
    Task<QuestionQualityDto> GetQuestionQualityAsync(int questionId);
    Task<DifficultyAnalysisDto> GetQuestionDifficultyAnalysisAsync(int questionId);
    Task<IEnumerable<QuestionPerformanceTrendDto>> GetPerformanceTrendsAsync(int questionId, DateTime fromDate, DateTime toDate);
    // Nullable-date overload
    Task<IEnumerable<QuestionPerformanceTrendDto>> GetQuestionPerformanceTrendsAsync(int questionId, DateTime? fromDate, DateTime? toDate);
    // Controller-friendly overloads
    Task<IEnumerable<QuestionPerformanceTrendDto>> GetPerformanceTrendsAsync(int questionId);
    Task<IEnumerable<QuestionPerformanceTrendDto>> GetPerformanceTrendsAsync(int questionId, DateTime? fromDate, DateTime? toDate);
    // Controller-friendly: trends for all questions (date range optional)
    Task<IEnumerable<PerformanceTrendDto>> GetPerformanceTrendsAsync(DateTime? fromDate, DateTime? toDate);

    // Additional analytics used by API controllers
    Task<IEnumerable<DifficultyLevelCountDto>> GetDifficultyLevelAnalyticsAsync();
    Task<IEnumerable<UsageTrendDto>> GetQuestionUsageTrendsAsync(DateTime? fromDate, DateTime? toDate);
    Task<IEnumerable<PopularTagDto>> GetPopularTagsAsync(int limit = 20);
    Task<IEnumerable<TagCategoryDto>> GetTagCategoriesAsync();
    Task<IEnumerable<TemplateQualityDto>> GetTemplateQualityAnalyticsAsync();
    Task<IEnumerable<VersionAnalyticsDto>> GetVersionAnalyticsAsync();
    Task<IEnumerable<ValidationStatisticsDto>> GetValidationStatisticsAsync();
    Task<IEnumerable<ReportingDto>> GetReportingAnalyticsAsync();
    Task<CustomAnalyticsReportDto> GetCustomAnalyticsReportAsync(CustomAnalyticsRequestDto request);
    Task<AnalyticsExportDto> ExportAnalyticsDataAsync(AnalyticsExportRequestDto request);

        // Usage analytics
        Task<QuestionUsageStatisticsDto> GetQuestionUsageAsync(int questionId, DateTime fromDate, DateTime toDate);
        // Overload without explicit date range
        Task<QuestionUsageStatisticsDto> GetQuestionUsageAsync(int questionId);
        Task<IEnumerable<QuestionUsageDto>> GetQuestionsUsageAsync(IEnumerable<int> questionIds, DateTime fromDate, DateTime toDate);
        Task<UsageSummaryDto> GetUsageSummaryAsync(UsageSummaryFilterDto filter);
        Task<IEnumerable<UsageTrendDto>> GetUsageTrendsAsync(DateTime fromDate, DateTime toDate);

        // Quality analytics
        Task<QuestionQualityMetricsDto> GetQuestionQualityMetricsAsync(int questionId);
        Task<IEnumerable<QuestionQualityMetricsDto>> GetQuestionsQualityMetricsAsync(IEnumerable<int> questionIds);
        Task<QualitySummaryDto> GetQualitySummaryAsync(QualitySummaryFilterDto filter);
        Task<IEnumerable<QualityTrendDto>> GetQualityTrendsAsync(DateTime fromDate, DateTime toDate);

        // Bank and category analytics
    Task<QuestionBankAnalyticsDto> GetQuestionBankAnalyticsAsync(int questionBankId, DateTime fromDate, DateTime toDate);
    // Controller-friendly overload: get bank analytics without dates
    Task<QuestionBankAnalyticsDto> GetQuestionBankAnalyticsAsync(int questionBankId);
        Task<TreeNodeAnalyticsDto> GetTreeNodeAnalyticsAsync(int treeNodeId, DateTime fromDate, DateTime toDate);
        Task<CategoryAnalyticsDto> GetCategoryAnalyticsAsync(string category, DateTime fromDate, DateTime toDate);
        Task<TypeAnalyticsDto> GetQuestionTypeAnalyticsAsync(int questionTypeId, DateTime fromDate, DateTime toDate);
        // Parameterless question type analytics
        Task<IEnumerable<QuestionTypeCountDto>> GetQuestionTypeAnalyticsAsync();

        // User and creator analytics
        Task<UserQuestionAnalyticsDto> GetUserQuestionAnalyticsAsync(int userId, DateTime fromDate, DateTime toDate);
        Task<CreatorAnalyticsDto> GetCreatorAnalyticsAsync(int creatorId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<UserQuestionAnalyticsDto>> GetTopCreatorsAsync(int limit = 10, DateTime fromDate = default, DateTime toDate = default);

        // Advanced analytics
        Task<QuestionSimilarityDto> GetQuestionSimilarityAsync(int questionId, int limit = 10);
        // Return similar questions directly
        Task<IEnumerable<QuestionDto>> GetSimilarQuestionsAsync(int questionId, int limit = 10);
        Task<IEnumerable<QuestionClusterDto>> GetQuestionClustersAsync(ClusteringOptionsDto options);
        Task<QuestionRecommendationDto> GetQuestionRecommendationsAsync(int questionId, int limit = 10);
        Task<QuestionPatternDto> GetQuestionPatternsAsync(PatternAnalysisOptionsDto options);

        // Reporting
        Task<byte[]> GenerateAnalyticsReportAsync(AnalyticsReportDto reportDto);
        Task<AnalyticsDashboardDto> GetAnalyticsDashboardAsync(DashboardFilterDto filter);
        Task<IEnumerable<AnalyticsAlertDto>> GetAnalyticsAlertsAsync(AlertFilterDto filter);
        Task<bool> ScheduleAnalyticsReportAsync(ScheduleAnalyticsReportDto dto);
    }
}
