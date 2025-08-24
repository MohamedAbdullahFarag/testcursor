using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionReviewService
    {
        Task<IEnumerable<QuestionReviewDto>> GetAllAsync();
        Task<QuestionReviewDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionReviewDto>> GetByQuestionIdAsync(int questionId);
        Task<IEnumerable<QuestionReviewDto>> GetByReviewerIdAsync(int reviewerId);
        Task<IEnumerable<QuestionReviewDto>> GetPendingAsync();
        Task<IEnumerable<QuestionReviewDto>> GetByStatusAsync(ReviewStatus status);

        Task<QuestionReviewDto> CreateAsync(CreateQuestionReviewDto dto);
        Task<QuestionReviewDto?> UpdateAsync(int id, UpdateQuestionReviewDto dto);
        Task<bool> DeleteAsync(int id);

        Task<QuestionReviewDto?> SubmitAsync(int id);
        Task<QuestionReviewDto?> ApproveAsync(int id);
        Task<QuestionReviewDto?> RejectAsync(int id, RejectQuestionReviewDto dto);
        Task<QuestionReviewDto?> RequestRevisionAsync(int id, RequestRevisionDto dto);
        Task<QuestionReviewDto?> AssignReviewerAsync(int id, AssignReviewerDto dto);

        Task<object?> GetWorkflowAsync(int id);
        Task<object?> GetStatisticsAsync();
        Task<IEnumerable<object>?> GetTimelineAsync(int id);
        Task<object?> GetDashboardAsync();
        Task<IEnumerable<object>?> GetReportsAsync(DateTime? startDate, DateTime? endDate);

        Task<object?> AddCommentAsync(int id, AddReviewCommentDto dto);
        Task<IEnumerable<object>?> GetCommentsAsync(int id);
    }
}
