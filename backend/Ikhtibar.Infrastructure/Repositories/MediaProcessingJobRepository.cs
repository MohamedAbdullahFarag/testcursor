using Dapper;
using Microsoft.Extensions.Logging;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using CoreProcessingJobStatus = Ikhtibar.Shared.Entities.ProcessingJobStatus;
using CoreProcessingJobType = Ikhtibar.Shared.Entities.ProcessingJobType;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MediaProcessingJob entity operations
/// </summary>
public class MediaProcessingJobRepository : BaseRepository<MediaProcessingJob>, IMediaProcessingJobRepository
{
    private new readonly ILogger<MediaProcessingJobRepository> _logger;

    public MediaProcessingJobRepository(IDbConnectionFactory connectionFactory, ILogger<MediaProcessingJobRepository> logger)
        : base(connectionFactory, logger, "MediaProcessingJobs", "Id")
    {
        _logger = logger;
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetByMediaFileAsync(int mediaFileId, CoreProcessingJobStatus? statusFilter = null, CoreProcessingJobType? jobTypeFilter = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT * FROM MediaProcessingJobs WHERE MediaFileId = @MediaFileId AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("MediaFileId", mediaFileId);
            
            if (statusFilter.HasValue)
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", (int)statusFilter.Value);
            }
            
            if (jobTypeFilter.HasValue)
            {
                sql += " AND JobType = @JobType";
                parameters.Add("JobType", (int)jobTypeFilter.Value);
            }
            
            sql += " ORDER BY CreatedAt DESC";
            
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, parameters);
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving processing jobs for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetByStatusAsync(CoreProcessingJobStatus status, int limit = 100, string? orderBy = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var orderClause = string.IsNullOrEmpty(orderBy) ? "ORDER BY Priority DESC, CreatedAt ASC" : $"ORDER BY {orderBy}";
            var sql = $"SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE Status = @Status AND IsDeleted = 0 {orderClause}";
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, new { Status = (int)status, Limit = limit });
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving processing jobs by status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetByTypeAsync(CoreProcessingJobType jobType, CoreProcessingJobStatus? statusFilter = null, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE JobType = @JobType AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("JobType", (int)jobType);
            parameters.Add("Limit", limit);
            
            if (statusFilter.HasValue)
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", (int)statusFilter.Value);
            }
            
            sql += " ORDER BY Priority DESC, CreatedAt ASC";
            
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, parameters);
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving processing jobs by type {JobType}", jobType);
            throw;
        }
    }

    public async Task<MediaProcessingJob?> GetNextJobAsync(string workerName, IEnumerable<CoreProcessingJobType>? supportedJobTypes = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT TOP 1 * FROM MediaProcessingJobs WHERE Status = @QueuedStatus AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("QueuedStatus", (int)CoreProcessingJobStatus.Queued);
            
            if (supportedJobTypes != null && supportedJobTypes.Any())
            {
                var jobTypeList = supportedJobTypes.Select(jt => (int)jt).ToList();
                sql += " AND JobType IN @JobTypes";
                parameters.Add("JobTypes", jobTypeList);
            }
            
            sql += " AND (NextRetryAt IS NULL OR NextRetryAt <= @CurrentTime) ORDER BY Priority DESC, CreatedAt ASC";
            parameters.Add("CurrentTime", DateTime.UtcNow);
            
            var job = await connection.QueryFirstOrDefaultAsync<MediaProcessingJob>(sql, parameters);
            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error getting next job for worker {WorkerName}", workerName);
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetRetryableJobsAsync(int limit = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE Status = @FailedStatus AND AttemptCount < MaxAttempts AND IsDeleted = 0 ORDER BY Priority DESC, CreatedAt ASC";
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, new { FailedStatus = (int)CoreProcessingJobStatus.Failed, Limit = limit });
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving retryable jobs");
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetStuckJobsAsync(int stuckThresholdMinutes = 30, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var timeoutThreshold = DateTime.UtcNow.AddMinutes(-stuckThresholdMinutes);
            var sql = "SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE Status = @ProcessingStatus AND StartedAt <= @TimeoutThreshold AND IsDeleted = 0 ORDER BY StartedAt ASC";
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, new 
            { 
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing, 
                TimeoutThreshold = timeoutThreshold, 
                Limit = limit 
            });
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving stuck jobs");
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetPermanentlyFailedJobsAsync(int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE Status = @FailedStatus AND AttemptCount >= MaxAttempts AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, new { FailedStatus = (int)CoreProcessingJobStatus.Failed, Limit = limit });
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving permanently failed jobs");
            throw;
        }
    }

    public async Task<Dictionary<CoreProcessingJobStatus, int>> GetJobStatsByStatusAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Status, COUNT(*) as JobCount FROM MediaProcessingJobs WHERE IsDeleted = 0 GROUP BY Status";
            var results = await connection.QueryAsync<(int Status, int JobCount)>(sql);
            var statistics = results.ToDictionary(r => (CoreProcessingJobStatus)r.Status, r => r.JobCount);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving job statistics by status");
            throw;
        }
    }

    public async Task<Dictionary<CoreProcessingJobType, int>> GetJobStatsByTypeAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT JobType, COUNT(*) as JobCount FROM MediaProcessingJobs WHERE IsDeleted = 0 GROUP BY JobType";
            var results = await connection.QueryAsync<(int JobType, int JobCount)>(sql);
            var statistics = results.ToDictionary(r => (CoreProcessingJobType)r.JobType, r => r.JobCount);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving job statistics by type");
            throw;
        }
    }

    public async Task<Dictionary<CoreProcessingJobType, double>> GetAverageProcessingTimeAsync(int days = 30)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var sql = "SELECT JobType, AVG(CAST(ProcessingTimeMs as float)) as AvgTime FROM MediaProcessingJobs WHERE Status = @CompletedStatus AND ProcessingTimeMs IS NOT NULL AND ProcessingTimeMs > 0 AND CreatedAt >= @CutoffDate AND IsDeleted = 0 GROUP BY JobType";
            var results = await connection.QueryAsync<(int JobType, double AvgTime)>(sql, new 
            { 
                CompletedStatus = (int)CoreProcessingJobStatus.Completed, 
                CutoffDate = cutoffDate 
            });
            var statistics = results.ToDictionary(r => (CoreProcessingJobType)r.JobType, r => r.AvgTime);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error calculating average processing time");
            throw;
        }
    }

    public async Task<Dictionary<DateTime, int>> GetThroughputStatsAsync(int hours = 24)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var cutoffDate = DateTime.UtcNow.AddHours(-hours);
            var sql = "SELECT DATEADD(HOUR, DATEDIFF(HOUR, 0, CompletedAt), 0) as HourBucket, COUNT(*) as CompletedJobs FROM MediaProcessingJobs WHERE Status = @CompletedStatus AND CompletedAt >= @CutoffDate AND IsDeleted = 0 GROUP BY DATEADD(HOUR, DATEDIFF(HOUR, 0, CompletedAt), 0) ORDER BY HourBucket";
            var results = await connection.QueryAsync<(DateTime HourBucket, int CompletedJobs)>(sql, new 
            { 
                CompletedStatus = (int)CoreProcessingJobStatus.Completed, 
                CutoffDate = cutoffDate 
            });
            var statistics = results.ToDictionary(r => r.HourBucket, r => r.CompletedJobs);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving throughput statistics");
            throw;
        }
    }

    public async Task<MediaProcessingJob> QueueJobAsync(int mediaFileId, CoreProcessingJobType jobType, int priority = 5, string? jobParameters = null, int maxAttempts = 3)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var job = new MediaProcessingJob
            {
                MediaFileId = mediaFileId,
                JobType = jobType,
                Status = CoreProcessingJobStatus.Queued,
                Priority = priority,
                AttemptCount = 0,
                MaxAttempts = maxAttempts,
                ProgressPercentage = 0,
                JobParameters = jobParameters,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            
            var sql = "INSERT INTO MediaProcessingJobs (MediaFileId, JobType, Status, Priority, JobParameters, MaxAttempts, AttemptCount, ProgressPercentage, CreatedAt, ModifiedAt, IsDeleted) VALUES (@MediaFileId, @JobType, @Status, @Priority, @JobParameters, @MaxAttempts, @AttemptCount, @ProgressPercentage, @CreatedAt, @ModifiedAt, @IsDeleted)";
            
            await connection.ExecuteAsync(sql, new
            {
                job.MediaFileId,
                JobType = (int)job.JobType,
                Status = (int)job.Status,
                job.Priority,
                job.JobParameters,
                job.MaxAttempts,
                job.AttemptCount,
                job.ProgressPercentage,
                job.CreatedAt,
                job.ModifiedAt,
                job.IsDeleted
            });
            
            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error queueing job for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<bool> MarkJobStartedAsync(int jobId, string workerName)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaProcessingJobs SET Status = @Status, StartedAt = @StartedAt, ProcessedBy = @WorkerName, ProgressPercentage = 0, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND Status = @QueuedStatus AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                JobId = jobId,
                Status = (int)CoreProcessingJobStatus.Processing,
                QueuedStatus = (int)CoreProcessingJobStatus.Queued,
                StartedAt = DateTime.UtcNow,
                WorkerName = workerName,
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error marking job {JobId} as started", jobId);
            throw;
        }
    }

    public async Task<bool> UpdateProgressAsync(int jobId, int progressPercentage, string? currentStage = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaProcessingJobs SET ProgressPercentage = @ProgressPercentage, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND Status = @ProcessingStatus AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                JobId = jobId,
                ProgressPercentage = Math.Clamp(progressPercentage, 0, 100),
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing,
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error updating progress for job {JobId}", jobId);
            throw;
        }
    }

    public async Task<bool> MarkJobCompletedAsync(int jobId, string? jobResults = null, long? processingDurationMs = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaProcessingJobs SET Status = @Status, CompletedAt = @CompletedAt, ProgressPercentage = 100, ErrorMessage = @JobResults, ProcessingTimeMs = @ProcessingDurationMs, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND Status = @ProcessingStatus AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                JobId = jobId,
                Status = (int)CoreProcessingJobStatus.Completed,
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing,
                CompletedAt = DateTime.UtcNow,
                JobResults = jobResults,
                ProcessingDurationMs = processingDurationMs,
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error marking job {JobId} as completed", jobId);
            throw;
        }
    }

    public async Task<bool> MarkJobFailedAsync(int jobId, string errorMessage, string? errorStackTrace = null, int? retryAfterMinutes = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var errorDetails = errorMessage;
            if (!string.IsNullOrEmpty(errorStackTrace))
            {
                errorDetails += Environment.NewLine + "Stack Trace:" + Environment.NewLine + errorStackTrace;
            }
            
            var nextScheduleTime = retryAfterMinutes.HasValue ? DateTime.UtcNow.AddMinutes(retryAfterMinutes.Value) : (DateTime?)null;
            var sql = "UPDATE MediaProcessingJobs SET Status = @Status, CompletedAt = @CompletedAt, ErrorMessage = @ErrorMessage, AttemptCount = AttemptCount + 1, NextRetryAt = @NextScheduleTime, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND Status = @ProcessingStatus AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                JobId = jobId,
                Status = (int)CoreProcessingJobStatus.Failed,
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing,
                CompletedAt = DateTime.UtcNow,
                ErrorMessage = errorDetails,
                NextScheduleTime = nextScheduleTime,
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error marking job {JobId} as failed", jobId);
            throw;
        }
    }

    public async Task<bool> CancelJobAsync(int jobId, string? reason = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaProcessingJobs SET Status = @Status, CompletedAt = @CompletedAt, ErrorMessage = @Reason, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND Status IN (@QueuedStatus, @ProcessingStatus) AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                JobId = jobId,
                Status = (int)CoreProcessingJobStatus.Cancelled,
                QueuedStatus = (int)CoreProcessingJobStatus.Queued,
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing,
                CompletedAt = DateTime.UtcNow,
                Reason = reason ?? "Job cancelled by user",
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error cancelling job {JobId}", jobId);
            throw;
        }
    }

    public async Task<int> ResetStuckJobsAsync(int stuckThresholdMinutes = 30)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var timeoutThreshold = DateTime.UtcNow.AddMinutes(-stuckThresholdMinutes);
            var sql = "UPDATE MediaProcessingJobs SET Status = @QueuedStatus, StartedAt = NULL, ProgressPercentage = 0, ProcessedBy = NULL, ErrorMessage = @ErrorMessage, ModifiedAt = @ModifiedAt WHERE Status = @ProcessingStatus AND StartedAt <= @TimeoutThreshold AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                QueuedStatus = (int)CoreProcessingJobStatus.Queued,
                ProcessingStatus = (int)CoreProcessingJobStatus.Processing,
                TimeoutThreshold = timeoutThreshold,
                ErrorMessage = $"Job reset due to timeout (exceeded {stuckThresholdMinutes} minutes)",
                ModifiedAt = DateTime.UtcNow
            });
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error resetting stuck jobs");
            throw;
        }
    }

    public async Task<int> BulkUpdatePriorityAsync(IEnumerable<int> jobIds, int newPriority)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var updatedCount = 0;
                foreach (var jobId in jobIds)
                {
                    var sql = "UPDATE MediaProcessingJobs SET Priority = @Priority, ModifiedAt = @ModifiedAt WHERE Id = @JobId AND IsDeleted = 0";
                    var rowsAffected = await connection.ExecuteAsync(sql, new
                    {
                        JobId = jobId,
                        Priority = newPriority,
                        ModifiedAt = DateTime.UtcNow
                    }, transaction);
                    updatedCount += rowsAffected;
                }
                
                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error updating job priorities in bulk");
            throw;
        }
    }

    public async Task<int> CleanupOldJobsAsync(int olderThanDays = 30, bool keepFailed = true)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var statusList = new List<int> { (int)CoreProcessingJobStatus.Completed, (int)CoreProcessingJobStatus.Cancelled };
            
            if (!keepFailed)
            {
                statusList.Add((int)CoreProcessingJobStatus.Failed);
            }
            
            var sql = $"DELETE FROM MediaProcessingJobs WHERE Status IN ({string.Join(",", statusList)}) AND CompletedAt <= @CutoffDate AND IsDeleted = 0";
            var rowsAffected = await connection.ExecuteAsync(sql, new { CutoffDate = cutoffDate });
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error cleaning up old jobs");
            throw;
        }
    }

    public async Task<IEnumerable<MediaProcessingJob>> GetByWorkerAsync(string workerName, CoreProcessingJobStatus? statusFilter = null, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT TOP (@Limit) * FROM MediaProcessingJobs WHERE ProcessedBy = @WorkerName AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("WorkerName", workerName);
            parameters.Add("Limit", limit);
            
            if (statusFilter.HasValue)
            {
                sql += " AND Status = @StatusFilter";
                parameters.Add("StatusFilter", (int)statusFilter.Value);
            }
            
            sql += " ORDER BY StartedAt DESC";
            
            var jobs = await connection.QueryAsync<MediaProcessingJob>(sql, parameters);
            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error getting jobs processed by worker {WorkerName}", workerName);
            throw;
        }
    }

    public async Task<Dictionary<int, int>> GetQueueDepthByPriorityAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Priority, COUNT(*) as JobCount FROM MediaProcessingJobs WHERE Status = @QueuedStatus AND IsDeleted = 0 GROUP BY Priority ORDER BY Priority DESC";
            var results = await connection.QueryAsync<(int Priority, int JobCount)>(sql, new { QueuedStatus = (int)CoreProcessingJobStatus.Queued });
            var stats = results.ToDictionary(r => r.Priority, r => r.JobCount);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error getting job queue depth by priority");
            throw;
        }
    }

    public async Task<bool> HasPendingJobsAsync(int mediaFileId, CoreProcessingJobType? jobType = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(*) FROM MediaProcessingJobs WHERE MediaFileId = @MediaFileId AND Status IN (@QueuedStatus, @ProcessingStatus) AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("MediaFileId", mediaFileId);
            parameters.Add("QueuedStatus", (int)CoreProcessingJobStatus.Queued);
            parameters.Add("ProcessingStatus", (int)CoreProcessingJobStatus.Processing);
            
            if (jobType.HasValue)
            {
                sql += " AND JobType = @JobType";
                parameters.Add("JobType", (int)jobType.Value);
            }
            
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error checking for pending jobs for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }
}
