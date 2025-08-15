using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Events;

/// <summary>
/// Exam-related notification events
/// </summary>
public class ExamEventNotifications
{
    public class ExamReminderEvent
    {
        public int ExamId { get; }
        public int ReminderMinutes { get; }
        public DateTime ExamStartTime { get; }

        public ExamReminderEvent(int examId, int reminderMinutes, DateTime examStartTime)
        {
            ExamId = examId;
            ReminderMinutes = reminderMinutes;
            ExamStartTime = examStartTime;
        }
    }

    public class ExamStartEvent
    {
        public int ExamId { get; }
        public DateTime StartTime { get; }

        public ExamStartEvent(int examId, DateTime startTime)
        {
            ExamId = examId;
            StartTime = startTime;
        }
    }

    public class ExamEndEvent
    {
        public int ExamId { get; }
        public DateTime EndTime { get; }

        public ExamEndEvent(int examId, DateTime endTime)
        {
            ExamId = examId;
            EndTime = endTime;
        }
    }

    public class GradingCompleteEvent
    {
        public int ExamId { get; }
        public int StudentId { get; }
        public decimal Score { get; }
        public string Grade { get; }

        public GradingCompleteEvent(int examId, int studentId, decimal score, string grade)
        {
            ExamId = examId;
            StudentId = studentId;
            Score = score;
            Grade = grade;
        }
    }
}
