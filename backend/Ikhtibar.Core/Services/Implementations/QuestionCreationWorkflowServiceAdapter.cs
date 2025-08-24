using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Implementations
{
    public class QuestionCreationWorkflowServiceAdapter : IQuestionCreationWorkflowService
    {
        public Task<QuestionCreationWorkflowDto> CreateAsync(CreateQuestionCreationWorkflowDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionCreationWorkflowDto>> GetActiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionCreationWorkflowDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionCreationWorkflowDto>> GetByCreatorIdAsync(int creatorId)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionCreationWorkflowDto>> GetByStatusAsync(WorkflowStatus status)
        {
            throw new NotImplementedException();
        }

        

        public Task<object?> GetCurrentStepAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetDashboardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetReportsAsync(DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetStepsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetTimelineAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetProgressAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetStatisticsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> MoveToNextStepAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> MoveToPreviousStepAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> PauseAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> ResumeAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> StartAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> CompleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> CancelAsync(int id, CancelWorkflowDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionCreationWorkflowDto?> UpdateAsync(int id, UpdateQuestionCreationWorkflowDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
