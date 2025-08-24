using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionCreationWorkflowService
    {
        Task<IEnumerable<QuestionCreationWorkflowDto>> GetAllAsync();
        Task<QuestionCreationWorkflowDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionCreationWorkflowDto>> GetByCreatorIdAsync(int creatorId);
        Task<IEnumerable<QuestionCreationWorkflowDto>> GetByStatusAsync(WorkflowStatus status);
        Task<IEnumerable<QuestionCreationWorkflowDto>> GetActiveAsync();

        Task<QuestionCreationWorkflowDto> CreateAsync(CreateQuestionCreationWorkflowDto dto);
        Task<QuestionCreationWorkflowDto?> UpdateAsync(int id, UpdateQuestionCreationWorkflowDto dto);
    Task<bool> DeleteAsync(int id);

    Task<QuestionCreationWorkflowDto?> StartAsync(int id);
    Task<QuestionCreationWorkflowDto?> PauseAsync(int id);
    Task<QuestionCreationWorkflowDto?> ResumeAsync(int id);
    Task<QuestionCreationWorkflowDto?> CompleteAsync(int id);
    Task<QuestionCreationWorkflowDto?> CancelAsync(int id, CancelWorkflowDto dto);
    Task<QuestionCreationWorkflowDto?> MoveToNextStepAsync(int id);
    Task<QuestionCreationWorkflowDto?> MoveToPreviousStepAsync(int id);

    Task<IEnumerable<object>?> GetStepsAsync(int id);
        Task<object?> GetCurrentStepAsync(int id);
        Task<object?> GetProgressAsync(int id);
        Task<IEnumerable<object>?> GetTimelineAsync(int id);
        Task<object?> GetStatisticsAsync();
        Task<object?> GetDashboardAsync();
        Task<IEnumerable<object>?> GetReportsAsync(DateTime? startDate, DateTime? endDate);
    }
}
