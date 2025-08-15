using Microsoft.AspNetCore.Http;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for media storage operations
/// Handles file storage, retrieval, and URL generation
/// </summary>
public interface IMediaStorageService
{
    /// <summary>
    /// Uploads a file to storage
    /// </summary>
    Task<StorageUploadResult> UploadFileAsync(IFormFile file, string fileName, MediaUploadDto uploadDto);

    /// <summary>
    /// Gets a file stream from storage
    /// </summary>
    Task<Stream> GetFileStreamAsync(string filePath);

    /// <summary>
    /// Gets file bytes from storage
    /// </summary>
    Task<byte[]> GetFileBytesAsync(string filePath);

    /// <summary>
    /// Gets a public URL for a file
    /// </summary>
    Task<string> GetFileUrlAsync(string filePath);

    /// <summary>
    /// Gets a signed URL with expiration for private files
    /// </summary>
    Task<string> GetSignedUrlAsync(string filePath, TimeSpan expiration);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// Checks if a file exists in storage
    /// </summary>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    /// Extracts metadata from a file
    /// </summary>
    Task<Dictionary<string, object>> ExtractMetadataAsync(string filePath);
}

/// <summary>
/// Result of a storage upload operation
/// </summary>
public class StorageUploadResult
{
    public string FilePath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
