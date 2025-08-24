using Microsoft.AspNetCore.Http;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for file upload operations
/// Handles file validation, hash calculation, and filename generation
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Calculates SHA256 hash of a file for duplicate detection
    /// </summary>
    Task<string> CalculateFileHashAsync(IFormFile file);

    /// <summary>
    /// Generates a unique filename to avoid conflicts
    /// </summary>
    Task<string> GenerateUniqueFileNameAsync(string originalFileName);

    /// <summary>
    /// Determines media type based on content type and file extension
    /// </summary>
    Shared.Enums.MediaType DetermineMediaType(string contentType, string fileName);

    /// <summary>
    /// Validates file size and type constraints
    /// </summary>
    Task<bool> ValidateFileConstraintsAsync(IFormFile file, long maxSizeBytes, string[] allowedExtensions);
}
