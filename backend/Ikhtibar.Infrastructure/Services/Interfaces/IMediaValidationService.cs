using Microsoft.AspNetCore.Http;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for media file validation
/// Handles file type, size, and content validation
/// </summary>
public interface IMediaValidationService
{
    /// <summary>
    /// Validates a file for upload
    /// </summary>
    Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto);

    /// <summary>
    /// Checks if file type is allowed
    /// </summary>
    bool IsAllowedFileType(string fileName, string contentType);

    /// <summary>
    /// Checks if file size is within limits
    /// </summary>
    bool IsFileSizeValid(long fileSizeBytes, Shared.Enums.MediaType mediaType);

    /// <summary>
    /// Performs virus scanning on file (if configured)
    /// </summary>
    Task<bool> ScanForVirusAsync(IFormFile file);

    /// <summary>
    /// Validates file content integrity
    /// </summary>
    Task<bool> ValidateFileIntegrityAsync(IFormFile file);
}
