// Media Management Types
// Based on backend DTOs and entities

export interface MediaFile {
  id: number;
  fileName: string;
  originalFileName: string;
  filePath: string;
  fileUrl: string;
  fileSizeBytes: number;
  mimeType: string;
  mediaType: MediaType;
  status: MediaStatus;
  categoryId?: number;
  collectionId?: number;
  uploadedBy: number;
  uploadedAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  metadata?: MediaMetadata[];
  thumbnails?: MediaThumbnail[];
  accessLogs?: MediaAccessLog[];
}

export interface MediaCategory {
  id: number;
  name: string;
  description?: string;
  parentId?: number;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  children?: MediaCategory[];
  mediaCount?: number;
}

export interface MediaCollection {
  id: number;
  name: string;
  description?: string;
  collectionType: MediaCollectionType;
  isPublic: boolean;
  createdBy: number;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  items?: MediaCollectionItem[];
  itemCount?: number;
}

export interface MediaCollectionItem {
  id: number;
  collectionId: number;
  mediaFileId: number;
  sortOrder: number;
  caption?: string;
  isFeatured: boolean;
  createdAt: string;
  mediaFile?: MediaFile;
}

export interface MediaMetadata {
  id: number;
  mediaFileId: number;
  metadataKey: string;
  metadataValue: string;
  dataType: string;
  createdAt: string;
  modifiedAt: string;
}

export interface MediaThumbnail {
  id: number;
  mediaFileId: number;
  size: ThumbnailSize;
  width: number;
  height: number;
  format: string;
  filePath: string;
  fileUrl: string;
  fileSizeBytes: number;
  status: ThumbnailStatus;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaAccessLog {
  id: number;
  mediaFileId: number;
  userId: number;
  action: MediaAccessAction;
  ipAddress?: string;
  userAgent?: string;
  accessedAt: string;
  sessionId?: string;
  referrer?: string;
}

export interface MediaProcessingJob {
  id: number;
  mediaFileId: number;
  jobType: ProcessingJobType;
  status: ProcessingJobStatus;
  priority: number;
  progressPercentage: number;
  attemptCount: number;
  maxAttempts: number;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  jobParameters?: string;
  processingTimeMs?: number;
  nextRetryAt?: string;
  processedBy?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

// Enums
export enum MediaType {
  Image = 1,
  Video = 2,
  Audio = 3,
  Document = 4,
  Archive = 5,
  Other = 6
}

export enum MediaStatus {
  Pending = 1,
  Processing = 2,
  Ready = 3,
  Failed = 4,
  Deleted = 5
}

export enum MediaCollectionType {
  Album = 1,
  Playlist = 2,
  Gallery = 3,
  Portfolio = 4,
  Archive = 5
}

export enum ThumbnailSize {
  Small = 1,
  Medium = 2,
  Large = 3,
  ExtraLarge = 4,
  Custom = 5
}

export enum ThumbnailStatus {
  Pending = 1,
  Processing = 2,
  Ready = 3,
  Failed = 4
}

export enum MediaAccessAction {
  View = 1,
  Download = 2,
  Edit = 3,
  Delete = 4,
  Share = 5
}

export enum ProcessingJobType {
  ThumbnailGeneration = 1,
  VideoTranscoding = 2,
  AudioTranscoding = 3,
  ImageOptimization = 4,
  MetadataExtraction = 5,
  VirusScan = 6
}

export enum ProcessingJobStatus {
  Queued = 1,
  Processing = 2,
  Completed = 3,
  Failed = 4,
  Cancelled = 5
}

// DTOs for API requests/responses
export interface CreateMediaFileDto {
  fileName: string;
  originalFileName: string;
  filePath: string;
  fileSizeBytes: number;
  mimeType: string;
  mediaType: MediaType;
  categoryId?: number;
  collectionId?: number;
  metadata?: Record<string, any>;
}

export interface UpdateMediaFileDto {
  fileName?: string;
  categoryId?: number;
  collectionId?: number;
  status?: MediaStatus;
  metadata?: Record<string, any>;
}

export interface MediaFileSearchDto {
  searchTerm?: string;
  mediaType?: MediaType;
  categoryId?: number;
  collectionId?: number;
  status?: MediaStatus;
  uploadedBy?: number;
  dateFrom?: string;
  dateTo?: string;
  minSizeBytes?: number;
  maxSizeBytes?: number;
  tags?: string[];
  page: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface MediaFileDto {
  id: number;
  fileName: string;
  originalFileName: string;
  filePath: string;
  fileUrl: string;
  fileSizeBytes: number;
  mimeType: string;
  mediaType: MediaType;
  status: MediaStatus;
  categoryId?: number;
  categoryName?: string;
  collectionId?: number;
  collectionName?: string;
  uploadedBy: number;
  uploadedByUsername?: string;
  uploadedAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  metadata?: MediaMetadataDto[];
  thumbnails?: MediaThumbnailDto[];
  accessCount?: number;
  lastAccessedAt?: string;
}

export interface MediaMetadataDto {
  id: number;
  mediaFileId: number;
  metadataKey: string;
  metadataValue: string;
  dataType: string;
  createdAt: string;
  modifiedAt: string;
}

export interface MediaThumbnailDto {
  id: number;
  mediaFileId: number;
  size: ThumbnailSize;
  width: number;
  height: number;
  format: string;
  filePath: string;
  fileUrl: string;
  fileSizeBytes: number;
  status: ThumbnailStatus;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaCategoryDto {
  id: number;
  name: string;
  description?: string;
  parentId?: number;
  parentName?: string;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  children?: MediaCategoryDto[];
  mediaCount: number;
  level: number;
}

export interface MediaCollectionDto {
  id: number;
  name: string;
  description?: string;
  collectionType: MediaCollectionType;
  isPublic: boolean;
  createdBy: number;
  createdByUsername?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
  items?: MediaCollectionItemDto[];
  itemCount: number;
  coverImageUrl?: string;
}

export interface MediaCollectionItemDto {
  id: number;
  collectionId: number;
  mediaFileId: number;
  sortOrder: number;
  caption?: string;
  isFeatured: boolean;
  createdAt: string;
  mediaFile?: MediaFileDto;
}

export interface MediaProcessingJobDto {
  id: number;
  mediaFileId: number;
  mediaFileName?: string;
  jobType: ProcessingJobType;
  status: ProcessingJobStatus;
  priority: number;
  progressPercentage: number;
  attemptCount: number;
  maxAttempts: number;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  jobParameters?: string;
  processingTimeMs?: number;
  nextRetryAt?: string;
  processedBy?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

// Utility types
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface MediaUploadProgress {
  fileId: string;
  fileName: string;
  progress: number;
  status: 'uploading' | 'processing' | 'completed' | 'failed';
  error?: string;
}

export interface MediaFilterOptions {
  categories: MediaCategoryDto[];
  collections: MediaCollectionDto[];
  mediaTypes: { value: MediaType; label: string }[];
  statuses: { value: MediaStatus; label: string }[];
  dateRanges: { value: string; label: string }[];
}

export interface MediaSortOption {
  value: string;
  label: string;
  direction: 'asc' | 'desc';
}

export interface MediaViewMode {
  grid: 'grid';
  list: 'list';
  gallery: 'gallery';
  timeline: 'timeline';
}
