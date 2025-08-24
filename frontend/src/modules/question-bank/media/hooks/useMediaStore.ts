import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { 
  MediaFileDto, 
  MediaCategoryDto, 
  MediaCollectionDto, 
  MediaFileSearchDto,
  MediaType,
  MediaStatus,
  ProcessingJobType,
  ProcessingJobStatus
} from '../types/media.types';
import { mediaApiService } from '../services/mediaApi.service';

interface MediaStore {
  // State
  mediaFiles: MediaFileDto[];
  categories: MediaCategoryDto[];
  collections: MediaCollectionDto[];
  processingJobs: any[];
  stats: {
    totalFiles: number;
    totalSize: number;
    byType: Record<MediaType, number>;
    byStatus: Record<MediaStatus, number>;
    recentUploads: number;
  } | null;
  loading: boolean;
  error: string | null;
  pagination: {
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };

  // Actions
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  setMediaFiles: (files: MediaFileDto[]) => void;
  setCategories: (categories: MediaCategoryDto[]) => void;
  setCollections: (collections: MediaCollectionDto[]) => void;
  setProcessingJobs: (jobs: any[]) => void;
  setStats: (stats: any) => void;
  setPagination: (pagination: any) => void;
  
  // Async actions
  fetchMediaFiles: (searchDto?: MediaFileSearchDto) => Promise<void>;
  fetchCategories: () => Promise<void>;
  fetchCollections: () => Promise<void>;
  fetchProcessingJobs: (status?: ProcessingJobStatus, jobType?: ProcessingJobType) => Promise<void>;
  fetchStats: () => Promise<void>;
  refreshData: () => Promise<void>;
  
  // Media file operations
  addMediaFile: (file: MediaFileDto) => void;
  updateMediaFile: (id: number, updates: Partial<MediaFileDto>) => void;
  removeMediaFile: (id: number) => void;
  bulkUpdateMediaFiles: (ids: number[], updates: Partial<MediaFileDto>) => void;
  bulkDeleteMediaFiles: (ids: number[]) => void;
  
  // Category operations
  addCategory: (category: MediaCategoryDto) => void;
  updateCategory: (id: number, updates: Partial<MediaCategoryDto>) => void;
  removeCategory: (id: number) => void;
  
  // Collection operations
  addCollection: (collection: MediaCollectionDto) => void;
  updateCollection: (id: number, updates: Partial<MediaCollectionDto>) => void;
  removeCollection: (id: number) => void;
  
  // Utility methods
  getMediaFileById: (id: number) => MediaFileDto | undefined;
  getCategoryById: (id: number) => MediaCategoryDto | undefined;
  getCollectionById: (id: number) => MediaCollectionDto | undefined;
  getMediaByCategory: (categoryId: number) => MediaFileDto[];
  getMediaByCollection: (collectionId: number) => MediaFileDto[];
  getMediaByType: (type: MediaType) => MediaFileDto[];
  getMediaByStatus: (status: MediaStatus) => MediaFileDto[];
  searchMediaFiles: (query: string) => MediaFileDto[];
}

export const useMediaStore = create<MediaStore>()(
  devtools(
    (set, get) => ({
      // Initial state
      mediaFiles: [],
      categories: [],
      collections: [],
      processingJobs: [],
      stats: null,
      loading: false,
      error: null,
      pagination: {
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0
      },

      // State setters
      setLoading: (loading) => set({ loading }),
      setError: (error) => set({ error }),
      setMediaFiles: (mediaFiles) => set({ mediaFiles }),
      setCategories: (categories) => set({ categories }),
      setCollections: (collections) => set({ collections }),
      setProcessingJobs: (processingJobs) => set({ processingJobs }),
      setStats: (stats) => set({ stats }),
      setPagination: (pagination) => set({ pagination }),

      // Async actions
      fetchMediaFiles: async (searchDto) => {
        try {
          set({ loading: true, error: null });
          
          const searchParams = searchDto || {
            page: get().pagination.currentPage,
            pageSize: get().pagination.pageSize,
            sortBy: 'uploadedAt',
            sortOrder: 'desc' as const
          };
          
          const result = await mediaApiService.getMediaFiles(searchParams);
          
          set({
            mediaFiles: result.items,
            pagination: {
              currentPage: result.pageNumber,
              pageSize: result.pageSize,
              totalCount: result.totalCount,
              totalPages: result.totalPages
            }
          });
        } catch (error) {
          set({ 
            error: error instanceof Error ? error.message : 'Failed to fetch media files',
            mediaFiles: []
          });
        } finally {
          set({ loading: false });
        }
      },

      fetchCategories: async () => {
        try {
          set({ loading: true, error: null });
          const categories = await mediaApiService.getCategories();
          set({ categories });
        } catch (error) {
          set({ 
            error: error instanceof Error ? error.message : 'Failed to fetch categories',
            categories: []
          });
        } finally {
          set({ loading: false });
        }
      },

      fetchCollections: async () => {
        try {
          set({ loading: true, error: null });
          const collections = await mediaApiService.getCollections();
          set({ collections });
        } catch (error) {
          set({ 
            error: error instanceof Error ? error.message : 'Failed to fetch collections',
            collections: []
          });
        } finally {
          set({ loading: false });
        }
      },

      fetchProcessingJobs: async (status, jobType) => {
        try {
          set({ loading: true, error: null });
          const jobs = await mediaApiService.getProcessingJobs(status, jobType);
          set({ processingJobs: jobs });
        } catch (error) {
          set({ 
            error: error instanceof Error ? error.message : 'Failed to fetch processing jobs',
            processingJobs: []
          });
        } finally {
          set({ loading: false });
        }
      },

      fetchStats: async () => {
        try {
          set({ loading: true, error: null });
          const stats = await mediaApiService.getMediaStats();
          set({ stats });
        } catch (error) {
          set({ 
            error: error instanceof Error ? error.message : 'Failed to fetch stats',
            stats: null
          });
        } finally {
          set({ loading: false });
        }
      },

      refreshData: async () => {
        const { fetchMediaFiles, fetchCategories, fetchCollections, fetchStats } = get();
        await Promise.all([
          fetchMediaFiles(),
          fetchCategories(),
          fetchCollections(),
          fetchStats()
        ]);
      },

      // Media file operations
      addMediaFile: (file) => {
        set((state) => ({
          mediaFiles: [file, ...state.mediaFiles],
          stats: state.stats ? {
            ...state.stats,
            totalFiles: state.stats.totalFiles + 1,
            byType: {
              ...state.stats.byType,
              [file.mediaType]: (state.stats.byType[file.mediaType] || 0) + 1
            }
          } : null
        }));
      },

      updateMediaFile: (id, updates) => {
        set((state) => ({
          mediaFiles: state.mediaFiles.map(file =>
            file.id === id ? { ...file, ...updates } : file
          )
        }));
      },

      removeMediaFile: (id) => {
        set((state) => {
          const fileToRemove = state.mediaFiles.find(f => f.id === id);
          return {
            mediaFiles: state.mediaFiles.filter(f => f.id !== id),
            stats: state.stats && fileToRemove ? {
              ...state.stats,
              totalFiles: Math.max(0, state.stats.totalFiles - 1),
              byType: {
                ...state.stats.byType,
                [fileToRemove.mediaType]: Math.max(0, (state.stats.byType[fileToRemove.mediaType] || 0) - 1)
              }
            } : state.stats
          };
        });
      },

      bulkUpdateMediaFiles: (ids, updates) => {
        set((state) => ({
          mediaFiles: state.mediaFiles.map(file =>
            ids.includes(file.id) ? { ...file, ...updates } : file
          )
        }));
      },

      bulkDeleteMediaFiles: (ids) => {
        set((state) => {
          const filesToRemove = state.mediaFiles.filter(f => ids.includes(f.id));
          return {
            mediaFiles: state.mediaFiles.filter(f => !ids.includes(f.id)),
            stats: state.stats ? {
              ...state.stats,
              totalFiles: Math.max(0, state.stats.totalFiles - filesToRemove.length),
              byType: filesToRemove.reduce((acc, file) => ({
                ...acc,
                [file.mediaType]: Math.max(0, (acc[file.mediaType] || 0) - 1)
              }), state.stats.byType)
            } : null
          };
        });
      },

      // Category operations
      addCategory: (category) => {
        set((state) => ({
          categories: [...state.categories, category]
        }));
      },

      updateCategory: (id, updates) => {
        set((state) => ({
          categories: state.categories.map(cat =>
            cat.id === id ? { ...cat, ...updates } : cat
          )
        }));
      },

      removeCategory: (id) => {
        set((state) => ({
          categories: state.categories.filter(cat => cat.id !== id)
        }));
      },

      // Collection operations
      addCollection: (collection) => {
        set((state) => ({
          collections: [...state.collections, collection]
        }));
      },

      updateCollection: (id, updates) => {
        set((state) => ({
          collections: state.collections.map(col =>
            col.id === id ? { ...col, ...updates } : col
          )
        }));
      },

      removeCollection: (id) => {
        set((state) => ({
          collections: state.collections.filter(col => col.id !== id)
        }));
      },

      // Utility methods
      getMediaFileById: (id) => {
        return get().mediaFiles.find(file => file.id === id);
      },

      getCategoryById: (id) => {
        return get().categories.find(cat => cat.id === id);
      },

      getCollectionById: (id) => {
        return get().collections.find(col => col.id === id);
      },

      getMediaByCategory: (categoryId) => {
        return get().mediaFiles.filter(file => file.categoryId === categoryId);
      },

      getMediaByCollection: (collectionId) => {
        return get().mediaFiles.filter(file => file.collectionId === collectionId);
      },

      getMediaByType: (type) => {
        return get().mediaFiles.filter(file => file.mediaType === type);
      },

      getMediaByStatus: (status) => {
        return get().mediaFiles.filter(file => file.status === status);
      },

      searchMediaFiles: (query) => {
        const lowercaseQuery = query.toLowerCase();
        return get().mediaFiles.filter(file =>
          file.fileName.toLowerCase().includes(lowercaseQuery) ||
          file.originalFileName.toLowerCase().includes(lowercaseQuery) ||
          file.mimeType.toLowerCase().includes(lowercaseQuery)
        );
      }
    }),
    {
      name: 'media-store',
      enabled: process.env.NODE_ENV === 'development'
    }
  )
);

