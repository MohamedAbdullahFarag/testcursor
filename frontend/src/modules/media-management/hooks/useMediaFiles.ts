import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import { mediaFilesApi } from '../services/mediaApi';
import {
  MediaFileDto,
  CreateMediaFileDto,
  UpdateMediaFileDto,
  MediaFileSearchDto,
  PagedResult,
  MediaType,
  MediaFileStatus
} from '../types';

// Query keys
export const mediaFilesKeys = {
  all: ['media-files'] as const,
  lists: () => [...mediaFilesKeys.all, 'list'] as const,
  list: (filters: MediaFileSearchDto) => [...mediaFilesKeys.lists(), filters] as const,
  details: () => [...mediaFilesKeys.all, 'detail'] as const,
  detail: (id: number) => [...mediaFilesKeys.details(), id] as const,
  byCategory: (categoryId: number) => [...mediaFilesKeys.all, 'category', categoryId] as const,
  byCollection: (collectionId: number) => [...mediaFilesKeys.all, 'collection', collectionId] as const,
  byUser: (userId: number) => [...mediaFilesKeys.all, 'user', userId] as const,
  byType: (mediaType: MediaType) => [...mediaFilesKeys.all, 'type', mediaType] as const,
  recent: (count: number, userId?: number) => [...mediaFilesKeys.all, 'recent', count, userId] as const,
  popular: (count: number, timeRange?: string) => [...mediaFilesKeys.all, 'popular', count, timeRange] as const,
  similar: (mediaId: number) => [...mediaFilesKeys.all, 'similar', mediaId] as const,
};

// Hook for getting media files with search and pagination
export const useMediaFiles = (searchParams: MediaFileSearchDto) => {
  return useQuery({
    queryKey: mediaFilesKeys.list(searchParams),
    queryFn: async (): Promise<PagedResult<MediaFileDto>> => {
      try {
        const result = await mediaFilesApi.getMediaFiles(searchParams);
        // Ensure we return a valid PagedResult structure
        if (!result) {
          return {
            items: [],
            totalCount: 0,
            pageNumber: searchParams.page || 1,
            pageSize: searchParams.pageSize || 10,
          };
        }
        return result;
      } catch (error) {
        console.error('Error fetching media files:', error);
        // Return empty result structure instead of undefined
        return {
          items: [],
          totalCount: 0,
          pageNumber: searchParams.page || 1,
          pageSize: searchParams.pageSize || 10,
        };
      }
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
  });
};

// Hook for getting a single media file
export const useMediaFile = (id: number) => {
  return useQuery({
    queryKey: mediaFilesKeys.detail(id),
    queryFn: () => mediaFilesApi.getMediaFile(id),
    enabled: !!id,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for getting media files by category
export const useMediaFilesByCategory = (categoryId: number, page = 1, pageSize = 20) => {
  return useQuery({
    queryKey: mediaFilesKeys.byCategory(categoryId),
    queryFn: () => mediaFilesApi.getMediaFilesByCategory(categoryId, page, pageSize),
    enabled: !!categoryId,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for getting media files by collection
export const useMediaFilesByCollection = (collectionId: number) => {
  return useQuery({
    queryKey: mediaFilesKeys.byCollection(collectionId),
    queryFn: () => mediaFilesApi.getMediaFilesByCollection(collectionId),
    enabled: !!collectionId,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for getting media files by user
export const useMediaFilesByUser = (userId: number, page = 1, pageSize = 20) => {
  return useQuery({
    queryKey: mediaFilesKeys.byUser(userId),
    queryFn: () => mediaFilesApi.getMediaFilesByUser(userId, page, pageSize),
    enabled: !!userId,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for getting media files by type
export const useMediaFilesByType = (mediaType: MediaType, page = 1, pageSize = 20) => {
  return useQuery({
    queryKey: mediaFilesKeys.byType(mediaType),
    queryFn: () => mediaFilesApi.getMediaFilesByType(mediaType, page, pageSize),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for getting recent media files
export const useRecentMediaFiles = (count = 10, userId?: number) => {
  return useQuery({
    queryKey: mediaFilesKeys.recent(count, userId),
    queryFn: () => mediaFilesApi.getRecentMediaFiles(count, userId),
    staleTime: 2 * 60 * 1000, // 2 minutes for recent items
    gcTime: 5 * 60 * 1000,
  });
};

// Hook for getting popular media files
export const usePopularMediaFiles = (count = 10, timeRange?: string) => {
  return useQuery({
    queryKey: mediaFilesKeys.popular(count, timeRange),
    queryFn: () => mediaFilesApi.getPopularMediaFiles(count, timeRange),
    staleTime: 10 * 60 * 1000, // 10 minutes for popular items
    gcTime: 20 * 60 * 1000,
  });
};

// Hook for getting similar media files
export const useSimilarMediaFiles = (mediaId: number) => {
  return useQuery({
    queryKey: mediaFilesKeys.similar(mediaId),
    queryFn: () => mediaFilesApi.getSimilarMediaFiles(mediaId),
    enabled: !!mediaId,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
  });
};

// Hook for creating a media file
export const useCreateMediaFile = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateMediaFileDto) => mediaFilesApi.createMediaFile(data),
    onSuccess: (newMediaFile) => {
      // Invalidate and refetch media files lists
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });
      
      // Add the new media file to the cache
      queryClient.setQueryData(
        mediaFilesKeys.detail(newMediaFile.id),
        newMediaFile
      );

      toast.success('Media file created successfully');
    },
    onError: (error: any) => {
      console.error('Error creating media file:', error);
      toast.error(error.response?.data?.message || 'Failed to create media file');
    },
  });
};

// Hook for updating a media file
export const useUpdateMediaFile = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateMediaFileDto }) =>
      mediaFilesApi.updateMediaFile(id, data),
    onSuccess: (updatedMediaFile, { id }) => {
      // Update the media file in the cache
      queryClient.setQueryData(
        mediaFilesKeys.detail(id),
        updatedMediaFile
      );

      // Invalidate lists to reflect changes
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });

      toast.success('Media file updated successfully');
    },
    onError: (error: any) => {
      console.error('Error updating media file:', error);
      toast.error(error.response?.data?.message || 'Failed to update media file');
    },
  });
};

// Hook for deleting a media file
export const useDeleteMediaFile = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => mediaFilesApi.deleteMediaFile(id),
    onSuccess: (_, deletedId) => {
      // Remove the media file from the cache
      queryClient.removeQueries({ queryKey: mediaFilesKeys.detail(deletedId) });

      // Invalidate lists to reflect changes
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });

      toast.success('Media file deleted successfully');
    },
    onError: (error: any) => {
      console.error('Error deleting media file:', error);
      toast.error(error.response?.data?.message || 'Failed to delete media file');
    },
  });
};

// Hook for uploading a media file
export const useUploadMediaFile = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ file, metadata }: { file: File; metadata?: Partial<CreateMediaFileDto> }) =>
      mediaFilesApi.uploadMediaFile(file, metadata),
    onSuccess: (newMediaFile) => {
      // Invalidate and refetch media files lists
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });
      
      // Add the new media file to the cache
      queryClient.setQueryData(
        mediaFilesKeys.detail(newMediaFile.id),
        newMediaFile
      );

      toast.success('Media file uploaded successfully');
    },
    onError: (error: any) => {
      console.error('Error uploading media file:', error);
      toast.error(error.response?.data?.message || 'Failed to upload media file');
    },
  });
};

// Hook for bulk operations
export const useBulkDeleteMedia = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (mediaIds: number[]) => mediaFilesApi.bulkDeleteMedia(mediaIds),
    onSuccess: (_, deletedIds) => {
      // Remove deleted media files from the cache
      deletedIds.forEach(id => {
        queryClient.removeQueries({ queryKey: mediaFilesKeys.detail(id) });
      });

      // Invalidate lists to reflect changes
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });

      toast.success(`${deletedIds.length} media files deleted successfully`);
    },
    onError: (error: any) => {
      console.error('Error bulk deleting media files:', error);
      toast.error(error.response?.data?.message || 'Failed to delete media files');
    },
  });
};

export const useBulkMoveMedia = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ mediaIds, targetCategoryId }: { mediaIds: number[]; targetCategoryId: number }) =>
      mediaFilesApi.bulkMoveMedia(mediaIds, targetCategoryId),
    onSuccess: (_, { mediaIds }) => {
      // Invalidate lists to reflect changes
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });

      toast.success(`${mediaIds.length} media files moved successfully`);
    },
    onError: (error: any) => {
      console.error('Error bulk moving media files:', error);
      toast.error(error.response?.data?.message || 'Failed to move media files');
    },
  });
};

export const useBulkUpdateMedia = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: { mediaFileIds: number[]; operation: string; targetCategoryId?: number; targetStatus?: MediaFileStatus; tags?: string }) =>
      mediaFilesApi.bulkUpdateMedia(data),
    onSuccess: (_, { mediaFileIds }) => {
      // Invalidate lists to reflect changes
      queryClient.invalidateQueries({ queryKey: mediaFilesKeys.lists() });

      toast.success(`${mediaFileIds.length} media files updated successfully`);
    },
    onError: (error: any) => {
      console.error('Error bulk updating media files:', error);
      toast.error(error.response?.data?.message || 'Failed to update media files');
    },
  });
};

// Hook for logging media access
export const useLogMediaAccess = () => {
  return useMutation({
    mutationFn: ({ mediaId, userId, action }: { mediaId: number; userId: number; action: string }) =>
      mediaFilesApi.logMediaAccess(mediaId, userId, action),
    onError: (error: any) => {
      console.error('Error logging media access:', error);
    },
  });
};
