// Media Management Module
// Main exports for the media management system

// Components
export { MediaManagementPage } from './components/MediaManagementPage';
export { MediaLibrary } from './components/MediaLibrary';

// Hooks
export { 
  useMediaFiles,
  useMediaFile,
  useMediaFilesByCategory,
  useMediaFilesByCollection,
  useMediaFilesByUser,
  useMediaFilesByType,
  useRecentMediaFiles,
  usePopularMediaFiles,
  useSimilarMediaFiles,
  useCreateMediaFile,
  useUpdateMediaFile,
  useDeleteMediaFile,
  useUploadMediaFile,
  useBulkDeleteMedia,
  useBulkMoveMedia,
  useBulkUpdateMedia,
  useLogMediaAccess,
  mediaFilesKeys
} from './hooks/useMediaFiles';

// Services
export {
  mediaFilesApi,
  mediaCategoriesApi,
  mediaCollectionsApi,
  mediaThumbnailsApi,
  mediaSearchApi
} from './services/mediaApi';

// Types
export type {
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
  MediaCollectionItem,
  MediaMetadata,
  MediaThumbnail,
  MediaAccessLog,
  MediaAccessLogDto,
  MediaProcessingJob,
  BulkMediaFileOperationDto,
  PagedResult,
  SearchStatistics,
  MediaFileFilter,
  MediaSortOption,
  MediaUploadProgress
} from './types';

export {
  MediaType,
  MediaStatus,
  MediaFileStatus,
  CollectionType,
  ThumbnailSize,
  ThumbnailStatus,
  ThumbnailGenerationMethod,
  AccessType,
  MediaAccessAction,
  ProcessingJobType,
  ProcessingJobStatus,
  ProcessingJobPriority
} from './types';

// Utils
export {
  formatFileSize,
  formatDate,
  getMediaTypeIcon,
  getMediaTypeLabel,
  getFileExtension,
  isImageFile,
  isVideoFile,
  isAudioFile,
  isDocumentFile,
  isArchiveFile,
  getMimeType,
  validateFileSize,
  validateFileType,
  getThumbnailUrl,
  getDownloadUrl,
  getPreviewUrl,
  formatDuration,
  getMediaTypeColor,
  truncateText,
  generateSlug,
  debounce,
  throttle
} from './utils/mediaUtils';

// Constants
export * from './constants';

// Locales
export { default as en } from './locales/en.json';
export { default as ar } from './locales/ar.json';
