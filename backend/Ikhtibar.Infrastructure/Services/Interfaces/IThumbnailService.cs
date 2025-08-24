
using ThumbnailSize = Ikhtibar.Shared.Enums.ThumbnailSize;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for media thumbnail generation
/// Handles thumbnail creation, storage, and retrieval
/// </summary>
public interface IThumbnailService
{
    /// <summary>
    /// Generates thumbnails for a media file
    /// </summary>
    Task<bool> GenerateThumbnailsAsync(MediaFile mediaFile);

    /// <summary>
    /// Gets thumbnail URL for a specific size
    /// </summary>
    Task<string> GetThumbnailUrlAsync(int mediaFileId, ThumbnailSize size = ThumbnailSize.Medium);

    /// <summary>
    /// Regenerates thumbnails for a media file
    /// </summary>
    Task<bool> RegenerateThumbnailsAsync(int mediaFileId);

    /// <summary>
    /// Deletes thumbnails for a media file
    /// </summary>
    Task<bool> DeleteThumbnailsAsync(int mediaFileId);

    /// <summary>
    /// Checks if thumbnails exist for a media file
    /// </summary>
    Task<bool> ThumbnailsExistAsync(int mediaFileId);

    /// <summary>
    /// Gets thumbnail dimensions for a specific size
    /// </summary>
    (int width, int height) GetThumbnailDimensions(ThumbnailSize size);
}
