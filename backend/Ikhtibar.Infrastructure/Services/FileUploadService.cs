using Ikhtibar.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using MediaType = Ikhtibar.Shared.Entities.MediaType;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// File upload service implementation
/// Handles file validation, hash calculation, and filename generation
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
    private readonly string[] _allowedVideoExtensions = { ".mp4", ".webm", ".avi", ".mov", ".wmv", ".flv", ".mkv" };
    private readonly string[] _allowedAudioExtensions = { ".mp3", ".wav", ".ogg", ".aac", ".flac", ".wma" };
    private readonly string[] _allowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".rtf", ".odt" };
    private readonly string[] _allowedArchiveExtensions = { ".zip", ".rar", ".7z", ".tar", ".gz" };

    public FileUploadService(ILogger<FileUploadService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CalculateFileHashAsync(IFormFile file)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating file hash for {FileName}", file.FileName);
            throw new InvalidOperationException("Failed to calculate file hash", ex);
        }
    }

    public async Task<string> GenerateUniqueFileNameAsync(string originalFileName)
    {
        try
        {
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            
            // Generate a unique identifier
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            
            // Create a safe filename
            var safeName = string.Join("_", 
                nameWithoutExtension.Split(Path.GetInvalidFileNameChars())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Take(3)); // Limit to first 3 parts
            
            return $"{safeName}_{timestamp}_{uniqueId}{extension}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating unique filename for {OriginalFileName}", originalFileName);
            // Fallback to int-based filename
            var extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{extension}";
        }
    }

    public Shared.Enums.MediaType DetermineMediaType(string contentType, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        
        // Check by content type first
        if (contentType.StartsWith("image/"))
            return Shared.Enums.MediaType.Image;
        if (contentType.StartsWith("video/"))
            return Shared.Enums.MediaType.Video;
        if (contentType.StartsWith("audio/"))
            return Shared.Enums.MediaType.Audio;
        if (contentType.StartsWith("application/pdf") || 
            contentType.StartsWith("application/msword") ||
            contentType.StartsWith("application/vnd.openxmlformats"))
            return Shared.Enums.MediaType.Document;
        if (contentType.StartsWith("application/zip") ||
            contentType.StartsWith("application/x-rar-compressed"))
            return Shared.Enums.MediaType.Document;
        
        // Fallback to file extension
        if (_allowedImageExtensions.Contains(extension))
            return Shared.Enums.MediaType.Image;
        if (_allowedVideoExtensions.Contains(extension))
            return Shared.Enums.MediaType.Video;
        if (_allowedAudioExtensions.Contains(extension))
            return Shared.Enums.MediaType.Audio;
        if (_allowedDocumentExtensions.Contains(extension))
            return Shared.Enums.MediaType.Document;
        
        return Shared.Enums.MediaType.Document;
    }

    public async Task<bool> ValidateFileConstraintsAsync(IFormFile file, long maxSizeBytes, string[] allowedExtensions)
    {
        try
        {
            // Check file size
            if (file.Length > maxSizeBytes)
            {
                _logger.LogWarning("File {FileName} exceeds size limit: {Size} > {MaxSize}", 
                    file.FileName, file.Length, maxSizeBytes);
                return false;
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("File {FileName} has disallowed extension: {Extension}", 
                    file.FileName, extension);
                return false;
            }

            // Check if file is empty
            if (file.Length == 0)
            {
                _logger.LogWarning("File {FileName} is empty", file.FileName);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file constraints for {FileName}", file.FileName);
            return false;
        }
    }
}
