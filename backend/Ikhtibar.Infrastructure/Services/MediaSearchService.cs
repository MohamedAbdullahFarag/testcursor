using Ikhtibar.Core.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
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

            var (mediaFiles, totalCount) = await _mediaRepository.SearchAsync(searchDto);
            var mediaFileDtos = _mapper.Map<List<MediaFileDto>>(mediaFiles);

            return new PagedResult<MediaFileDto>
            {
                Items = mediaFileDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching media files");
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(Guid mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                return Enumerable.Empty<MediaFileDto>();
            }

            // Get media files with similar characteristics
            var similarMedia = await _mediaRepository.GetSimilarMediaAsync(mediaId, 10);
            return _mapper.Map<List<MediaFileDto>>(similarMedia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar media for: {MediaId}", mediaId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, Guid? userId = null)
    {
        try
        {
            var recentMedia = await _mediaRepository.GetRecentMediaAsync(count, userId);
            return _mapper.Map<List<MediaFileDto>>(recentMedia);
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
            var mediaFiles = await _mediaRepository.GetByTagsAsync(tags);
            return _mapper.Map<List<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by tags: {Tags}", string.Join(", ", tags));
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20)
    {
        try
        {
            var (mediaFiles, totalCount) = await _mediaRepository.GetByCategoryAsync(categoryId, page, pageSize);
            var mediaFileDtos = _mapper.Map<List<MediaFileDto>>(mediaFiles);

            return new PagedResult<MediaFileDto>
            {
                Items = mediaFileDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByCollectionAsync(Guid collectionId)
    {
        try
        {
            var mediaFiles = await _mediaRepository.GetByCollectionAsync(collectionId);
            return _mapper.Map<List<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by collection: {CollectionId}", collectionId);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaByUserAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var (mediaFiles, totalCount) = await _mediaRepository.GetByUserAsync(userId, page, pageSize);
            var mediaFileDtos = _mapper.Map<List<MediaFileDto>>(mediaFiles);

            return new PagedResult<MediaFileDto>
            {
                Items = mediaFileDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by user: {UserId}", userId);
            throw;
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaByTypeAsync(MediaFileType mediaType, int page = 1, int pageSize = 20)
    {
        try
        {
            var (mediaFiles, totalCount) = await _mediaRepository.GetByTypeAsync(mediaType, page, pageSize);
            var mediaFileDtos = _mapper.Map<List<MediaFileDto>>(mediaFiles);

            return new PagedResult<MediaFileDto>
            {
                Items = mediaFileDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by type: {MediaType}", mediaType);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var mediaFiles = await _mediaRepository.GetByDateRangeAsync(fromDate, toDate);
            return _mapper.Map<List<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by date range: {FromDate} to {ToDate}", fromDate, toDate);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetMediaBySizeRangeAsync(long minSizeBytes, long maxSizeBytes)
    {
        try
        {
            var mediaFiles = await _mediaRepository.GetBySizeRangeAsync(minSizeBytes, maxSizeBytes);
            return _mapper.Map<List<MediaFileDto>>(mediaFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media by size range: {MinSize} to {MaxSize} bytes", minSizeBytes, maxSizeBytes);
            return Enumerable.Empty<MediaFileDto>();
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetPopularMediaAsync(int count = 10, TimeSpan? timeRange = null)
    {
        try
        {
            var popularMedia = await _mediaRepository.GetPopularMediaAsync(count, timeRange);
            return _mapper.Map<List<MediaFileDto>>(popularMedia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular media");
            return Enumerable.Empty<MediaFileDto>();
        }
    }
}
