
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using MediaType = Ikhtibar.Shared.Entities.MediaType;
using Ikhtibar.Shared.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Media search service implementation
/// Handles search, filtering, and discovery of media files
/// </summary>
public class MediaSearchService : IMediaSearchService
{
    private readonly IMediaFileRepository _mediaRepository;
    private readonly IMediaCategoryRepository _categoryRepository;
    private readonly IMediaCollectionRepository _collectionRepository;
    private readonly ILogger<MediaSearchService> _logger;
    private readonly IMapper _mapper;

    public MediaSearchService(
        IMediaFileRepository mediaRepository,
        IMediaCategoryRepository categoryRepository,
        IMediaCollectionRepository collectionRepository,
        ILogger<MediaSearchService> logger,
        IMapper mapper)
    {
        _mediaRepository = mediaRepository;
        _categoryRepository = categoryRepository;
        _collectionRepository = collectionRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaFileSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching media files with criteria: {SearchTerm}, Type: {MediaType}, Category: {CategoryId}",
                searchDto.SearchTerm, searchDto.MediaType, searchDto.CategoryId);

            var searchResult = await _mediaRepository.SearchAsync(searchDto);
            var mediaDtos = _mapper.Map<IEnumerable<MediaFileDto>>(searchResult.Items);

            return new PagedResult<MediaFileDto>
            {
                Items = mediaDtos,
                TotalCount = searchResult.TotalCount,
                PageNumber = searchResult.PageNumber,
                PageSize = searchResult.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching media files");
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                return Enumerable.Empty<MediaFileDto>();
            }

            // Get media files with similar characteristics
            var similarMedia = new List<MediaFile>();
            
            // Same category
            if (mediaFile.CategoryId.HasValue)
            {
                var categoryMedia = await _mediaRepository.GetByCategoryAsync(mediaFile.CategoryId.Value, false);
                similarMedia.AddRange(categoryMedia.Where(m => m.Id != mediaId).Take(5));
            }
            
            // Same type
            var typeMedia = await _mediaRepository.GetByTypeAsync(mediaFile.MediaType, 0, 10);
            similarMedia.AddRange(typeMedia.Where(m => m.Id != mediaId && !similarMedia.Any(sm => sm.Id == m.Id)).Take(5));
            
            // Remove duplicates and limit results
            var uniqueSimilar = similarMedia.GroupBy(m => m.Id).Select(g => g.First()).Take(10);
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(uniqueSimilar);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar media for: {MediaId}", mediaId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, int? userId = null)
    {
        try
        {
            IEnumerable<MediaFile> recentMedia;
            
            if (userId.HasValue)
            {
                recentMedia = await _mediaRepository.GetByUserAsync(userId.Value, 0, count);
            }
            else
            {
                recentMedia = await _mediaRepository.GetRecentAsync(7, count);
            }
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(recentMedia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent media");
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByTagsAsync(string[] tags)
    {
        try
        {
            if (tags == null || !tags.Any())
            {
                return Enumerable.Empty<MediaFileDto>();
            }

            // Search for media files containing any of the tags
            var searchTerm = string.Join(" ", tags);
            var searchResults = await _mediaRepository.SearchAsync(searchTerm, null, null, 0, 100);
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(searchResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by tags: {Tags}", string.Join(", ", tags));
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetPopularMediaAsync(int count = 10, Shared.Enums.MediaType? mediaType = null)
    {
        try
        {
            var popularMedia = await _mediaRepository.GetMostAccessedAsync(count);
            
            if (mediaType.HasValue)
            {
                popularMedia = popularMedia.Where(m => m.MediaType == mediaType.Value);
            }
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(popularMedia.Take(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular media");
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByDateRangeAsync(DateTime startDate, DateTime endDate, int? userId = null)
    {
        try
        {
            // Get recent media and filter by date range
            var recentMedia = await GetRecentMediaAsync(1000, userId);
            var filteredMedia = recentMedia.Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate);
            
            return filteredMedia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by date range: {StartDate} to {EndDate}", startDate, endDate);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaBySizeRangeAsync(long minSize, long maxSize)
    {
        try
        {
            var largeMedia = await _mediaRepository.GetLargerThanAsync(minSize);
            var filteredMedia = largeMedia.Where(m => m.FileSizeBytes <= maxSize);
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(filteredMedia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by size range: {MinSize} to {MaxSize} bytes", minSize, maxSize);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<Dictionary<string, int>> GetTagStatisticsAsync()
    {
        try
        {
            // Get all media files and extract tags from metadata
            var allMedia = await _mediaRepository.GetRecentAsync(1000, 1000);
            var tagCounts = new Dictionary<string, int>();
            
            foreach (var media in allMedia)
            {
                // Extract tags from metadata if available
                if (!string.IsNullOrEmpty(media.Metadata))
                {
                    try
                    {
                        // Parse metadata JSON to extract tags
                        // For now, we'll look for common tag patterns
                        var metadata = media.Metadata.ToLowerInvariant();
                        if (metadata.Contains("tags") || metadata.Contains("keywords"))
                        {
                            // This is a simplified approach - in production you'd use proper JSON parsing
                            var tagPatterns = new[] { "image", "video", "audio", "document", "photo", "picture" };
                            foreach (var tag in tagPatterns)
                            {
                                if (metadata.Contains(tag))
                                {
                                    if (tagCounts.ContainsKey(tag))
                                        tagCounts[tag]++;
                                    else
                                        tagCounts[tag] = 1;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore metadata parsing errors
                    }
                }
            }
            
            return tagCounts.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag statistics");
            return new Dictionary<string, int>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByCollectionAsync(int collectionId, int offset = 0, int limit = 50)
    {
        try
        {
            var collectionItems = await _collectionRepository.GetCollectionItemsAsync(collectionId, offset, limit);
            var mediaIds = collectionItems.Select(ci => ci.MediaFileId);
            
            var mediaFiles = new List<MediaFile>();
            foreach (var mediaId in mediaIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    mediaFiles.Add(mediaFile);
                }
            }
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by collection: {CollectionId}", collectionId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByCategoryHierarchyAsync(int categoryId, bool includeSubcategories = true)
    {
        try
        {
            var mediaFiles = await _mediaRepository.GetByCategoryAsync(categoryId, includeSubcategories);
            return _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by category hierarchy: {CategoryId}", categoryId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<SearchStatistics> GetSearchStatisticsAsync()
    {
        try
        {
            var totalMedia = await _mediaRepository.GetRecentAsync(1, 1); // Just to get count
            var totalCategories = await _categoryRepository.GetRootCategoriesAsync();
            var totalCollections = await _collectionRepository.GetPublicAsync(0, 1);
            
            return new SearchStatistics
            {
                TotalMediaFiles = totalMedia.Count(),
                TotalCategories = totalCategories.Count(),
                TotalCollections = totalCollections.Count(),
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search statistics");
            return new SearchStatistics
            {
                TotalMediaFiles = 0,
                TotalCategories = 0,
                TotalCollections = 0,
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    // Implementation of missing interface methods
    public async Task<PagedResult<MediaFileDto>> GetMediaByCategoryAsync(int categoryId, int page = 1, int pageSize = 20)
    {
        try
        {
            var offset = (page - 1) * pageSize;
            var mediaFiles = await _mediaRepository.GetByCategoryAsync(categoryId, false);
            var totalCount = mediaFiles.Count();
            var pagedMedia = mediaFiles.Skip(offset).Take(pageSize);
            
            var mediaDtos = _mapper.Map<IEnumerable<MediaFileDto>>(pagedMedia);
            
            return new PagedResult<MediaFileDto>
            {
                Items = mediaDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by category: {CategoryId}", categoryId);
            return new PagedResult<MediaFileDto>
            {
                Items = Enumerable.Empty<MediaFileDto>(),
                TotalCount = 0,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByCollectionAsync(int collectionId)
    {
        try
        {
            var collectionItems = await _collectionRepository.GetCollectionItemsAsync(collectionId, 0, 1000);
            var mediaIds = collectionItems.Select(ci => ci.MediaFileId);
            
            var mediaFiles = new List<MediaFile>();
            foreach (var mediaId in mediaIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    mediaFiles.Add(mediaFile);
                }
            }
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by collection: {CollectionId}", collectionId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaByUserAsync(int userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var offset = (page - 1) * pageSize;
            var mediaFiles = await _mediaRepository.GetByUserAsync(userId, offset, pageSize);
            var totalCount = mediaFiles.Count();
            
            var mediaDtos = _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
            
            return new PagedResult<MediaFileDto>
            {
                Items = mediaDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by user: {UserId}", userId);
            return new PagedResult<MediaFileDto>
            {
                Items = Enumerable.Empty<MediaFileDto>(),
                TotalCount = 0,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaByTypeAsync(Shared.Enums.MediaType mediaType, int page = 1, int pageSize = 20)
    {
        try
        {
            var offset = (page - 1) * pageSize;
            var mediaFiles = await _mediaRepository.GetByTypeAsync(mediaType, offset, pageSize);
            var totalCount = mediaFiles.Count();
            
            var mediaDtos = _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
            
            return new PagedResult<MediaFileDto>
            {
                Items = mediaDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by type: {MediaType}", mediaType);
            return new PagedResult<MediaFileDto>
            {
                Items = Enumerable.Empty<MediaFileDto>(),
                TotalCount = 0,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            // Get recent media and filter by date range
            var recentMedia = await GetRecentMediaAsync(1000, null);
            var filteredMedia = recentMedia.Where(m => m.CreatedAt >= fromDate && m.CreatedAt <= toDate);
            
            return filteredMedia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by date range: {FromDate} to {ToDate}", fromDate, toDate);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetPopularMediaAsync(int count = 10, TimeSpan? timeRange = null)
    {
        try
        {
            var popularMedia = await _mediaRepository.GetMostAccessedAsync(count);
            
            if (timeRange.HasValue)
            {
                var cutoffDate = DateTime.UtcNow.Subtract(timeRange.Value);
                popularMedia = popularMedia.Where(m => m.CreatedAt >= cutoffDate);
            }
            
            return _mapper.Map<IEnumerable<MediaFileDto>>(popularMedia.Take(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular media");
            return Enumerable.Empty<MediaFileDto>();
        }
    }
}
