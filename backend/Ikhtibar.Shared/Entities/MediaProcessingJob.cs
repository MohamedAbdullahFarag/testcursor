using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Media processing job entity for tracking background processing tasks
/// Manages thumbnail generation, format conversion, optimization, etc.
/// </summary>
[Table("MediaProcessingJobs")]
public class MediaProcessingJob : BaseEntity
{
    /// <summary>
    /// Media file being processed
    /// </summary>
    [Required]
    public int MediaFileId { get; set; }

    /// <summary>
    /// Type of processing job
    /// </summary>
    [Required]
    public ProcessingJobType JobType { get; set; }

    /// <summary>
    /// Current status of the job
    /// </summary>
    [Required]
    public ProcessingJobStatus Status { get; set; } = ProcessingJobStatus.Queued;

    /// <summary>
    /// Job priority (higher number = higher priority)
    /// </summary>
    [Required]
    public int Priority { get; set; } = 5;

    /// <summary>
    /// Number of processing attempts
    /// </summary>
    [Required]
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    [Required]
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// When the job started processing
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the job completed (success or failure)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Next retry time if job failed
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    public long? ProcessingDurationMs { get; set; }

    /// <summary>
    /// Percentage completion (0-100)
    /// </summary>
    [Range(0, 100)]
    public int ProgressPercentage { get; set; } = 0;

    /// <summary>
    /// Current processing stage description
    /// </summary>
    [StringLength(255)]
    public string? CurrentStage { get; set; }

    /// <summary>
    /// Job parameters and configuration (JSON)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? JobParameters { get; set; }

    /// <summary>
    /// Job results and output information (JSON)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? JobResults { get; set; }

    /// <summary>
    /// Error message if job failed
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Stack trace if job failed (for debugging)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// Worker/server that processed this job
    /// </summary>
    [StringLength(100)]
    public string? ProcessedBy { get; set; }

    /// <summary>
    /// Additional job metadata
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    // Navigation Properties

    /// <summary>
    /// Media file being processed
    /// </summary>
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile MediaFile { get; set; } = null!;
}

/// <summary>
/// Types of media processing jobs
/// </summary>
public enum ProcessingJobType
{
    /// <summary>
    /// Generate thumbnails for images/videos
    /// </summary>
    ThumbnailGeneration = 1,

    /// <summary>
    /// Convert file format
    /// </summary>
    FormatConversion = 2,

    /// <summary>
    /// Optimize file size/quality
    /// </summary>
    Optimization = 3,

    /// <summary>
    /// Extract metadata from file
    /// </summary>
    MetadataExtraction = 4,

    /// <summary>
    /// Scan file for viruses/malware
    /// </summary>
    VirusScan = 5,

    /// <summary>
    /// Validate file integrity
    /// </summary>
    IntegrityCheck = 6,

    /// <summary>
    /// Generate video preview/clips
    /// </summary>
    VideoPreview = 7,

    /// <summary>
    /// Extract text content (OCR/document parsing)
    /// </summary>
    TextExtraction = 8,

    /// <summary>
    /// Apply watermark to media
    /// </summary>
    Watermarking = 9,

    /// <summary>
    /// Archive old media to cold storage
    /// </summary>
    Archival = 10
}

/// <summary>
/// Processing job status states
/// </summary>
public enum ProcessingJobStatus
{
    /// <summary>
    /// Job is queued for processing
    /// </summary>
    Queued = 1,

    /// <summary>
    /// Job is currently being processed
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Job completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Job failed and will be retried
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Job failed permanently (max retries exceeded)
    /// </summary>
    FailedPermanently = 5,

    /// <summary>
    /// Job was cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Job is paused/suspended
    /// </summary>
    Paused = 7,

    /// <summary>
    /// Job is waiting for retry
    /// </summary>
    WaitingRetry = 8
}
