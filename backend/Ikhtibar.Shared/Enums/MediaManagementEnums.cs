namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Enumeration for media file types supported by the system
/// </summary>
public enum MediaFileType
{
    /// <summary>
    /// Unknown file type
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Image files (JPEG, PNG, GIF, WebP, SVG, etc.)
    /// </summary>
    Image = 1,
    
    /// <summary>
    /// Video files (MP4, WebM, AVI, MOV, etc.)
    /// </summary>
    Video = 2,
    
    /// <summary>
    /// Audio files (MP3, WAV, OGG, AAC, etc.)
    /// </summary>
    Audio = 3,
    
    /// <summary>
    /// Document files (PDF, DOC, DOCX, TXT, etc.)
    /// </summary>
    Document = 4,
    
    /// <summary>
    /// Presentation files (PPT, PPTX, ODP, etc.)
    /// </summary>
    Presentation = 5,
    
    /// <summary>
    /// Spreadsheet files (XLS, XLSX, ODS, CSV, etc.)
    /// </summary>
    Spreadsheet = 6,
    
    /// <summary>
    /// Archive files (ZIP, RAR, 7Z, TAR, etc.)
    /// </summary>
    Archive = 7,
    
    /// <summary>
    /// Code files (JS, CSS, HTML, JSON, XML, etc.)
    /// </summary>
    Code = 8,
    
    /// <summary>
    /// Font files (TTF, OTF, WOFF, WOFF2, etc.)
    /// </summary>
    Font = 9,
    
    /// <summary>
    /// 3D model files (OBJ, FBX, GLTF, etc.)
    /// </summary>
    Model3D = 10,
    
    /// <summary>
    /// Other file types not covered by specific categories
    /// </summary>
    Other = 99
}

/// <summary>
/// Enumeration for media file processing and availability status
/// </summary>
public enum MediaFileStatus
{
    /// <summary>
    /// File is being uploaded
    /// </summary>
    Uploading = 0,
    
    /// <summary>
    /// File is being processed (thumbnails, metadata extraction, etc.)
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// File is available and ready for use
    /// </summary>
    Available = 2,
    
    /// <summary>
    /// File processing failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// File is quarantined (security scan, virus detected, etc.)
    /// </summary>
    Quarantined = 4,
    
    /// <summary>
    /// File is archived (moved to cold storage)
    /// </summary>
    Archived = 5,
    
    /// <summary>
    /// File is marked for deletion
    /// </summary>
    MarkedForDeletion = 6,
    
    /// <summary>
    /// File is temporarily unavailable
    /// </summary>
    Unavailable = 7
}

/// <summary>
/// Enumeration for media access log actions
/// </summary>
public enum MediaAccessAction
{
    /// <summary>
    /// File was viewed/displayed
    /// </summary>
    View = 1,
    
    /// <summary>
    /// File was downloaded
    /// </summary>
    Download = 2,
    
    /// <summary>
    /// File was streamed (for audio/video)
    /// </summary>
    Stream = 3,
    
    /// <summary>
    /// File thumbnail was generated
    /// </summary>
    ThumbnailGeneration = 4,
    
    /// <summary>
    /// File metadata was accessed
    /// </summary>
    MetadataAccess = 5,
    
    /// <summary>
    /// File was shared with others
    /// </summary>
    Share = 6,
    
    /// <summary>
    /// File was embedded in content
    /// </summary>
    Embed = 7,
    
    /// <summary>
    /// File was previewed (quick view)
    /// </summary>
    Preview = 8
}

/// <summary>
/// Enumeration for media collection types
/// </summary>
public enum MediaCollectionType
{
    /// <summary>
    /// User-created playlist
    /// </summary>
    Playlist = 1,
    
    /// <summary>
    /// Album for images
    /// </summary>
    Album = 2,
    
    /// <summary>
    /// Gallery collection
    /// </summary>
    Gallery = 3,
    
    /// <summary>
    /// Course materials collection
    /// </summary>
    Course = 4,
    
    /// <summary>
    /// Question bank media collection
    /// </summary>
    QuestionBank = 5,
    
    /// <summary>
    /// Resource library collection
    /// </summary>
    Library = 6,
    
    /// <summary>
    /// Temporary collection
    /// </summary>
    Temporary = 7,
    
    /// <summary>
    /// Favorites collection
    /// </summary>
    Favorites = 8
}

/// <summary>
/// Standard thumbnail size categories
/// </summary>
public enum ThumbnailSize
{
    /// <summary>
    /// Small thumbnail (typically 64x64 or 100x100)
    /// </summary>
    Small = 1,

    /// <summary>
    /// Medium thumbnail (typically 150x150 or 200x200)
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Large thumbnail (typically 300x300 or 400x400)
    /// </summary>
    Large = 3,

    /// <summary>
    /// Extra large thumbnail (typically 600x600 or 800x800)
    /// </summary>
    ExtraLarge = 4
}

/// <summary>
/// Processing job types for media files
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
