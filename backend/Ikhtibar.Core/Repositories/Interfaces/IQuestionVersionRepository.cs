
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionVersionRepository : IBaseRepository<QuestionVersion>
    {
        // Version retrieval
        Task<QuestionVersion?> GetByIdWithDetailsAsync(int versionId);
        Task<QuestionVersion?> GetVersionAsync(int questionId, string version);
        Task<QuestionVersion?> GetCurrentVersionAsync(int questionId);
        Task<IEnumerable<QuestionVersion>> GetQuestionVersionsAsync(int questionId);
        Task<IEnumerable<QuestionVersion>> GetVersionsByStatusAsync(int questionId, VersionStatus status);
        
        // Version management
        Task<QuestionVersion> CreateVersionAsync(QuestionVersion version);
        Task<bool> UpdateVersionAsync(QuestionVersion version);
        Task<bool> DeleteVersionAsync(int versionId);
        Task<bool> ArchiveVersionAsync(int questionId, string version);
        Task<bool> RestoreVersionAsync(int questionId, string version);
        
        // Version control
        Task<bool> SetCurrentVersionAsync(int questionId, string version);
        Task<bool> ValidateVersionNumberAsync(int questionId, string version);
        Task<string> GenerateNextVersionNumberAsync(int questionId);
        Task<IEnumerable<string>> GetExistingVersionNumbersAsync(int questionId);
        
        // Version comparison
        Task<VersionDiffResult> CompareVersionsAsync(int questionId, string version1, string version2);
        Task<IEnumerable<VersionChangeDto>> GetVersionChangesAsync(int questionId, string fromVersion, string toVersion);
        Task<VersionMetadataDto> GetVersionMetadataAsync(int questionId, string version);
        
        // Version history
        Task<IEnumerable<QuestionVersion>> GetVersionHistoryAsync(int questionId, int limit = 50);
        Task<QuestionVersion?> GetPreviousVersionAsync(int questionId, string currentVersion);
        Task<QuestionVersion?> GetNextVersionAsync(int questionId, string currentVersion);
        Task<IEnumerable<QuestionVersion>> GetVersionsInRangeAsync(int questionId, string fromVersion, string toVersion);
        
        // Version analytics
        Task<VersionStatisticsDto> GetVersionStatisticsAsync(int questionId);
        Task<IEnumerable<VersionUsageDto>> GetVersionUsageAsync(int questionId, DateTime fromDate, DateTime toDate);
        Task<VersionAnalyticsDto> GetVersionAnalyticsAsync(int questionId, DateTime fromDate, DateTime toDate);
        
        // Bulk version operations
        Task<bool> BulkCreateVersionsAsync(IEnumerable<QuestionVersion> versions);
        Task<bool> BulkUpdateVersionsAsync(IEnumerable<QuestionVersion> versions);
        Task<bool> BulkArchiveVersionsAsync(IEnumerable<int> versionIds);
        Task<bool> BulkSetCurrentVersionsAsync(IEnumerable<(int QuestionId, string Version)> versionMappings);
        
        // Version cleanup
        Task<bool> CleanupOldVersionsAsync(int questionId, int keepVersions = 10);
        Task<bool> CleanupArchivedVersionsAsync(DateTime olderThan);
        Task<bool> OptimizeVersionStorageAsync(int questionId);
    }
}
