
using Ikhtibar.Shared.Entities;
using CoreProcessingJobStatus = Ikhtibar.Shared.Entities.ProcessingJobStatus;
using CoreProcessingJobType = Ikhtibar.Shared.Entities.ProcessingJobType;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaProcessingJob entity operations
/// Provides specialized methods for background job queue management
/// </summary>
public interface IMediaProcessingJobRepository : IBaseRepository<MediaProcessingJob>
{
    /// <summary>
    /// Gets jobs for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="statusFilter">Optional status filter</param>
    /// <param name="jobTypeFilter">Optional job type filter</param>
    /// <returns>Collection of processing jobs for the media file</returns>
    Task<IEnumerable<MediaProcessingJob>> GetByMediaFileAsync(int mediaFileId, CoreProcessingJobStatus? statusFilter = null, CoreProcessingJobType? jobTypeFilter = null);

    /// <summary>
    /// Gets jobs by status
    /// </summary>
    /// <param name="status">Job status to filter by</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="orderBy">Optional ordering (default: priority desc, created asc)</param>
    /// <returns>Collection of jobs with the specified status</returns>
    Task<IEnumerable<MediaProcessingJob>> GetByStatusAsync(CoreProcessingJobStatus status, int limit = 100, string? orderBy = null);

    /// <summary>
    /// Gets jobs by type
    /// </summary>
    /// <param name="jobType">Job type to filter by</param>
    /// <param name="statusFilter">Optional status filter</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of jobs of the specified type</returns>
    Task<IEnumerable<MediaProcessingJob>> GetByTypeAsync(CoreProcessingJobType jobType, CoreProcessingJobStatus? statusFilter = null, int limit = 100);

    /// <summary>
    /// Gets the next job to process from the queue
    /// </summary>
    /// <param name="workerName">Name of the worker requesting the job</param>
    /// <param name="supportedJobTypes">Types of jobs this worker can handle</param>
    /// <returns>Next job to process, null if no jobs available</returns>
    Task<MediaProcessingJob?> GetNextJobAsync(string workerName, IEnumerable<CoreProcessingJobType>? supportedJobTypes = null);

    /// <summary>
    /// Gets jobs that are ready for retry
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of jobs ready for retry</returns>
    Task<IEnumerable<MediaProcessingJob>> GetRetryableJobsAsync(int limit = 50);

    /// <summary>
    /// Gets stuck jobs (processing for too long)
    /// </summary>
    /// <param name="stuckThresholdMinutes">Minutes after which a processing job is considered stuck</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of stuck jobs</returns>
    Task<IEnumerable<MediaProcessingJob>> GetStuckJobsAsync(int stuckThresholdMinutes = 30, int limit = 100);

    /// <summary>
    /// Gets failed jobs that have exceeded max retry attempts
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of permanently failed jobs</returns>
    Task<IEnumerable<MediaProcessingJob>> GetPermanentlyFailedJobsAsync(int limit = 100);

    /// <summary>
    /// Gets job statistics by status
    /// </summary>
    /// <returns>Dictionary with status and count of jobs</returns>
    Task<Dictionary<CoreProcessingJobStatus, int>> GetJobStatsByStatusAsync();

    /// <summary>
    /// Gets job statistics by type
    /// </summary>
    /// <returns>Dictionary with job type and count of jobs</returns>
    Task<Dictionary<CoreProcessingJobType, int>> GetJobStatsByTypeAsync();

    /// <summary>
    /// Gets average processing time by job type
    /// </summary>
    /// <param name="days">Number of days to analyze</param>
    /// <returns>Dictionary with job type and average processing time in milliseconds</returns>
    Task<Dictionary<CoreProcessingJobType, double>> GetAverageProcessingTimeAsync(int days = 30);

    /// <summary>
    /// Gets job throughput statistics
    /// </summary>
    /// <param name="hours">Number of hours to analyze</param>
    /// <returns>Jobs completed per hour statistics</returns>
    Task<Dictionary<DateTime, int>> GetThroughputStatsAsync(int hours = 24);

    /// <summary>
    /// Queues a new processing job
    /// </summary>
    /// <param name="mediaFileId">Media file to process</param>
    /// <param name="jobType">Type of processing job</param>
    /// <param name="priority">Job priority (higher = more urgent)</param>
    /// <param name="jobParameters">JSON parameters for the job</param>
    /// <param name="maxAttempts">Maximum retry attempts</param>
    /// <returns>The created processing job</returns>
    Task<MediaProcessingJob> QueueJobAsync(int mediaFileId, CoreProcessingJobType jobType, int priority = 5, string? jobParameters = null, int maxAttempts = 3);

    /// <summary>
    /// Marks a job as started
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="workerName">Name of the worker processing the job</param>
    /// <returns>True if marked successfully</returns>
    Task<bool> MarkJobStartedAsync(int jobId, string workerName);

    /// <summary>
    /// Updates job progress
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="progressPercentage">Progress percentage (0-100)</param>
    /// <param name="currentStage">Optional description of current stage</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateProgressAsync(int jobId, int progressPercentage, string? currentStage = null);

    /// <summary>
    /// Marks a job as completed successfully
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="jobResults">JSON results from the job</param>
    /// <param name="processingDurationMs">Processing time in milliseconds</param>
    /// <returns>True if marked successfully</returns>
    Task<bool> MarkJobCompletedAsync(int jobId, string? jobResults = null, long? processingDurationMs = null);

    /// <summary>
    /// Marks a job as failed
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorStackTrace">Optional stack trace</param>
    /// <param name="retryAfterMinutes">Minutes to wait before retry (null for immediate)</param>
    /// <returns>True if marked successfully</returns>
    Task<bool> MarkJobFailedAsync(int jobId, string errorMessage, string? errorStackTrace = null, int? retryAfterMinutes = null);

    /// <summary>
    /// Cancels a job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="reason">Reason for cancellation</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelJobAsync(int jobId, string? reason = null);

    /// <summary>
    /// Resets stuck jobs back to queued status
    /// </summary>
    /// <param name="stuckThresholdMinutes">Minutes after which a processing job is considered stuck</param>
    /// <returns>Number of jobs reset</returns>
    Task<int> ResetStuckJobsAsync(int stuckThresholdMinutes = 30);

    /// <summary>
    /// Bulk updates job priority
    /// </summary>
    /// <param name="jobIds">Collection of job identifiers</param>
    /// <param name="newPriority">New priority value</param>
    /// <returns>Number of jobs updated</returns>
    Task<int> BulkUpdatePriorityAsync(IEnumerable<int> jobIds, int newPriority);

    /// <summary>
    /// Deletes old completed/failed jobs
    /// </summary>
    /// <param name="olderThanDays">Delete jobs older than this many days</param>
    /// <param name="keepFailed">Whether to keep failed jobs for analysis</param>
    /// <returns>Number of jobs deleted</returns>
    Task<int> CleanupOldJobsAsync(int olderThanDays = 30, bool keepFailed = true);

    /// <summary>
    /// Gets jobs processed by a specific worker
    /// </summary>
    /// <param name="workerName">Worker name</param>
    /// <param name="statusFilter">Optional status filter</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of jobs processed by the worker</returns>
    Task<IEnumerable<MediaProcessingJob>> GetByWorkerAsync(string workerName, CoreProcessingJobStatus? statusFilter = null, int limit = 100);

    /// <summary>
    /// Gets job queue depth by priority
    /// </summary>
    /// <returns>Dictionary with priority levels and number of queued jobs</returns>
    Task<Dictionary<int, int>> GetQueueDepthByPriorityAsync();

    /// <summary>
    /// Checks if there are any pending jobs for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="jobType">Optional job type filter</param>
    /// <returns>True if there are pending jobs</returns>
    Task<bool> HasPendingJobsAsync(int mediaFileId, CoreProcessingJobType? jobType = null);
}
