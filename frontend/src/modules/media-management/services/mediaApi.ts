import { apiClient } from '@/shared/services/apiClient';
import {
  MediaFile,
  MediaFileDto,
  CreateMediaFileDto,
  UpdateMediaFileDto,
  MediaFileSearchDto,
  MediaCategory,
  MediaCategoryDto,
  CreateMediaCategoryDto,
  UpdateMediaCategoryDto,
  MediaCollection,
  MediaCollectionDto,
  CreateMediaCollectionDto,
  UpdateMediaCollectionDto,
  MediaThumbnail,
  MediaAccessLogDto,
  BulkMediaFileOperationDto,
  PagedResult,
  SearchStatistics,
  MediaType,
  MediaStatus,
  CollectionType
} from '../types';

// Media Files API
export const mediaFilesApi = {
  // Get all media files with pagination and filtering
  async getMediaFiles(params: MediaFileSearchDto): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.post('/api/media/search', params);
    return response.data;
  },

  // Get media file by ID
  async getMediaFile(id: number): Promise<MediaFileDto> {
    const response = await apiClient.get(`/api/media/${id}`);
    return response.data;
  },

  // Create new media file
  async createMediaFile(data: CreateMediaFileDto): Promise<MediaFileDto> {
    const response = await apiClient.post('/api/media', data);
    return response.data;
  },

  // Update media file
  async updateMediaFile(id: number, data: UpdateMediaFileDto): Promise<MediaFileDto> {
    const response = await apiClient.put(`/api/media/${id}`, data);
    return response.data;
  },

  // Delete media file
  async deleteMediaFile(id: number): Promise<boolean> {
    const response = await apiClient.delete(`/api/media/${id}`);
    return response.data;
  },

  // Upload media file
  async uploadMediaFile(file: File, metadata?: Partial<CreateMediaFileDto>): Promise<MediaFileDto> {
    const formData = new FormData();
    formData.append('file', file);
    
    if (metadata) {
      Object.entries(metadata).forEach(([key, value]) => {
        if (value !== undefined) {
          formData.append(key, value.toString());
        }
      });
    }

    const response = await apiClient.post('/api/media/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  // Download media file
  async downloadMediaFile(id: number): Promise<Blob> {
    const response = await apiClient.get(`/api/media/${id}/download`, {
      responseType: 'blob',
    });
    return response.data;
  },

  // Get media files by category
  async getMediaFilesByCategory(categoryId: number, page = 1, pageSize = 20): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.get(`/api/media/category/${categoryId}`, {
      params: { page, pageSize }
    });
    return response.data;
  },

  // Get media files by collection
  async getMediaFilesByCollection(collectionId: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get(`/api/media/collection/${collectionId}`);
    return response.data;
  },

  // Get media files by user
  async getMediaFilesByUser(userId: number, page = 1, pageSize = 20): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.get(`/api/media/user/${userId}`, {
      params: { page, pageSize }
    });
    return response.data;
  },

  // Get media files by type
  async getMediaFilesByType(mediaType: MediaType, page = 1, pageSize = 20): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.get(`/api/media/type/${mediaType}`, {
      params: { page, pageSize }
    });
    return response.data;
  },

  // Get recent media files
  async getRecentMediaFiles(count = 10, userId?: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get('/api/media/recent', {
      params: { count, userId }
    });
    return response.data;
  },

  // Get popular media files
  async getPopularMediaFiles(count = 10, timeRange?: string): Promise<MediaFileDto[]> {
    const response = await apiClient.get('/api/media/popular', {
      params: { count, timeRange }
    });
    return response.data;
  },

  // Get similar media files
  async getSimilarMediaFiles(mediaId: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get(`/api/media/${mediaId}/similar`);
    return response.data;
  },

  // Bulk operations
  async bulkDeleteMedia(mediaIds: number[]): Promise<boolean> {
    const response = await apiClient.post('/api/media/bulk-delete', { mediaIds });
    return response.data;
  },

  async bulkMoveMedia(mediaIds: number[], targetCategoryId: number): Promise<boolean> {
    const response = await apiClient.post('/api/media/bulk-move', { mediaIds, targetCategoryId });
    return response.data;
  },

  async bulkUpdateMedia(data: BulkMediaFileOperationDto): Promise<boolean> {
    const response = await apiClient.post('/api/media/bulk-update', data);
    return response.data;
  },

  // Log media access
  async logMediaAccess(mediaId: number, userId: number, action: string): Promise<MediaAccessLogDto> {
    const response = await apiClient.post(`/api/media/${mediaId}/access-log`, {
      userId,
      action
    });
    return response.data;
  }
};

// Media Categories API
export const mediaCategoriesApi = {
  // Get all categories
  async getCategories(): Promise<MediaCategoryDto[]> {
    const response = await apiClient.get('/api/media/categories');
    return response.data;
  },

  // Get category by ID
  async getCategory(id: number): Promise<MediaCategoryDto> {
    const response = await apiClient.get(`/api/media/categories/${id}`);
    return response.data;
  },

  // Create new category
  async createCategory(data: CreateMediaCategoryDto): Promise<MediaCategoryDto> {
    const response = await apiClient.post('/api/media/categories', data);
    return response.data;
  },

  // Update category
  async updateCategory(id: number, data: UpdateMediaCategoryDto): Promise<MediaCategoryDto> {
    const response = await apiClient.put(`/api/media/categories/${id}`, data);
    return response.data;
  },

  // Delete category
  async deleteCategory(id: number): Promise<boolean> {
    const response = await apiClient.delete(`/api/media/categories/${id}`);
    return response.data;
  },

  // Get root categories
  async getRootCategories(): Promise<MediaCategoryDto[]> {
    const response = await apiClient.get('/api/media/categories/root');
    return response.data;
  },

  // Get child categories
  async getChildCategories(parentId: number): Promise<MediaCategoryDto[]> {
    const response = await apiClient.get(`/api/media/categories/${parentId}/children`);
    return response.data;
  },

  // Get category tree
  async getCategoryTree(): Promise<MediaCategoryDto[]> {
    const response = await apiClient.get('/api/media/categories/tree');
    return response.data;
  }
};

// Media Collections API
export const mediaCollectionsApi = {
  // Get all collections
  async getCollections(userId?: number): Promise<MediaCollectionDto[]> {
    const response = await apiClient.get('/api/media/collections', {
      params: { userId }
    });
    return response.data;
  },

  // Get collection by ID
  async getCollection(id: number): Promise<MediaCollectionDto> {
    const response = await apiClient.get(`/api/media/collections/${id}`);
    return response.data;
  },

  // Create new collection
  async createCollection(data: CreateMediaCollectionDto): Promise<MediaCollectionDto> {
    const response = await apiClient.post('/api/media/collections', data);
    return response.data;
  },

  // Update collection
  async updateCollection(id: number, data: UpdateMediaCollectionDto): Promise<MediaCollectionDto> {
    const response = await apiClient.put(`/api/media/collections/${id}`, data);
    return response.data;
  },

  // Delete collection
  async deleteCollection(id: number): Promise<boolean> {
    const response = await apiClient.delete(`/api/media/collections/${id}`);
    return response.data;
  },

  // Get collections by type
  async getCollectionsByType(type: CollectionType, publicOnly = true): Promise<MediaCollectionDto[]> {
    const response = await apiClient.get(`/api/media/collections/type/${type}`, {
      params: { publicOnly }
    });
    return response.data;
  },

  // Get featured collections
  async getFeaturedCollections(type?: CollectionType): Promise<MediaCollectionDto[]> {
    const response = await apiClient.get('/api/media/collections/featured', {
      params: { type }
    });
    return response.data;
  },

  // Get public collections
  async getPublicCollections(offset = 0, limit = 50): Promise<MediaCollectionDto[]> {
    const response = await apiClient.get('/api/media/collections/public', {
      params: { offset, limit }
    });
    return response.data;
  },

  // Add media to collection
  async addMediaToCollection(collectionId: number, mediaId: number): Promise<boolean> {
    const response = await apiClient.post(`/api/media/collections/${collectionId}/media`, {
      mediaId
    });
    return response.data;
  },

  // Remove media from collection
  async removeMediaFromCollection(collectionId: number, mediaId: number): Promise<boolean> {
    const response = await apiClient.delete(`/api/media/collections/${collectionId}/media/${mediaId}`);
    return response.data;
  },

  // Get collection items
  async getCollectionItems(collectionId: number, offset = 0, limit = 100): Promise<any[]> {
    const response = await apiClient.get(`/api/media/collections/${collectionId}/items`, {
      params: { offset, limit }
    });
    return response.data;
  }
};

// Media Thumbnails API
export const mediaThumbnailsApi = {
  // Get thumbnails for media file
  async getThumbnails(mediaFileId: number): Promise<MediaThumbnail[]> {
    const response = await apiClient.get(`/api/media/${mediaFileId}/thumbnails`);
    return response.data;
  },

  // Get thumbnail by size
  async getThumbnailBySize(mediaFileId: number, size: number): Promise<MediaThumbnail | null> {
    const response = await apiClient.get(`/api/media/${mediaFileId}/thumbnails/size/${size}`);
    return response.data;
  },

  // Generate thumbnail
  async generateThumbnail(mediaFileId: number, size: number): Promise<MediaThumbnail> {
    const response = await apiClient.post(`/api/media/${mediaFileId}/thumbnails/generate`, {
      size
    });
    return response.data;
  },

  // Set default thumbnail
  async setDefaultThumbnail(thumbnailId: number): Promise<boolean> {
    const response = await apiClient.post(`/api/media/thumbnails/${thumbnailId}/set-default`);
    return response.data;
  }
};

// Media Search API
export const mediaSearchApi = {
  // Search media files
  async searchMedia(searchDto: MediaFileSearchDto): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.post('/api/media/search', searchDto);
    return response.data;
  },

  // Get search statistics
  async getSearchStatistics(): Promise<SearchStatistics> {
    const response = await apiClient.get('/api/media/search/statistics');
    return response.data;
  },

  // Get media by tags
  async getMediaByTags(tags: string[]): Promise<MediaFileDto[]> {
    const response = await apiClient.post('/api/media/search/tags', { tags });
    return response.data;
  },

  // Get media by date range
  async getMediaByDateRange(fromDate: string, toDate: string): Promise<MediaFileDto[]> {
    const response = await apiClient.get('/api/media/search/date-range', {
      params: { fromDate, toDate }
    });
    return response.data;
  },

  // Get media by size range
  async getMediaBySizeRange(minSize: number, maxSize: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get('/api/media/search/size-range', {
      params: { minSize, maxSize }
    });
    return response.data;
  }
};
