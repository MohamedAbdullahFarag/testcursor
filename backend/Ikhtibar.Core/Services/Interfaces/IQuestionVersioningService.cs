using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionVersioningService
    {
        // Version management
        Task<QuestionVersionDto> CreateInitialVersionAsync(int questionId, CreateQuestionDto dto);
        Task<QuestionVersionDto> CreateVersionFromUpdateAsync(int questionId, UpdateQuestionDto dto);
        Task<QuestionVersionDto> CreateVersionAsync(int questionId, CreateVersionDto dto);
        Task<QuestionVersionDto> GetVersionAsync(int questionId, string version);
        Task<IEnumerable<QuestionVersionDto>> GetQuestionVersionsAsync(int questionId);
        Task<QuestionVersionDto> GetCurrentVersionAsync(int questionId);
        
        // Version operations
        Task<bool> RestoreQuestionVersionAsync(int questionId, string version);
        Task<bool> CompareVersionsAsync(int questionId, string version1, string version2);
        Task<VersionDiffResult> GetVersionDiffAsync(int questionId, string version1, string version2);
        Task<bool> ArchiveVersionAsync(int questionId, string version);
        
        // Version control
        Task<bool> SetCurrentVersionAsync(int questionId, string version);
        Task<bool> RequiresNewVersionAsync(int questionId, UpdateQuestionDto dto);
        Task<string> GenerateNextVersionNumberAsync(int questionId);
        Task<bool> ValidateVersionNumberAsync(int questionId, string version);
        
        // Version metadata
        Task<QuestionVersionMetadataDto> GetVersionMetadataAsync(int questionId, string version);
        Task<bool> UpdateVersionMetadataAsync(int questionId, string version, UpdateVersionMetadataDto dto);
        Task<IEnumerable<QuestionVersionDto>> GetVersionsByStatusAsync(int questionId, VersionStatus status);
    }
}
