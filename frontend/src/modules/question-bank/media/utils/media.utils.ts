// Media Management Utilities
// Helper functions for common media management operations

import { MediaFileDto, MediaType, MediaStatus, ThumbnailSize } from '../types/media.types';
import { MEDIA_CONSTANTS } from '../constants/media.constants';

/**
 * Format file size in human-readable format
 */
export const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 B';

  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
};

/**
 * Get file extension from filename
 */
export const getFileExtension = (filename: string): string => {
  return filename.slice((filename.lastIndexOf('.') - 1 >>> 0) + 2);
};

/**
 * Get MIME type from file extension
 */
export const getMimeTypeFromExtension = (extension: string): string => {
  const ext = extension.toLowerCase();
  
  const mimeTypes: Record<string, string> = {
    // Images
    'jpg': 'image/jpeg',
    'jpeg': 'image/jpeg',
    'png': 'image/png',
    'gif': 'image/gif',
    'bmp': 'image/bmp',
    'webp': 'image/webp',
    'svg': 'image/svg+xml',
    'tiff': 'image/tiff',
    'tif': 'image/tiff',
    
    // Videos
    'mp4': 'video/mp4',
    'avi': 'video/x-msvideo',
    'mov': 'video/quicktime',
    'wmv': 'video/x-ms-wmv',
    'flv': 'video/x-flv',
    'webm': 'video/webm',
    'mkv': 'video/x-matroska',
    'm4v': 'video/x-m4v',
    
    // Audio
    'mp3': 'audio/mpeg',
    'wav': 'audio/wav',
    'flac': 'audio/flac',
    'aac': 'audio/aac',
    'ogg': 'audio/ogg',
    'wma': 'audio/x-ms-wma',
    'm4a': 'audio/mp4',
    
    // Documents
    'pdf': 'application/pdf',
    'doc': 'application/msword',
    'docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'xls': 'application/vnd.ms-excel',
    'xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'ppt': 'application/vnd.ms-powerpoint',
    'pptx': 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    'txt': 'text/plain',
    'rtf': 'application/rtf',
    
    // Archives
    'zip': 'application/zip',
    'rar': 'application/vnd.rar',
    '7z': 'application/x-7z-compressed',
    'tar': 'application/x-tar',
    'gz': 'application/gzip',
    'bz2': 'application/x-bzip2',
  };

  return mimeTypes[ext] || 'application/octet-stream';
};

/**
 * Determine media type from MIME type
 */
export const getMediaTypeFromMimeType = (mimeType: string): MediaType => {
  if (mimeType.startsWith('image/')) return MediaType.Image;
  if (mimeType.startsWith('video/')) return MediaType.Video;
  if (mimeType.startsWith('audio/')) return MediaType.Audio;
  if (mimeType.startsWith('text/') || mimeType.includes('document') || mimeType.includes('pdf')) return MediaType.Document;
  if (mimeType.includes('zip') || mimeType.includes('rar') || mimeType.includes('tar') || mimeType.includes('gz')) return MediaType.Archive;
  return MediaType.Other;
};

/**
 * Determine media type from file extension
 */
export const getMediaTypeFromExtension = (extension: string): MediaType => {
  const ext = extension.toLowerCase();
  
  if (MEDIA_CONSTANTS.SUPPORTED_FORMATS.IMAGES.includes(`.${ext}`)) return MediaType.Image;
  if (MEDIA_CONSTANTS.SUPPORTED_FORMATS.VIDEOS.includes(`.${ext}`)) return MediaType.Video;
  if (MEDIA_CONSTANTS.SUPPORTED_FORMATS.AUDIO.includes(`.${ext}`)) return MediaType.Audio;
  if (MEDIA_CONSTANTS.SUPPORTED_FORMATS.DOCUMENTS.includes(`.${ext}`)) return MediaType.Document;
  if (MEDIA_CONSTANTS.SUPPORTED_FORMATS.ARCHIVES.includes(`.${ext}`)) return MediaType.Archive;
  
  return MediaType.Other;
};

/**
 * Check if file type is supported
 */
export const isFileTypeSupported = (filename: string): boolean => {
  const extension = getFileExtension(filename);
  const allSupportedFormats = [
    ...MEDIA_CONSTANTS.SUPPORTED_FORMATS.IMAGES,
    ...MEDIA_CONSTANTS.SUPPORTED_FORMATS.VIDEOS,
    ...MEDIA_CONSTANTS.SUPPORTED_FORMATS.AUDIO,
    ...MEDIA_CONSTANTS.SUPPORTED_FORMATS.DOCUMENTS,
    ...MEDIA_CONSTANTS.SUPPORTED_FORMATS.ARCHIVES,
  ];
  
  return allSupportedFormats.includes(`.${extension.toLowerCase()}`);
};

/**
 * Check if file size is within limits
 */
export const isFileSizeValid = (bytes: number): boolean => {
  return bytes <= MEDIA_CONSTANTS.UPLOAD.MAX_FILE_SIZE;
};

/**
 * Validate file for upload
 */
export const validateFileForUpload = (file: File): { isValid: boolean; errors: string[] } => {
  const errors: string[] = [];
  
  if (!isFileTypeSupported(file.name)) {
    errors.push('File type not supported');
  }
  
  if (!isFileSizeValid(file.size)) {
    errors.push(`File size exceeds maximum limit of ${formatFileSize(MEDIA_CONSTANTS.UPLOAD.MAX_FILE_SIZE)}`);
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
};

/**
 * Generate thumbnail URL for media file
 */
export const getThumbnailUrl = (mediaFile: MediaFileDto, size: ThumbnailSize): string => {
  const thumbnail = mediaFile.thumbnails?.find(t => t.size === size);
  return thumbnail?.fileUrl || mediaFile.fileUrl;
};

/**
 * Get media type icon
 */
export const getMediaTypeIcon = (mediaType: MediaType): string => {
  switch (mediaType) {
    case MediaType.Image:
      return '🖼️';
    case MediaType.Video:
      return '🎥';
    case MediaType.Audio:
      return '🎵';
    case MediaType.Document:
      return '📄';
    case MediaType.Archive:
      return '📦';
    default:
      return '📁';
  }
};

/**
 * Get status color for media file
 */
export const getStatusColor = (status: MediaStatus): string => {
  switch (status) {
    case MediaStatus.Pending:
      return 'text-yellow-600 bg-yellow-100';
    case MediaStatus.Processing:
      return 'text-blue-600 bg-blue-100';
    case MediaStatus.Ready:
      return 'text-green-600 bg-green-100';
    case MediaStatus.Failed:
      return 'text-red-600 bg-red-100';
    case MediaStatus.Deleted:
      return 'text-gray-600 bg-gray-100';
    default:
      return 'text-gray-600 bg-gray-100';
  }
};

/**
 * Get media type color
 */
export const getMediaTypeColor = (mediaType: MediaType): string => {
  switch (mediaType) {
    case MediaType.Image:
      return 'text-blue-600 bg-blue-100';
    case MediaType.Video:
      return 'text-red-600 bg-red-100';
    case MediaType.Audio:
      return 'text-green-600 bg-green-100';
    case MediaType.Document:
      return 'text-yellow-600 bg-yellow-100';
    case MediaType.Archive:
      return 'text-purple-600 bg-purple-100';
    default:
      return 'text-gray-600 bg-gray-100';
  }
};

/**
 * Format date for display
 */
export const formatDate = (dateString: string, includeTime: boolean = false): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInHours = (now.getTime() - date.getTime()) / (1000 * 60 * 60);
  
  if (diffInHours < 24) {
    if (diffInHours < 1) {
      const diffInMinutes = Math.floor(diffInHours * 60);
      return `${diffInMinutes} minute${diffInMinutes !== 1 ? 's' : ''} ago`;
    }
    return `${Math.floor(diffInHours)} hour${Math.floor(diffInHours) !== 1 ? 's' : ''} ago`;
  }
  
  if (diffInHours < 48) {
    return 'Yesterday';
  }
  
  if (diffInHours < 168) { // 7 days
    const days = Math.floor(diffInHours / 24);
    return `${days} day${days !== 1 ? 's' : ''} ago`;
  }
  
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    ...(includeTime && { hour: '2-digit', minute: '2-digit' })
  };
  
  return date.toLocaleDateString('en-US', options);
};

/**
 * Sort media files by various criteria
 */
export const sortMediaFiles = (
  files: MediaFileDto[],
  sortBy: string = 'uploadedAt',
  sortOrder: 'asc' | 'desc' = 'desc'
): MediaFileDto[] => {
  return [...files].sort((a, b) => {
    let aValue: any = a[sortBy as keyof MediaFileDto];
    let bValue: any = b[sortBy as keyof MediaFileDto];
    
    // Handle date sorting
    if (sortBy === 'uploadedAt' || sortBy === 'modifiedAt') {
      aValue = new Date(aValue).getTime();
      bValue = new Date(bValue).getTime();
    }
    
    // Handle string sorting
    if (typeof aValue === 'string' && typeof bValue === 'string') {
      aValue = aValue.toLowerCase();
      bValue = bValue.toLowerCase();
    }
    
    if (aValue < bValue) return sortOrder === 'asc' ? -1 : 1;
    if (aValue > bValue) return sortOrder === 'asc' ? 1 : -1;
    return 0;
  });
};

/**
 * Filter media files by various criteria
 */
export const filterMediaFiles = (
  files: MediaFileDto[],
  filters: {
    mediaType?: MediaType;
    status?: MediaStatus;
    categoryId?: number;
    collectionId?: number;
    searchTerm?: string;
    dateFrom?: string;
    dateTo?: string;
    minSize?: number;
    maxSize?: number;
  }
): MediaFileDto[] => {
  return files.filter(file => {
    // Media type filter
    if (filters.mediaType && file.mediaType !== filters.mediaType) return false;
    
    // Status filter
    if (filters.status && file.status !== filters.status) return false;
    
    // Category filter
    if (filters.categoryId && file.categoryId !== filters.categoryId) return false;
    
    // Collection filter
    if (filters.collectionId && file.collectionId !== filters.collectionId) return false;
    
    // Search term filter
    if (filters.searchTerm) {
      const searchLower = filters.searchTerm.toLowerCase();
      const matchesSearch = 
        file.fileName.toLowerCase().includes(searchLower) ||
        file.originalFileName.toLowerCase().includes(searchLower) ||
        file.mimeType.toLowerCase().includes(searchLower);
      if (!matchesSearch) return false;
    }
    
    // Date range filter
    if (filters.dateFrom) {
      const fileDate = new Date(file.uploadedAt);
      const fromDate = new Date(filters.dateFrom);
      if (fileDate < fromDate) return false;
    }
    
    if (filters.dateTo) {
      const fileDate = new Date(file.uploadedAt);
      const toDate = new Date(filters.dateTo);
      if (fileDate > toDate) return false;
    }
    
    // File size filter
    if (filters.minSize && file.fileSizeBytes < filters.minSize) return false;
    if (filters.maxSize && file.fileSizeBytes > filters.maxSize) return false;
    
    return true;
  });
};

/**
 * Group media files by category
 */
export const groupMediaFilesByCategory = (files: MediaFileDto[]): Record<string, MediaFileDto[]> => {
  return files.reduce((groups, file) => {
    const categoryName = file.categoryId ? `Category ${file.categoryId}` : 'Uncategorized';
    if (!groups[categoryName]) {
      groups[categoryName] = [];
    }
    groups[categoryName].push(file);
    return groups;
  }, {} as Record<string, MediaFileDto[]>);
};

/**
 * Group media files by media type
 */
export const groupMediaFilesByType = (files: MediaFileDto[]): Record<string, MediaFileDto[]> => {
  return files.reduce((groups, file) => {
    const typeName = getMediaTypeFromMimeType(file.mimeType).toString();
    if (!groups[typeName]) {
      groups[typeName] = [];
    }
    groups[typeName].push(file);
    return groups;
  }, {} as Record<string, MediaFileDto[]>);
};

/**
 * Calculate total size of media files
 */
export const calculateTotalSize = (files: MediaFileDto[]): number => {
  return files.reduce((total, file) => total + file.fileSizeBytes, 0);
};

/**
 * Get unique file extensions from media files
 */
export const getUniqueFileExtensions = (files: MediaFileDto[]): string[] => {
  const extensions = files.map(file => getFileExtension(file.fileName));
  return [...new Set(extensions)];
};

/**
 * Check if media file has thumbnails
 */
export const hasThumbnails = (mediaFile: MediaFileDto): boolean => {
  return mediaFile.thumbnails && mediaFile.thumbnails.length > 0;
};

/**
 * Get best available thumbnail URL
 */
export const getBestThumbnailUrl = (mediaFile: MediaFileDto): string => {
  if (!hasThumbnails(mediaFile)) {
    return mediaFile.fileUrl;
  }
  
  // Prefer medium thumbnail, fallback to others
  const mediumThumbnail = mediaFile.thumbnails?.find(t => t.size === ThumbnailSize.Medium);
  if (mediumThumbnail) return mediumThumbnail.fileUrl;
  
  const largeThumbnail = mediaFile.thumbnails?.find(t => t.size === ThumbnailSize.Large);
  if (largeThumbnail) return largeThumbnail.fileUrl;
  
  const smallThumbnail = mediaFile.thumbnails?.find(t => t.size === ThumbnailSize.Small);
  if (smallThumbnail) return smallThumbnail.fileUrl;
  
  return mediaFile.fileUrl;
};

/**
 * Generate a unique filename
 */
export const generateUniqueFilename = (originalName: string, timestamp: number = Date.now()): string => {
  const extension = getFileExtension(originalName);
  const nameWithoutExt = originalName.replace(`.${extension}`, '');
  const sanitizedName = nameWithoutExt.replace(/[^a-zA-Z0-9]/g, '_');
  return `${sanitizedName}_${timestamp}.${extension}`;
};

/**
 * Debounce function for search input
 */
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout;
  return (...args: Parameters<T>) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
};

/**
 * Throttle function for scroll events
 */
export const throttle = <T extends (...args: any[]) => any>(
  func: T,
  limit: number
): ((...args: Parameters<T>) => void) => {
  let inThrottle: boolean;
  return (...args: Parameters<T>) => {
    if (!inThrottle) {
      func(...args);
      inThrottle = true;
      setTimeout(() => inThrottle = false, limit);
    }
  };
};

