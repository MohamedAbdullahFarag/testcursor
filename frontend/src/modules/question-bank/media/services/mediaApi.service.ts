import { apiClient } from '@/shared/services/apiClient';
import {
  MediaFileDto,
  CreateMediaFileDto,
  UpdateMediaFileDto,
  MediaFileSearchDto,
  MediaCategoryDto,
  MediaCollectionDto,
  MediaProcessingJobDto,
  PagedResult,
  MediaType,
  MediaStatus,
  ProcessingJobType,
  ProcessingJobStatus
} from '../types/media.types';

export class MediaApiService {
  private readonly baseUrl = '/api/media';

  // Media File Operations
  async getMediaFiles(searchDto: MediaFileSearchDto): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.post<PagedResult<MediaFileDto>>(
      `${this.baseUrl}/search`,
      searchDto
    );
    return response.data;
  }

  async getMediaFileById(id: number): Promise<MediaFileDto> {
    const response = await apiClient.get<MediaFileDto>(`${this.baseUrl}/files/${id}`);
    return response.data;
  }

  async createMediaFile(createDto: CreateMediaFileDto): Promise<MediaFileDto> {
    const response = await apiClient.post<MediaFileDto>(`${this.baseUrl}/files`, createDto);
    return response.data;
  }

  async updateMediaFile(id: number, updateDto: UpdateMediaFileDto): Promise<MediaFileDto> {
    const response = await apiClient.put<MediaFileDto>(`${this.baseUrl}/files/${id}`, updateDto);
    return response.data;
  }

  async deleteMediaFile(id: number): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/files/${id}`);
  }

  async uploadMediaFile(file: File, metadata?: Record<string, any>): Promise<MediaFileDto> {
    const formData = new FormData();
    formData.append('file', file);
    
    if (metadata) {
      formData.append('metadata', JSON.stringify(metadata));
    }

    const response = await apiClient.post<MediaFileDto>(
      `${this.baseUrl}/upload`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  }

  async downloadMediaFile(id: number): Promise<Blob> {
    const response = await apiClient.get(`${this.baseUrl}/files/${id}/download`, {
      responseType: 'blob',
    });
    return response.data;
  }

  async getMediaFileThumbnail(id: number, size: number): Promise<string> {
    const response = await apiClient.get(`${this.baseUrl}/files/${id}/thumbnail/${size}`, {
      responseType: 'blob',
    });
    return URL.createObjectURL(response.data);
  }

  // Media Category Operations
  async getCategories(): Promise<MediaCategoryDto[]> {
    const response = await apiClient.get<MediaCategoryDto[]>(`${this.baseUrl}/categories`);
    return response.data;
  }

  async getCategoryById(id: number): Promise<MediaCategoryDto> {
    const response = await apiClient.get<MediaCategoryDto>(`${this.baseUrl}/categories/${id}`);
    return response.data;
  }

  async createCategory(category: Partial<MediaCategoryDto>): Promise<MediaCategoryDto> {
    const response = await apiClient.post<MediaCategoryDto>(`${this.baseUrl}/categories`, category);
    return response.data;
  }

  async updateCategory(id: number, category: Partial<MediaCategoryDto>): Promise<MediaCategoryDto> {
    const response = await apiClient.put<MediaCategoryDto>(`${this.baseUrl}/categories/${id}`, category);
    return response.data;
  }

  async deleteCategory(id: number): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/categories/${id}`);
  }

  async getMediaByCategory(categoryId: number, page = 1, pageSize = 20): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.get<PagedResult<MediaFileDto>>(
      `${this.baseUrl}/categories/${categoryId}/media`,
      {
        params: { page, pageSize }
      }
    );
    return response.data;
  }

  // Media Collection Operations
  async getCollections(): Promise<MediaCollectionDto[]> {
    const response = await apiClient.get<MediaCollectionDto[]>(`${this.baseUrl}/collections`);
    return response.data;
  }

  async getCollectionById(id: number): Promise<MediaCollectionDto> {
    const response = await apiClient.get<MediaCollectionDto>(`${this.baseUrl}/collections/${id}`);
    return response.data;
  }

  async createCollection(collection: Partial<MediaCollectionDto>): Promise<MediaCollectionDto> {
    const response = await apiClient.post<MediaCollectionDto>(`${this.baseUrl}/collections`, collection);
    return response.data;
  }

  async updateCollection(id: number, collection: Partial<MediaCollectionDto>): Promise<MediaCollectionDto> {
    const response = await apiClient.put<MediaCollectionDto>(`${this.baseUrl}/collections/${id}`, collection);
    return response.data;
  }

  async deleteCollection(id: number): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/collections/${id}`);
  }

  async getMediaByCollection(collectionId: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/collections/${collectionId}/media`);
    return response.data;
  }

  async addMediaToCollection(collectionId: number, mediaFileId: number, sortOrder?: number): Promise<void> {
    await apiClient.post(`${this.baseUrl}/collections/${collectionId}/media`, {
      mediaFileId,
      sortOrder: sortOrder || 0
    });
  }

  async removeMediaFromCollection(collectionId: number, mediaFileId: number): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/collections/${collectionId}/media/${mediaFileId}`);
  }

  // Media Processing Job Operations
  async getProcessingJobs(
    status?: ProcessingJobStatus,
    jobType?: ProcessingJobType,
    limit = 100
  ): Promise<MediaProcessingJobDto[]> {
    const params: any = { limit };
    if (status) params.status = status;
    if (jobType) params.jobType = jobType;

    const response = await apiClient.get<MediaProcessingJobDto[]>(`${this.baseUrl}/jobs`, { params });
    return response.data;
  }

  async getProcessingJobById(id: number): Promise<MediaProcessingJobDto> {
    const response = await apiClient.get<MediaProcessingJobDto>(`${this.baseUrl}/jobs/${id}`);
    return response.data;
  }

  async createProcessingJob(
    mediaFileId: number,
    jobType: ProcessingJobType,
    priority = 5,
    parameters?: Record<string, any>
  ): Promise<MediaProcessingJobDto> {
    const response = await apiClient.post<MediaProcessingJobDto>(`${this.baseUrl}/jobs`, {
      mediaFileId,
      jobType,
      priority,
      jobParameters: parameters ? JSON.stringify(parameters) : undefined
    });
    return response.data;
  }

  async cancelProcessingJob(id: number, reason?: string): Promise<void> {
    await apiClient.post(`${this.baseUrl}/jobs/${id}/cancel`, { reason });
  }

  async retryProcessingJob(id: number): Promise<void> {
    await apiClient.post(`${this.baseUrl}/jobs/${id}/retry`);
  }

  // Media Search Operations
  async searchMedia(searchDto: MediaFileSearchDto): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.post<PagedResult<MediaFileDto>>(
      `${this.baseUrl}/search`,
      searchDto
    );
    return response.data;
  }

  async getSimilarMedia(mediaId: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/files/${mediaId}/similar`);
    return response.data;
  }

  async getRecentMedia(count = 10, userId?: number): Promise<MediaFileDto[]> {
    const params: any = { count };
    if (userId) params.userId = userId;

    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/files/recent`, { params });
    return response.data;
  }

  async getMediaByTags(tags: string[]): Promise<MediaFileDto[]> {
    const response = await apiClient.post<MediaFileDto[]>(`${this.baseUrl}/files/by-tags`, { tags });
    return response.data;
  }

  async getMediaByType(mediaType: MediaType, page = 1, pageSize = 20): Promise<PagedResult<MediaFileDto>> {
    const response = await apiClient.get<PagedResult<MediaFileDto>>(
      `${this.baseUrl}/files/by-type/${mediaType}`,
      {
        params: { page, pageSize }
      }
    );
    return response.data;
  }

  async getMediaByDateRange(fromDate: string, toDate: string): Promise<MediaFileDto[]> {
    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/files/by-date-range`, {
      params: { fromDate, toDate }
    });
    return response.data;
  }

  async getMediaBySizeRange(minSizeBytes: number, maxSizeBytes: number): Promise<MediaFileDto[]> {
    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/files/by-size-range`, {
      params: { minSizeBytes, maxSizeBytes }
    });
    return response.data;
  }

  async getPopularMedia(count = 10, timeRange?: string): Promise<MediaFileDto[]> {
    const params: any = { count };
    if (timeRange) params.timeRange = timeRange;

    const response = await apiClient.get<MediaFileDto[]>(`${this.baseUrl}/files/popular`, { params });
    return response.data;
  }

  // Media Statistics
  async getMediaStats(): Promise<{
    totalFiles: number;
    totalSize: number;
    byType: Record<MediaType, number>;
    byStatus: Record<MediaStatus, number>;
    recentUploads: number;
  }> {
    const response = await apiClient.get(`${this.baseUrl}/stats`);
    return response.data;
  }

  async getProcessingJobStats(): Promise<{
    byStatus: Record<ProcessingJobStatus, number>;
    byType: Record<ProcessingJobType, number>;
    averageProcessingTime: Record<ProcessingJobType, number>;
    throughput: Record<string, number>;
  }> {
    const response = await apiClient.get(`${this.baseUrl}/jobs/stats`);
    return response.data;
  }

  // Media Access Logging
  async logMediaAccess(
    mediaFileId: number,
    action: string,
    sessionId?: string,
    referrer?: string
  ): Promise<void> {
    await apiClient.post(`${this.baseUrl}/files/${mediaFileId}/access-log`, {
      action,
      sessionId,
      referrer
    });
  }

  async getMediaAccessLogs(mediaFileId: number, limit = 100): Promise<any[]> {
    const response = await apiClient.get(`${this.baseUrl}/files/${mediaFileId}/access-logs`, {
      params: { limit }
    });
    return response.data;
  }

  // Utility Methods
  async getMediaFileUrl(id: number): Promise<string> {
    const response = await apiClient.get(`${this.baseUrl}/files/${id}/url`);
    return response.data.url;
  }

  async validateFile(file: File): Promise<{
    isValid: boolean;
    errors: string[];
    warnings: string[];
    suggestedMetadata?: Record<string, any>;
  }> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post(`${this.baseUrl}/validate`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  }

  async getSupportedFormats(): Promise<{
    images: string[];
    videos: string[];
    audio: string[];
    documents: string[];
    archives: string[];
  }> {
    const response = await apiClient.get(`${this.baseUrl}/supported-formats`);
    return response.data;
  }

  async getUploadLimits(): Promise<{
    maxFileSize: number;
    maxFilesPerUpload: number;
    allowedTypes: string[];
    maxConcurrentUploads: number;
  }> {
    const response = await apiClient.get(`${this.baseUrl}/upload-limits`);
    return response.data;
  }
}

// Export singleton instance
export const mediaApiService = new MediaApiService();

