using Ikhtibar.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using MediaType = Ikhtibar.Core.Entities.MediaType;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Media validation service implementation
/// Handles file type, size, and content validation
/// </summary>
public class MediaValidationService : IMediaValidationService
{
    private readonly ILogger<MediaValidationService> _logger;
    private readonly IConfiguration _configuration;

    // File size limits by media type (in bytes)
    private readonly Dictionary<MediaType, long> _maxFileSizes = new()
    {
        { MediaType.Image, 10 * 1024 * 1024 },      // 10MB
        { MediaType.Video, 100 * 1024 * 1024 },     // 100MB
        { MediaType.Audio, 50 * 1024 * 1024 },      // 50MB
        { MediaType.Document, 25 * 1024 * 1024 },   // 25MB
        { MediaType.Archive, 100 * 1024 * 1024 },   // 100MB
        { MediaType.Other, 10 * 1024 * 1024 }       // 10MB default
    };

    // Allowed file extensions by media type
    private readonly Dictionary<MediaType, string[]> _allowedExtensions = new()
    {
        { MediaType.Image, new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp", ".tiff" } },
        { MediaType.Video, new[] { ".mp4", ".webm", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".m4v" } },
        { MediaType.Audio, new[] { ".mp3", ".wav", ".ogg", ".aac", ".flac", ".wma", ".m4a" } },
        { MediaType.Document, new[] { ".pdf", ".doc", ".docx", ".txt", ".rtf", ".odt", ".pages" } },
        { MediaType.Archive, new[] { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2" } },
        { MediaType.Other, new[] { ".json", ".xml", ".csv", ".log" } }
    };

    // Allowed content types by media type
    private readonly Dictionary<MediaType, string[]> _allowedContentTypes = new()
    {
        { MediaType.Image, new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml", "image/bmp", "image/tiff" } },
        { MediaType.Video, new[] { "video/mp4", "video/webm", "video/avi", "video/quicktime", "video/x-msvideo", "video/x-flv", "video/x-matroska" } },
        { MediaType.Audio, new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac", "audio/flac", "audio/x-ms-wma", "audio/x-m4a" } },
        { MediaType.Document, new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain", "application/rtf", "application/vnd.oasis.opendocument.text" } },
        { MediaType.Archive, new[] { "application/zip", "application/x-rar-compressed", "application/x-7z-compressed", "application/x-tar", "application/gzip", "application/x-bzip2" } },
        { MediaType.Other, new[] { "application/json", "application/xml", "text/csv", "text/plain" } }
    };

    public MediaValidationService(ILogger<MediaValidationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto)
    {
        var result = new MediaValidationResult
        {
            IsValid = true,
            FileSizeBytes = file.Length
        };

        try
        {
            // Check if file is null or empty
            if (file == null || file.Length == 0)
            {
                result.IsValid = false;
                result.Errors.Add("File is empty or null");
                return result;
            }

            // Determine media type
            var mediaType = DetermineMediaType(file.ContentType, file.FileName);
            result.DetectedMediaType = mediaType.ToString();

            // Validate file type
            if (!IsAllowedFileType(file.FileName, file.ContentType))
            {
                result.IsValid = false;
                result.Errors.Add($"File type not allowed: {file.ContentType}");
            }

            // Validate file size
            if (!IsFileSizeValid(file.Length, mediaType))
            {
                result.IsValid = false;
                var maxSizeMB = _maxFileSizes[mediaType] / (1024 * 1024);
                result.Errors.Add($"File size exceeds limit: {file.Length / (1024 * 1024)}MB > {maxSizeMB}MB");
            }

            // Validate file integrity
            if (!await ValidateFileIntegrityAsync(file))
            {
                result.IsValid = false;
                result.Errors.Add("File integrity validation failed");
            }

            // Virus scanning (if enabled)
            var virusScanEnabled = _configuration.GetValue<bool>("MediaValidation:EnableVirusScan", false);
            if (virusScanEnabled)
            {
                result.IsVirusScanned = true;
                result.IsVirusFree = await ScanForVirusAsync(file);
                
                if (!result.IsVirusFree)
                {
                    result.IsValid = false;
                    result.Errors.Add("Virus scan failed - file may contain malware");
                }
            }
            else
            {
                result.IsVirusScanned = false;
                result.IsVirusFree = true; // Assume safe if scanning is disabled
            }

            // Add warnings for large files
            if (file.Length > _maxFileSizes[mediaType] * 0.8) // 80% of max size
            {
                result.Warnings.Add("File size is approaching the limit");
            }

            // Add warnings for uncommon file types
            if (mediaType == MediaType.Other)
            {
                result.Warnings.Add("File type is not in the standard categories");
            }

            _logger.LogInformation("File validation completed: {FileName}, Valid: {IsValid}, Type: {MediaType}, Size: {Size} bytes",
                file.FileName, result.IsValid, mediaType, file.Length);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file validation: {FileName}", file.FileName);
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    public bool IsAllowedFileType(string fileName, string contentType)
    {
        var mediaType = DetermineMediaType(contentType, fileName);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = _allowedExtensions[mediaType];
        var allowedContentTypes = _allowedContentTypes[mediaType];

        return allowedExtensions.Contains(extension) && allowedContentTypes.Contains(contentType);
    }

    public bool IsFileSizeValid(long fileSizeBytes, MediaType mediaType)
    {
        return fileSizeBytes <= _maxFileSizes[mediaType];
    }

    public async Task<bool> ScanForVirusAsync(IFormFile file)
    {
        try
        {
            // In a production environment, this would integrate with a virus scanning service
            // For now, we'll simulate a virus scan
            
            // Check file size - very large files might be suspicious
            if (file.Length > 500 * 1024 * 1024) // 500MB
            {
                _logger.LogWarning("Large file detected during virus scan: {FileName}, Size: {Size} bytes", 
                    file.FileName, file.Length);
                // In a real implementation, this would trigger additional scanning
            }

            // Check file extension for suspicious patterns
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var suspiciousExtensions = new[] { ".exe", ".bat", ".cmd", ".scr", ".pif", ".com" };
            if (suspiciousExtensions.Contains(extension))
            {
                _logger.LogWarning("Suspicious file extension detected: {FileName}", file.FileName);
                return false; // Block suspicious file types
            }

            // Simulate scanning delay
            await Task.Delay(100);

            // For now, assume all files are safe
            // In production, this would call an actual virus scanning service
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during virus scan: {FileName}", file.FileName);
            // If virus scanning fails, err on the side of caution
            return false;
        }
    }

    public async Task<bool> ValidateFileIntegrityAsync(IFormFile file)
    {
        try
        {
            // Check if file can be read completely
            using var stream = file.OpenReadStream();
            var buffer = new byte[8192];
            var totalBytesRead = 0L;

            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                totalBytesRead += bytesRead;
            }

            // Verify we read the expected number of bytes
            if (totalBytesRead != file.Length)
            {
                _logger.LogWarning("File integrity check failed: Expected {Expected}, Read {Actual}", 
                    file.Length, totalBytesRead);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file integrity validation: {FileName}", file.FileName);
            return false;
        }
    }

    private MediaType DetermineMediaType(string contentType, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        
        // Check by content type first
        if (contentType.StartsWith("image/"))
            return MediaType.Image;
        if (contentType.StartsWith("video/"))
            return MediaType.Video;
        if (contentType.StartsWith("audio/"))
            return MediaType.Audio;
        if (contentType.StartsWith("application/pdf") || 
            contentType.StartsWith("application/msword") ||
            contentType.StartsWith("application/vnd.openxmlformats"))
            return MediaType.Document;
        if (contentType.StartsWith("application/zip") ||
            contentType.StartsWith("application/x-rar-compressed"))
            return MediaType.Archive;
        
        // Fallback to file extension
        foreach (var kvp in _allowedExtensions)
        {
            if (kvp.Value.Contains(extension))
                return kvp.Key;
        }
        
        return MediaType.Other;
    }
}
