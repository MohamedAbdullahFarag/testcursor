using System;

namespace Ikhtibar.Shared.DTOs
{
    /// <summary>
    /// Data Transfer Object for scheduled background jobs related to notifications.
    /// </summary>
    public class ScheduledJobDto
    {
        /// <summary>
        /// Unique identifier for the scheduled job.
        /// </summary>
        public string JobId { get; set; } = string.Empty;

        /// <summary>
        /// Notification ID associated with this job.
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Scheduled execution time for the job.
        /// </summary>
        public DateTime ScheduledTime { get; set; }

        /// <summary>
        /// Type of the scheduled job (e.g., "OneTime", "Recurring").
        /// </summary>
        public string JobType { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the job (e.g., "Scheduled", "Completed").
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
