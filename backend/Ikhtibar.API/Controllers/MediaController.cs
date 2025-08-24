using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using System.ComponentModel.DataAnnotations;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Infrastructure.Services;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Media management controller
/// Provides comprehensive media operations including upload, download, management, and access control
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a new media file
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<MediaFileDto>> UploadFile(
        [Required] IFormFile file,
        [FromForm] MediaUploadDto uploadDto)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            uploadDto.UploadedBy = int.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value ?? "0");
            var result = await _mediaService.UploadFileAsync(file, uploadDto);
            return CreatedAtAction(nameof(GetMediaFile), new { id = result.Id }, result);
        }
        catch (Ikhtibar.Core.Exceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
            return StatusCode(500, "An error occurred while uploading the file");
        }
    }

    /// <summary>
    /// Get a media file by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MediaFileDto>> GetMediaFile(int id)
    {
        try
        {
            var mediaFile = await _mediaService.GetMediaFileAsync(id);
            if (mediaFile == null)
            {
                return NotFound();
            }

            return Ok(mediaFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media file: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the media file");
        }
    }

    /// <summary>
    /// Get media files with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<MediaFileDto>>> GetMediaFiles(
        [FromQuery] MediaFileSearchDto searchDto)
    {
        try
        {
            var result = await _mediaService.GetMediaFilesAsync(searchDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files");
            return StatusCode(500, "An error occurred while retrieving media files");
        }
    }

    /// <summary>
    /// Update a media file
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<MediaFileDto>> UpdateMediaFile(
        int id,
        [FromBody] UpdateMediaFileDto updateDto)
    {
        try
        {
            var result = await _mediaService.UpdateMediaFileAsync(id, updateDto);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating media file: {Id}", id);
            return StatusCode(500, "An error occurred while updating the media file");
        }
    }

    /// <summary>
    /// Delete a media file
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> DeleteMediaFile(int id)
    {
        try
        {
            var result = await _mediaService.DeleteMediaFileAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting media file: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the media file");
        }
    }

    /// <summary>
    /// Download a media file
    /// </summary>
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadMediaFile(int id)
    {
        try
        {
            var mediaFile = await _mediaService.GetMediaFileAsync(id);
            if (mediaFile == null)
            {
                return NotFound();
            }

            var stream = await _mediaService.GetFileStreamAsync(id);
            return File(stream, mediaFile.ContentType, mediaFile.OriginalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading media file: {Id}", id);
            return StatusCode(500, "An error occurred while downloading the file");
        }
    }

    /// <summary>
    /// Get media file URL
    /// </summary>
    [HttpGet("{id:int}/url")]
    public async Task<ActionResult<string>> GetMediaFileUrl(
        int id,
        [FromQuery] TimeSpan? expirationTime = null)
    {
        try
        {
            var url = await _mediaService.GetFileUrlAsync(id, expirationTime);
            return Ok(new { url });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media file URL: {Id}", id);
            return StatusCode(500, "An error occurred while getting the file URL");
        }
    }

    /// <summary>
    /// Get thumbnail URL for a media file
    /// </summary>
    [HttpGet("{id:int}/thumbnail")]
    public async Task<ActionResult<string>> GetThumbnailUrl(
        int id,
        [FromQuery] ThumbnailSize size = ThumbnailSize.Medium)
    {
        try
        {
            var url = await _mediaService.GetThumbnailUrlAsync(id, size);
            if (string.IsNullOrEmpty(url))
            {
                return NotFound("Thumbnail not available");
            }

            return Ok(new { url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting thumbnail URL: {Id}", id);
            return StatusCode(500, "An error occurred while getting the thumbnail URL");
        }
    }

    /// <summary>
    /// Get processing status for a media file
    /// </summary>
    [HttpGet("{id:int}/processing-status")]
    public async Task<ActionResult<MediaProcessingStatusDto>> GetProcessingStatus(int id)
    {
        try
        {
            var status = await _mediaService.GetProcessingStatusAsync(id);
            return Ok(status);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing status: {Id}", id);
            return StatusCode(500, "An error occurred while getting the processing status");
        }
    }

    /// <summary>
    /// Regenerate media processing for a file
    /// </summary>
    [HttpPost("{id:int}/regenerate-processing")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> RegenerateProcessing(int id)
    {
        try
        {
            var result = await _mediaService.RegenerateMediaProcessingAsync(id);
            if (!result)
            {
                return BadRequest("Failed to regenerate processing");
            }

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating processing: {Id}", id);
            return StatusCode(500, "An error occurred while regenerating processing");
        }
    }

    /// <summary>
    /// Get media categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<MediaCategoryDto>>> GetCategories()
    {
        try
        {
            var categories = await _mediaService.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media categories");
            return StatusCode(500, "An error occurred while retrieving categories");
        }
    }

    /// <summary>
    /// Get media collections
    /// </summary>
    [HttpGet("collections")]
    public async Task<ActionResult<IEnumerable<MediaCollectionDto>>> GetCollections(
        [FromQuery] int? userId = null)
    {
        try
        {
            var collections = await _mediaService.GetCollectionsAsync(userId);
            return Ok(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media collections");
            return StatusCode(500, "An error occurred while retrieving collections");
        }
    }

    /// <summary>
    /// Create a new media collection
    /// </summary>
    [HttpPost("collections")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult<MediaCollectionDto>> CreateCollection(
        [FromBody] CreateMediaCollectionDto createDto)
    {
        try
        {
            createDto.CreatedByUserId = int.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value ?? "0");
            var result = await _mediaService.CreateCollectionAsync(createDto);
            return CreatedAtAction(nameof(GetCollections), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating media collection");
            return StatusCode(500, "An error occurred while creating the collection");
        }
    }

    /// <summary>
    /// Add media to collection
    /// </summary>
    [HttpPost("collections/{collectionId:int}/media/{mediaId:int}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> AddMediaToCollection(int collectionId, int mediaId)
    {
        try
        {
            var result = await _mediaService.AddMediaToCollectionAsync(mediaId, collectionId);
            if (!result)
            {
                return BadRequest("Failed to add media to collection");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding media to collection: {MediaId}, {CollectionId}", mediaId, collectionId);
            return StatusCode(500, "An error occurred while adding media to collection");
        }
    }

    /// <summary>
    /// Remove media from collection
    /// </summary>
    [HttpDelete("collections/{collectionId:int}/media/{mediaId:int}")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> RemoveMediaFromCollection(int collectionId, int mediaId)
    {
        try
        {
            var result = await _mediaService.RemoveMediaFromCollectionAsync(mediaId, collectionId);
            if (!result)
            {
                return BadRequest("Failed to remove media from collection");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing media from collection: {MediaId}, {CollectionId}", mediaId, collectionId);
            return StatusCode(500, "An error occurred while removing media from collection");
        }
    }

    /// <summary>
    /// Search media files
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<MediaFileDto>>> SearchMedia(
        [FromBody] MediaFileSearchDto searchDto)
    {
        try
        {
            var result = await _mediaService.SearchMediaAsync(searchDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching media");
            return StatusCode(500, "An error occurred while searching media");
        }
    }

    /// <summary>
    /// Get similar media files
    /// </summary>
    [HttpGet("{id:int}/similar")]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetSimilarMedia(
        int id,
        [FromQuery] int maxResults = 10)
    {
        try
        {
            var result = await _mediaService.GetSimilarMediaAsync(id);
            return Ok(result.Take(maxResults));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar media: {Id}", id);
            return StatusCode(500, "An error occurred while getting similar media");
        }
    }

    /// <summary>
    /// Get recent media files
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetRecentMedia(
        [FromQuery] int count = 10,
        [FromQuery] int? userId = null)
    {
        try
        {
            var result = await _mediaService.GetRecentMediaAsync(count, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent media");
            return StatusCode(500, "An error occurred while getting recent media");
        }
    }

    /// <summary>
    /// Bulk delete media files
    /// </summary>
    [HttpPost("bulk-delete")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> BulkDeleteMedia([FromBody] List<int> mediaIds)
    {
        try
        {
            if (mediaIds == null || !mediaIds.Any())
            {
                return BadRequest("No media IDs provided");
            }

            var result = await _mediaService.BulkDeleteMediaAsync(mediaIds);
            if (!result)
            {
                return BadRequest("Some files could not be deleted");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting media files");
            return StatusCode(500, "An error occurred while bulk deleting media files");
        }
    }

    /// <summary>
    /// Bulk move media files to category
    /// </summary>
    [HttpPost("bulk-move")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> BulkMoveMedia(
        [FromBody] BulkMoveMediaDto moveDto)
    {
        try
        {
            if (moveDto.MediaIds == null || !moveDto.MediaIds.Any())
            {
                return BadRequest("No media IDs provided");
            }

            var result = await _mediaService.BulkMoveMediaAsync(moveDto.MediaIds, moveDto.TargetCategoryId);
            if (!result)
            {
                return BadRequest("Some files could not be moved");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk moving media files");
            return StatusCode(500, "An error occurred while bulk moving media files");
        }
    }

    /// <summary>
    /// Bulk update media files
    /// </summary>
    [HttpPost("bulk-update")]
    [Authorize(Policy = "QuestionManagement")]
    public async Task<ActionResult> BulkUpdateMedia(
        [FromBody] BulkMediaFileOperationDto updateDto)
    {
        try
        {
            if (updateDto.MediaFileIds == null || !updateDto.MediaFileIds.Any())
            {
                return BadRequest("No media IDs provided");
            }

            var result = await _mediaService.BulkUpdateMediaAsync(updateDto);
            if (!result)
            {
                return BadRequest("Some files could not be updated");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating media files");
            return StatusCode(500, "An error occurred while bulk updating media files");
        }
    }

    /// <summary>
    /// Validate file before upload
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<MediaValidationResult>> ValidateFile(
        [Required] IFormFile file,
        [FromForm] MediaUploadDto uploadDto)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            var result = await _mediaService.ValidateFileAsync(file, uploadDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file: {FileName}", file?.FileName);
            return StatusCode(500, "An error occurred while validating the file");
        }
    }
}

/// <summary>
/// DTO for bulk move operations
/// </summary>
public class BulkMoveMediaDto
{
    public List<int> MediaIds { get; set; } = new();
    public int TargetCategoryId { get; set; }
}
