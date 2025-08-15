using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.IO;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Local file system storage service implementation
/// Handles file storage on the local file system
/// </summary>
public class LocalFileStorageService : IMediaStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _baseStoragePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _baseStoragePath = configuration["MediaStorage:LocalPath"] ?? "wwwroot/media";
        _baseUrl = configuration["MediaStorage:BaseUrl"] ?? "/media";
        
        // Ensure storage directory exists
        Directory.CreateDirectory(_baseStoragePath);
    }

    public async Task<StorageUploadResult> UploadFileAsync(IFormFile file, string fileName, MediaUploadDto uploadDto)
    {
        try
        {
            // Create directory structure based on date
            var datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var uploadPath = Path.Combine(_baseStoragePath, datePath);
            Directory.CreateDirectory(uploadPath);

            // Full file path
            var filePath = Path.Combine(uploadPath, fileName);
            var relativePath = Path.Combine(datePath, fileName).Replace("\\", "/");

            // Save file
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Generate URL
            var url = $"{_baseUrl}/{relativePath}";

            _logger.LogInformation("File uploaded successfully: {FilePath}, URL: {Url}", filePath, url);

            return new StorageUploadResult
            {
                FilePath = relativePath,
                Url = url,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            return new StorageUploadResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<Stream> GetFileStreamAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file stream: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<byte[]> GetFileBytesAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file bytes: {FilePath}", filePath);
            throw;
        }
    }

    public Task<string> GetFileUrlAsync(string filePath)
    {
        var url = $"{_baseUrl}/{filePath.Replace("\\", "/")}";
        return Task.FromResult(url);
    }

    public Task<string> GetSignedUrlAsync(string filePath, TimeSpan expiration)
    {
        // For local storage, we don't need signed URLs
        // Just return the regular URL
        return GetFileUrlAsync(filePath);
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return false;
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_baseStoragePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public async Task<Dictionary<string, object>> ExtractMetadataAsync(string filePath)
    {
        try
        {
            var metadata = new Dictionary<string, object>();
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            if (!File.Exists(fullPath))
            {
                return metadata;
            }

            var fileInfo = new FileInfo(fullPath);
            metadata["FileSize"] = fileInfo.Length;
            metadata["Created"] = fileInfo.CreationTimeUtc;
            metadata["Modified"] = fileInfo.LastWriteTimeUtc;
            metadata["Extension"] = fileInfo.Extension;

            // For images, we could extract more metadata using ImageSharp or similar
            // For now, we'll just return basic file info
            if (fileInfo.Extension.ToLowerInvariant() is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
            {
                metadata["Type"] = "Image";
            }
            else if (fileInfo.Extension.ToLowerInvariant() is ".mp4" or ".avi" or ".mov" or ".wmv")
            {
                metadata["Type"] = "Video";
            }
            else if (fileInfo.Extension.ToLowerInvariant() is ".mp3" or ".wav" or ".ogg")
            {
                metadata["Type"] = "Audio";
            }
            else if (fileInfo.Extension.ToLowerInvariant() is ".pdf" or ".doc" or ".txt")
            {
                metadata["Type"] = "Document";
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting metadata: {FilePath}", filePath);
            return new Dictionary<string, object>();
        }
    }

    public Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Copy(sourcePath, destinationPath, true);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file: {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
            return Task.FromResult(false);
        }
    }

    public Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Move(sourcePath, destinationPath, true);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving file: {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
            return Task.FromResult(false);
        }
    }

    private async Task<string> CalculateFileHashAsync(string filePath)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating file hash: {FilePath}", filePath);
            return string.Empty;
        }
    }
}
