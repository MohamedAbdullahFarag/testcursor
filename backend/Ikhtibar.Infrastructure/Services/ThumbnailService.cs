using Ikhtibar.Core.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Thumbnail service implementation
/// Handles thumbnail generation, storage, and retrieval
/// </summary>
public class ThumbnailService : IThumbnailService
{
    private readonly IMediaThumbnailRepository _thumbnailRepository;
    private readonly IMediaFileRepository _mediaRepository;
    private readonly ILogger<ThumbnailService> _logger;

    public ThumbnailService(
        IMediaThumbnailRepository thumbnailRepository,
        IMediaFileRepository mediaRepository,
        ILogger<ThumbnailService> logger)
    {
        _thumbnailRepository = thumbnailRepository;
        _mediaRepository = mediaRepository;
        _logger = logger;
    }

    public async Task<bool> GenerateThumbnailsAsync(MediaFile mediaFile)
    {
        try
        {
            _logger.LogInformation("Generating thumbnails for media file: {MediaId}, Type: {MediaType}", 
                mediaFile.Id, mediaFile.MediaType);

            // Only generate thumbnails for images and videos
            if (mediaFile.MediaType != Ikhtibar.Core.Entities.MediaType.Image && mediaFile.MediaType != Ikhtibar.Core.Entities.MediaType.Video)
            {
                _logger.LogInformation("Skipping thumbnail generation for non-image/video file: {MediaId}", mediaFile.Id);
                return true;
            }

            var success = true;

            // Generate thumbnails for all sizes
            foreach (ThumbnailSize size in Enum.GetValues(typeof(ThumbnailSize)))
            {
                try
                {
                    var thumbnail = await GenerateThumbnailAsync(mediaFile, size);
                    if (thumbnail != null)
                    {
                        await _thumbnailRepository.AddAsync(thumbnail);
                        _logger.LogDebug("Generated thumbnail for size {Size}: {MediaId}", size, mediaFile.Id);
                    }
                    else
                    {
                        success = false;
                        _logger.LogWarning("Failed to generate thumbnail for size {Size}: {MediaId}", size, mediaFile.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating thumbnail for size {Size}: {MediaId}", size, mediaFile.Id);
                    success = false;
                }
            }

            _logger.LogInformation("Thumbnail generation completed for media file: {MediaId}, Success: {Success}", 
                mediaFile.Id, success);

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating thumbnails for media file: {MediaId}", mediaFile.Id);
            return false;
        }
    }

    public async Task<string> GetThumbnailUrlAsync(int mediaFileId, ThumbnailSize size = ThumbnailSize.Medium)
    {
        try
        {
            var thumbnail = await _thumbnailRepository.GetByMediaFileAndSizeAsync(mediaFileId, size);
            if (thumbnail != null)
            {
                return thumbnail.StoragePath; // This should be the URL
            }

            // If thumbnail doesn't exist, try to generate it
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaFileId);
            if (mediaFile != null)
            {
                var newThumbnail = await GenerateThumbnailAsync(mediaFile, size);
                if (newThumbnail != null)
                {
                    await _thumbnailRepository.AddAsync(newThumbnail);
                    return newThumbnail.StoragePath;
                }
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting thumbnail URL for media file: {MediaId}, Size: {Size}", mediaFileId, size);
            return string.Empty;
        }
    }

    public async Task<bool> RegenerateThumbnailsAsync(int mediaFileId)
    {
        try
        {
            _logger.LogInformation("Regenerating thumbnails for media file: {MediaId}", mediaFileId);

            // Delete existing thumbnails
            await DeleteThumbnailsAsync(mediaFileId);

            // Get media file and regenerate
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaFileId);
            if (mediaFile != null)
            {
                return await GenerateThumbnailsAsync(mediaFile);
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error regenerating thumbnails for media file: {MediaId}", mediaFileId);
            return false;
        }
    }

    public async Task<bool> DeleteThumbnailsAsync(int mediaFileId)
    {
        try
        {
            var thumbnails = await _thumbnailRepository.GetByMediaFileIdAsync(mediaFileId);
            foreach (var thumbnail in thumbnails)
            {
                await _thumbnailRepository.DeleteAsync(thumbnail.Id);
            }

            _logger.LogInformation("Deleted {Count} thumbnails for media file: {MediaId}", thumbnails.Count, mediaFileId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting thumbnails for media file: {MediaId}", mediaFileId);
            return false;
        }
    }

    public async Task<bool> ThumbnailsExistAsync(int mediaFileId)
    {
        try
        {
            var thumbnails = await _thumbnailRepository.GetByMediaFileIdAsync(mediaFileId);
            return thumbnails.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error checking thumbnails existence for media file: {MediaId}", mediaFileId);
            return false;
        }
    }

    public (int width, int height) GetThumbnailDimensions(ThumbnailSize size)
    {
        return size switch
        {
            ThumbnailSize.Small => (64, 64),
            ThumbnailSize.Medium => (150, 150),
            ThumbnailSize.Large => (300, 300),
            ThumbnailSize.ExtraLarge => (600, 600),
            _ => (150, 150) // Default to medium
        };
    }

    private async Task<MediaThumbnail?> GenerateThumbnailAsync(MediaFile mediaFile, ThumbnailSize size)
    {
        try
        {
            var (width, height) = GetThumbnailDimensions(size);
            
            // For now, we'll create a placeholder thumbnail
            // In a production environment, this would use image processing libraries like ImageSharp
            // or video processing libraries to extract frames and generate actual thumbnails
            
            var thumbnailPath = $"thumbnails/{mediaFile.Id}_{size}_{width}x{height}.jpg";
            
            var thumbnail = new MediaThumbnail
            {
                MediaFileId = mediaFile.Id,
                Size = size,
                Width = width,
                Height = height,
                StoragePath = thumbnailPath,
                FileSizeBytes = 0, // Will be set after actual generation
                ContentType = "image/jpeg",
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogDebug("Created thumbnail entity for size {Size}: {Width}x{Height}, Path: {Path}", 
                size, width, height, thumbnailPath);

            return thumbnail;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error generating thumbnail for media file: {MediaId}, Size: {Size}", 
                mediaFile.Id, size);
            return null;
        }
    }
}
