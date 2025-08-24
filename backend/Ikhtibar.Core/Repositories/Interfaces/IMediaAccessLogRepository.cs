

using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaAccessLog entity operations
/// Provides specialized methods for access tracking and analytics
/// </summary>
public interface IMediaAccessLogRepository : IBaseRepository<MediaAccessLog>
{
    /// <summary>1
    /// Gets access logs for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Collection of access logs for the media file</returns>
    Task<IEnumerable<MediaAccessLog>> GetByMediaFileAsync(int mediaFileId, int offset = 0, int limit = 100, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets access logs for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Collection of access logs for the user</returns>
    Task<IEnumerable<MediaAccessLog>> GetByUserAsync(int userId, int offset = 0, int limit = 100, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets access logs by access type
    /// </summary>
    /// <param name="accessType">Type of access to filter by</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Collection of access logs for the specified type</returns>
    Task<IEnumerable<MediaAccessLog>> GetByAccessTypeAsync(AccessType accessType, int offset = 0, int limit = 100, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets access logs by IP address
    /// </summary>
    /// <param name="ipAddress">IP address to search for</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of access logs from the specified IP</returns>
    Task<IEnumerable<MediaAccessLog>> GetByIpAddressAsync(string ipAddress, int limit = 100);

    /// <summary>
    /// Gets failed access attempts
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of failed access logs</returns>
    Task<IEnumerable<MediaAccessLog>> GetFailedAccessAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 100);

    /// <summary>
    /// Gets access statistics for a date range
    /// </summary>
    /// <param name="startDate">Start date for statistics</param>
    /// <param name="endDate">End date for statistics</param>
    /// <returns>Access statistics including total accesses, unique users, etc.</returns>
    Task<dynamic> GetAccessStatsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets daily access counts for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="days">Number of days to look back</param>
    /// <returns>Dictionary with dates and access counts</returns>
    Task<Dictionary<DateTime, int>> GetDailyAccessCountsAsync(int mediaFileId, int days = 30);

    /// <summary>
    /// Gets hourly access pattern for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="days">Number of days to analyze</param>
    /// <returns>Dictionary with hours (0-23) and access counts</returns>
    Task<Dictionary<int, int>> GetHourlyAccessPatternAsync(int mediaFileId, int days = 7);

    /// <summary>
    /// Gets most accessed media files
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="limit">Maximum number of files to return</param>
    /// <returns>Media files ordered by access count</returns>
    Task<IEnumerable<dynamic>> GetMostAccessedFilesAsync(DateTime startDate, DateTime endDate, int limit = 10);

    /// <summary>
    /// Gets access statistics by media type
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <returns>Dictionary with media types and access counts</returns>
    Task<Dictionary<string, int>> GetAccessStatsByMediaTypeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets access statistics by access type
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <returns>Dictionary with access types and counts</returns>
    Task<Dictionary<AccessType, int>> GetAccessStatsByTypeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets unique user count for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Number of unique users who accessed the file</returns>
    Task<int> GetUniqueUserCountAsync(int mediaFileId, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets bandwidth usage statistics
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <returns>Total bytes transferred in the period</returns>
    Task<long> GetBandwidthUsageAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets top referrers for media access
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="limit">Maximum number of referrers to return</param>
    /// <returns>Dictionary with referrers and access counts</returns>
    Task<Dictionary<string, int>> GetTopReferrersAsync(DateTime startDate, DateTime endDate, int limit = 10);

    /// <summary>
    /// Gets access logs for security analysis (suspicious activity)
    /// </summary>
    /// <param name="minAccessCount">Minimum access count per IP to be considered suspicious</param>
    /// <param name="timeWindowHours">Time window in hours to analyze</param>
    /// <returns>Collection of potentially suspicious access patterns</returns>
    Task<IEnumerable<dynamic>> GetSuspiciousAccessAsync(int minAccessCount = 100, int timeWindowHours = 1);

    /// <summary>
    /// Logs a media access event
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="userId">User identifier (if authenticated)</param>
    /// <param name="accessType">Type of access</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <param name="referrer">Referrer URL</param>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="success">Whether the access was successful</param>
    /// <param name="bytesTransferred">Bytes transferred</param>
    /// <param name="duration">Access duration</param>
    /// <param name="errorMessage">Error message if access failed</param>
    /// <returns>The created access log entry</returns>
    Task<MediaAccessLog> LogAccessAsync(int mediaFileId, int? userId, AccessType accessType, string? ipAddress, string? userAgent, string? referrer, string? sessionId, bool success = true, long? bytesTransferred = null, TimeSpan? duration = null, string? errorMessage = null);

    /// <summary>
    /// Bulk inserts access logs (for batch processing)
    /// </summary>
    /// <param name="accessLogs">Collection of access logs to insert</param>
    /// <returns>Number of logs inserted</returns>
    Task<int> BulkInsertAsync(IEnumerable<MediaAccessLog> accessLogs);

    /// <summary>
    /// Deletes old access logs (for data retention)
    /// </summary>
    /// <param name="olderThanDays">Delete logs older than this many days</param>
    /// <returns>Number of logs deleted</returns>
    Task<int> DeleteOldLogsAsync(int olderThanDays = 365);

    /// <summary>
    /// Archives old access logs to a different table/storage
    /// </summary>
    /// <param name="olderThanDays">Archive logs older than this many days</param>
    /// <returns>Number of logs archived</returns>
    Task<int> ArchiveOldLogsAsync(int olderThanDays = 90);
}
