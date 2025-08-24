import { Image, Video, Music, FileText, Archive, File } from 'lucide-react';
import { MediaType } from '../types';

/**
 * Format file size in bytes to human readable format
 */
export const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes';

  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

/**
 * Format date string to human readable format
 */
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInHours = Math.abs(now.getTime() - date.getTime()) / (1000 * 60 * 60);

  if (diffInHours < 24) {
    if (diffInHours < 1) {
      const diffInMinutes = Math.floor(diffInHours * 60);
      if (diffInMinutes < 1) {
        return 'Just now';
      }
      return `${diffInMinutes} minute${diffInMinutes > 1 ? 's' : ''} ago`;
    }
    return `${Math.floor(diffInHours)} hour${Math.floor(diffInHours) > 1 ? 's' : ''} ago`;
  } else if (diffInHours < 168) { // 7 days
    const diffInDays = Math.floor(diffInHours / 24);
    return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`;
  } else {
    return date.toLocaleDateString();
  }
};

/**
 * Get media type icon component based on MediaType enum
 */
export const getMediaTypeIcon = (mediaType: MediaType, className: string = 'w-6 h-6') => {
  switch (mediaType) {
    case MediaType.Image:
      return { component: Image, className };
    case MediaType.Video:
      return { component: Video, className };
    case MediaType.Audio:
      return { component: Music, className };
    case MediaType.Document:
      return { component: FileText, className };
    case MediaType.Archive:
      return { component: Archive, className };
    default:
      return { component: File, className };
  }
};

/**
 * Get media type label
 */
export const getMediaTypeLabel = (mediaType: MediaType): string => {
  switch (mediaType) {
    case MediaType.Image:
      return 'Image';
    case MediaType.Video:
      return 'Video';
    case MediaType.Audio:
      return 'Audio';
    case MediaType.Document:
      return 'Document';
    case MediaType.Archive:
      return 'Archive';
    default:
      return 'Other';
  }
};

/**
 * Get file extension from filename
 */
export const getFileExtension = (filename: string): string => {
  return filename.slice((filename.lastIndexOf('.') - 1 >>> 0) + 2);
};

/**
 * Check if file is an image
 */
export const isImageFile = (filename: string): boolean => {
  const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'];
  const extension = getFileExtension(filename).toLowerCase();
  return imageExtensions.includes(extension);
};

/**
 * Check if file is a video
 */
export const isVideoFile = (filename: string): boolean => {
  const videoExtensions = ['mp4', 'avi', 'mov', 'wmv', 'flv', 'webm', 'mkv'];
  const extension = getFileExtension(filename).toLowerCase();
  return videoExtensions.includes(extension);
};

/**
 * Check if file is an audio file
 */
export const isAudioFile = (filename: string): boolean => {
  const audioExtensions = ['mp3', 'wav', 'flac', 'aac', 'ogg', 'wma'];
  const extension = getFileExtension(filename).toLowerCase();
  return audioExtensions.includes(extension);
};

/**
 * Check if file is a document
 */
export const isDocumentFile = (filename: string): boolean => {
  const documentExtensions = ['pdf', 'doc', 'docx', 'txt', 'rtf', 'odt'];
  const extension = getFileExtension(filename).toLowerCase();
  return documentExtensions.includes(extension);
};

/**
 * Check if file is an archive
 */
export const isArchiveFile = (filename: string): boolean => {
  const archiveExtensions = ['zip', 'rar', '7z', 'tar', 'gz', 'bz2'];
  const extension = getFileExtension(filename).toLowerCase();
  return archiveExtensions.includes(extension);
};

/**
 * Get MIME type from file extension
 */
export const getMimeType = (filename: string): string => {
  const extension = getFileExtension(filename).toLowerCase();
  
  const mimeTypes: Record<string, string> = {
    // Images
    'jpg': 'image/jpeg',
    'jpeg': 'image/jpeg',
    'png': 'image/png',
    'gif': 'image/gif',
    'bmp': 'image/bmp',
    'webp': 'image/webp',
    'svg': 'image/svg+xml',
    
    // Videos
    'mp4': 'video/mp4',
    'avi': 'video/x-msvideo',
    'mov': 'video/quicktime',
    'wmv': 'video/x-ms-wmv',
    'flv': 'video/x-flv',
    'webm': 'video/webm',
    'mkv': 'video/x-matroska',
    
    // Audio
    'mp3': 'audio/mpeg',
    'wav': 'audio/wav',
    'flac': 'audio/flac',
    'aac': 'audio/aac',
    'ogg': 'audio/ogg',
    'wma': 'audio/x-ms-wma',
    
    // Documents
    'pdf': 'application/pdf',
    'doc': 'application/msword',
    'docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'txt': 'text/plain',
    'rtf': 'application/rtf',
    'odt': 'application/vnd.oasis.opendocument.text',
    
    // Archives
    'zip': 'application/zip',
    'rar': 'application/x-rar-compressed',
    '7z': 'application/x-7z-compressed',
    'tar': 'application/x-tar',
    'gz': 'application/gzip',
    'bz2': 'application/x-bzip2'
  };
  
  return mimeTypes[extension] || 'application/octet-stream';
};

/**
 * Validate file size
 */
export const validateFileSize = (file: File, maxSizeMB: number): boolean => {
  const maxSizeBytes = maxSizeMB * 1024 * 1024;
  return file.size <= maxSizeBytes;
};

/**
 * Validate file type
 */
export const validateFileType = (file: File, allowedTypes: string[]): boolean => {
  return allowedTypes.includes(file.type);
};

/**
 * Generate thumbnail URL for media file
 */
export const getThumbnailUrl = (mediaId: number, size: string = 'medium'): string => {
  return `/api/media/${mediaId}/thumbnail?size=${size}`;
};

/**
 * Generate download URL for media file
 */
export const getDownloadUrl = (mediaId: number): string => {
  return `/api/media/${mediaId}/download`;
};

/**
 * Generate preview URL for media file
 */
export const getPreviewUrl = (mediaId: number): string => {
  return `/api/media/${mediaId}/preview`;
};

/**
 * Format duration in seconds to human readable format
 */
export const formatDuration = (seconds: number): string => {
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;

  if (hours > 0) {
    return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  } else {
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  }
};

/**
 * Get color for media type
 */
export const getMediaTypeColor = (mediaType: MediaType): string => {
  switch (mediaType) {
    case MediaType.Image:
      return 'text-blue-600 bg-blue-100 dark:bg-blue-900/20';
    case MediaType.Video:
      return 'text-purple-600 bg-purple-100 dark:bg-purple-900/20';
    case MediaType.Audio:
      return 'text-green-600 bg-green-100 dark:bg-green-900/20';
    case MediaType.Document:
      return 'text-orange-600 bg-orange-100 dark:bg-orange-900/20';
    case MediaType.Archive:
      return 'text-gray-600 bg-gray-100 dark:bg-gray-900/20';
    default:
      return 'text-gray-600 bg-gray-100 dark:bg-gray-900/20';
  }
};

/**
 * Truncate text to specified length
 */
export const truncateText = (text: string, maxLength: number): string => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
};

/**
 * Generate slug from text
 */
export const generateSlug = (text: string): string => {
  return text
    .toLowerCase()
    .replace(/[^a-z0-9 -]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim();
};

/**
 * Debounce function
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
 * Throttle function
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
