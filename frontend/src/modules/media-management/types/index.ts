// Media Management Types
// Based on backend DTOs and entities

export interface MediaFile {
  id: number;
  originalFileName: string;
  storageFileName: string;
  contentType: string;
  fileSizeBytes: number;
  storagePath: string;
  mediaType: MediaType;
  status: MediaStatus;
  title?: string;
  description?: string;
  altText?: string;
  fileHash?: string;
  categoryId?: number;
  uploadedBy: number;
  isPublic: boolean;
  accessCount: number;
  lastAccessedAt?: string;
  metadata?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaFileDto extends MediaFile {
  // Additional properties for DTOs
  category?: MediaCategory;
  thumbnails?: MediaThumbnail[];
  accessLogs?: MediaAccessLog[];
  collections?: MediaCollection[];
  processingJobs?: MediaProcessingJob[];
}

export interface CreateMediaFileDto {
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
  mediaType: MediaType;
  title?: string;
  description?: string;
  altText?: string;
  categoryId?: number;
  isPublic?: boolean;
  metadata?: string;
}

export interface UpdateMediaFileDto {
  title?: string;
  description?: string;
  altText?: string;
  categoryId?: number;
  isPublic?: boolean;
  metadata?: string;
}

export interface MediaFileSearchDto {
  searchTerm?: string;
  mediaType?: MediaType;
  categoryId?: number;
  status?: MediaStatus;
  uploadedByUserId?: number;
  createdAfter?: string;
  createdBefore?: string;
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface MediaCategory {
  id: number;
  name: string;
  description: string;
  icon?: string;
  color?: string;
  parentId?: number;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaCategoryDto extends MediaCategory {
  parent?: MediaCategoryDto;
  children: MediaCategoryDto[];
  mediaFileCount: number;
}

export interface CreateMediaCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  parentId?: number;
  sortOrder?: number;
  isActive?: boolean;
}

export interface UpdateMediaCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  parentId?: number;
  sortOrder?: number;
  isActive?: boolean;
}

export interface MediaCollection {
  id: number;
  name: string;
  description: string;
  slug: string;
  collectionType: CollectionType;
  isPublic: boolean;
  isFeatured: boolean;
  viewCount: number;
  tags?: string;
  createdByUserId: number;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaCollectionDto extends MediaCollection {
  // Additional properties for DTOs
  mediaFiles?: MediaFile[];
  collectionItems?: MediaCollectionItem[];
}

export interface CreateMediaCollectionDto {
  name: string;
  description?: string;
  slug: string;
  collectionType: CollectionType;
  isPublic?: boolean;
  isFeatured?: boolean;
  tags?: string;
}

export interface UpdateMediaCollectionDto {
  name?: string;
  description?: string;
  slug?: string;
  collectionType?: CollectionType;
  isPublic?: boolean;
  isFeatured?: boolean;
  tags?: string;
}

export interface MediaCollectionItem {
  id: number;
  collectionId: number;
  mediaFileId: number;
  sortOrder: number;
  caption?: string;
  isFeatured: boolean;
  createdAt: string;
}

export interface MediaMetadata {
  id: number;
  mediaFileId: number;
  metadataKey: string;
  metadataValue: string;
  dataType: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaThumbnail {
  id: number;
  mediaFileId: number;
  size: ThumbnailSize;
  width: number;
  height: number;
  format: string;
  fileSizeBytes: number;
  storagePath: string;
  url?: string;
  isDefault: boolean;
  status: ThumbnailStatus;
  generationMethod: ThumbnailGenerationMethod;
  generationTimeMs?: number;
  errorMessage?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaAccessLog {
  id: number;
  mediaFileId: number;
  userId?: number;
  accessType: AccessType;
  ipAddress?: string;
  userAgent?: string;
  referrer?: string;
  sessionId?: string;
  geoLocation?: string;
  accessContext?: string;
  duration?: string;
  bytesTransferred?: number;
  statusCode?: number;
  success: boolean;
  errorMessage?: string;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface MediaAccessLogDto {
  mediaFileId: number;
  userId?: number;
  action: MediaAccessAction;
  accessedAt: string;
  ipAddress?: string;
  userAgent?: string;
}

export interface MediaProcessingJob {
  id: number;
  mediaFileId: number;
  jobType: ProcessingJobType;
  status: ProcessingJobStatus;
  priority: ProcessingJobPriority;
  parameters?: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  progress?: number;
  createdAt: string;
  modifiedAt: string;
  isDeleted: boolean;
}

export interface BulkMediaFileOperationDto {
  mediaFileIds: number[];
  operation: string; // "delete", "move", "categorize", "status_change"
  targetCategoryId?: number;
  targetStatus?: MediaFileStatus;
  tags?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface SearchStatistics {
  totalMediaFiles: number;
  totalCategories: number;
  totalCollections: number;
  lastUpdated: string;
}

// Enums
export enum MediaType {
  Image = 1,
  Video = 2,
  Audio = 3,
  Document = 4,
  Archive = 5,
  Other = 99
}

export enum MediaStatus {
  Uploading = 0,
  Processing = 1,
  Ready = 2,
  Failed = 3,
  Deleted = 4
}

export enum MediaFileStatus {
  Uploading = 0,
  Processing = 1,
  Ready = 2,
  Failed = 3,
  Deleted = 4
}

export enum CollectionType {
  Album = 1,
  Gallery = 2,
  Portfolio = 3,
  Archive = 4,
  Custom = 99
}

export enum ThumbnailSize {
  Small = 1,    // 150x150
  Medium = 2,   // 300x300
  Large = 3,    // 600x600
  ExtraLarge = 4, // 1200x1200
  Custom = 99
}

export enum ThumbnailStatus {
  Pending = 0,
  Generating = 1,
  Ready = 2,
  Failed = 3
}

export enum ThumbnailGenerationMethod {
  Resize = 1,
  Crop = 2,
  Fit = 3,
  Custom = 99
}

export enum AccessType {
  View = 1,
  Download = 2,
  Edit = 3,
  Delete = 4
}

export enum MediaAccessAction {
  View = 1,
  Download = 2,
  Edit = 3,
  Delete = 4
}

export enum ProcessingJobType {
  ThumbnailGeneration = 1,
  MetadataExtraction = 2,
  FormatConversion = 3,
  Optimization = 4,
  VirusScan = 5,
  Custom = 99
}

export enum ProcessingJobStatus {
  Pending = 0,
  Running = 1,
  Completed = 2,
  Failed = 3,
  Cancelled = 4
}

export enum ProcessingJobPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Critical = 4
}

// Utility types
export type MediaFileFilter = {
  searchTerm?: string;
  mediaType?: MediaType;
  categoryId?: number;
  status?: MediaStatus;
  uploadedBy?: number;
  dateRange?: {
    from: string;
    to: string;
  };
  sizeRange?: {
    min: number;
    max: number;
  };
};

export type MediaSortOption = {
  field: keyof MediaFile;
  direction: 'asc' | 'desc';
};

export type MediaUploadProgress = {
  fileId: string;
  fileName: string;
  progress: number;
  status: 'uploading' | 'processing' | 'completed' | 'error';
  error?: string;
};
