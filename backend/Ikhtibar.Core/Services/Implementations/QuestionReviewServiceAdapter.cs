using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Implementations
{
    public class QuestionReviewServiceAdapter : IQuestionReviewService
    {
        public Task<QuestionReviewDto> CreateAsync(CreateQuestionReviewDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<object?> AddCommentAsync(int id, AddReviewCommentDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionReviewDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetCommentsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetDashboardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetStatisticsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetTimelineAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetWorkflowAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>?> GetReportsAsync(DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionReviewDto>> GetByQuestionIdAsync(int questionId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionReviewDto>> GetByReviewerIdAsync(int reviewerId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionReviewDto>> GetByStatusAsync(ReviewStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QuestionReviewDto>> GetPendingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> ApproveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> AssignReviewerAsync(int id, AssignReviewerDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> RejectAsync(int id, RejectQuestionReviewDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> RequestRevisionAsync(int id, RequestRevisionDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> SubmitAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionReviewDto?> UpdateAsync(int id, UpdateQuestionReviewDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
