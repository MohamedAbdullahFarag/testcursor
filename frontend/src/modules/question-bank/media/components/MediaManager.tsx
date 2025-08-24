import React, { useState, useEffect, useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { 
  PlusIcon, 
  FolderIcon, 
  PhotoIcon, 
  VideoCameraIcon, 
  MusicalNoteIcon, 
  DocumentIcon,
  ArchiveBoxIcon,
  MagnifyingGlassIcon,
  FunnelIcon,
  ViewColumnsIcon,
  ListBulletIcon,
  Squares2X2Icon
} from '@heroicons/react/24/outline';
import { MediaFileDto, MediaType, MediaStatus, MediaFileSearchDto, PagedResult } from '../types/media.types';
import { mediaApiService } from '../services/mediaApi.service';
import MediaUploader from './MediaUploader';
import MediaGallery from './MediaGallery';
import MediaGrid from './MediaGrid';
import MediaList from './MediaList';
import MediaFilters from './MediaFilters';
import MediaSearch from './MediaSearch';
import MediaCategoryManager from './MediaCategoryManager';
import MediaCollectionManager from './MediaCollectionManager';
import MediaPreview from './MediaPreview';
import MediaSelector from './MediaSelector';
import { useMediaStore } from '../hooks/useMediaStore';
import { useMediaFilters } from '../hooks/useMediaFilters';
import { useMediaSearch } from '../hooks/useMediaSearch';

interface MediaManagerProps {
  className?: string;
  onMediaSelect?: (media: MediaFileDto[]) => void;
  selectionMode?: boolean;
  maxSelection?: number;
}

export const MediaManager: React.FC<MediaManagerProps> = ({
  className = '',
  onMediaSelect,
  selectionMode = false,
  maxSelection = 1
}) => {
  const { t } = useTranslation('media');
  const [viewMode, setViewMode] = useState<'grid' | 'list' | 'gallery'>('grid');
  const [showUploader, setShowUploader] = useState(false);
  const [showCategoryManager, setShowCategoryManager] = useState(false);
  const [showCollectionManager, setShowCollectionManager] = useState(false);
  const [selectedMedia, setSelectedMedia] = useState<MediaFileDto[]>([]);
  const [previewMedia, setPreviewMedia] = useState<MediaFileDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    mediaFiles,
    categories,
    collections,
    stats,
    loading,
    error: storeError,
    fetchMediaFiles,
    fetchCategories,
    fetchCollections,
    fetchStats,
    refreshData
  } = useMediaStore();

  const {
    filters,
    setFilters,
    clearFilters,
    applyFilters
  } = useMediaFilters();

  const {
    searchTerm,
    setSearchTerm,
    searchResults,
    performSearch,
    clearSearch
  } = useMediaSearch();

  // Initialize data on component mount
  useEffect(() => {
    const initializeData = async () => {
      try {
        setIsLoading(true);
        setError(null);
        
        await Promise.all([
          fetchMediaFiles(),
          fetchCategories(),
          fetchCollections(),
          fetchStats()
        ]);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load media data');
      } finally {
        setIsLoading(false);
      }
    };

    initializeData();
  }, [fetchMediaFiles, fetchCategories, fetchCollections, fetchStats]);

  // Handle media selection
  const handleMediaSelect = useCallback((media: MediaFileDto, isSelected: boolean) => {
    if (selectionMode) {
      if (isSelected) {
        if (maxSelection === 1) {
          setSelectedMedia([media]);
        } else if (selectedMedia.length < maxSelection) {
          setSelectedMedia(prev => [...prev, media]);
        }
      } else {
        setSelectedMedia(prev => prev.filter(m => m.id !== media.id));
      }
    }
  }, [selectionMode, maxSelection, selectedMedia]);

  // Handle media preview
  const handleMediaPreview = useCallback((media: MediaFileDto) => {
    setPreviewMedia(media);
  }, []);

  // Handle media upload
  const handleMediaUpload = useCallback(async (files: File[]) => {
    try {
      setIsLoading(true);
      setError(null);

      const uploadPromises = files.map(file => 
        mediaApiService.uploadMediaFile(file)
      );

      await Promise.all(uploadPromises);
      
      // Refresh data after successful upload
      await refreshData();
      
      // Close uploader
      setShowUploader(false);
      
      // Show success message
      // You can implement a toast notification here
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to upload media files');
    } finally {
      setIsLoading(false);
    }
  }, [refreshData]);

  // Handle search
  const handleSearch = useCallback(async (searchDto: MediaFileSearchDto) => {
    try {
      await performSearch(searchDto);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Search failed');
    }
  }, [performSearch]);

  // Handle filter application
  const handleFilterApply = useCallback(async (appliedFilters: any) => {
    try {
      await applyFilters(appliedFilters);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to apply filters');
    }
  }, [applyFilters]);

  // Handle media deletion
  const handleMediaDelete = useCallback(async (mediaIds: number[]) => {
    try {
      setIsLoading(true);
      setError(null);

      const deletePromises = mediaIds.map(id => 
        mediaApiService.deleteMediaFile(id)
      );

      await Promise.all(deletePromises);
      
      // Refresh data after successful deletion
      await refreshData();
      
      // Clear selection
      setSelectedMedia([]);
      
      // Show success message
      // You can implement a toast notification here
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete media files');
    } finally {
      setIsLoading(false);
    }
  }, [refreshData]);

  // Handle final selection (for selection mode)
  const handleFinalSelection = useCallback(() => {
    if (onMediaSelect && selectedMedia.length > 0) {
      onMediaSelect(selectedMedia);
    }
  }, [onMediaSelect, selectedMedia]);

  // Render view mode component
  const renderViewMode = () => {
    const commonProps = {
      mediaFiles: searchResults.length > 0 ? searchResults : mediaFiles,
      onMediaSelect: handleMediaSelect,
      onMediaPreview: handleMediaPreview,
      selectedMedia,
      selectionMode,
      maxSelection
    };

    switch (viewMode) {
      case 'grid':
        return <MediaGrid {...commonProps} />;
      case 'list':
        return <MediaList {...commonProps} />;
      case 'gallery':
        return <MediaGallery {...commonProps} />;
      default:
        return <MediaGrid {...commonProps} />;
    }
  };

  // Render view mode toggle buttons
  const renderViewModeToggle = () => (
    <div className="flex items-center space-x-2">
      <button
        onClick={() => setViewMode('grid')}
        className={`p-2 rounded-lg transition-colors ${
          viewMode === 'grid' 
            ? 'bg-blue-100 text-blue-600' 
            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
        }`}
        title={t('viewMode.grid')}
      >
        <Squares2X2Icon className="w-5 h-5" />
      </button>
      <button
        onClick={() => setViewMode('list')}
        className={`p-2 rounded-lg transition-colors ${
          viewMode === 'list' 
            ? 'bg-blue-100 text-blue-600' 
            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
        }`}
        title={t('viewMode.list')}
      >
        <ListBulletIcon className="w-5 h-5" />
      </button>
      <button
        onClick={() => setViewMode('gallery')}
        className={`p-2 rounded-lg transition-colors ${
          viewMode === 'gallery' 
            ? 'bg-blue-100 text-blue-600' 
            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
        }`}
        title={t('viewMode.gallery')}
      >
        <ViewColumnsIcon className="w-5 h-5" />
      </button>
    </div>
  );

  // Render action buttons
  const renderActionButtons = () => (
    <div className="flex items-center space-x-3">
      <button
        onClick={() => setShowUploader(true)}
        className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
      >
        <PlusIcon className="w-5 h-5" />
        <span>{t('actions.upload')}</span>
      </button>

      <button
        onClick={() => setShowCategoryManager(true)}
        className="flex items-center space-x-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
      >
        <FolderIcon className="w-5 h-5" />
        <span>{t('actions.manageCategories')}</span>
      </button>

      <button
        onClick={() => setShowCollectionManager(true)}
        className="flex items-center space-x-2 px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
      >
        <PhotoIcon className="w-5 h-5" />
        <span>{t('actions.manageCollections')}</span>
      </button>

      {selectionMode && selectedMedia.length > 0 && (
        <button
          onClick={handleFinalSelection}
          className="flex items-center space-x-2 px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 transition-colors"
        >
          <span>{t('actions.select', { count: selectedMedia.length })}</span>
        </button>
      )}
    </div>
  );

  // Render stats
  const renderStats = () => (
    <div className="grid grid-cols-2 md:grid-cols-6 gap-4 mb-6">
      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <PhotoIcon className="w-6 h-6 text-blue-600" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.images')}</p>
            <p className="text-lg font-semibold">{stats?.byType?.[MediaType.Image] || 0}</p>
          </div>
        </div>
      </div>

      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <VideoCameraIcon className="w-6 h-6 text-red-600" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.videos')}</p>
            <p className="text-lg font-semibold">{stats?.byType?.[MediaType.Video] || 0}</p>
          </div>
        </div>
      </div>

      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <MusicalNoteIcon className="w-6 h-6 text-green-600" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.audio')}</p>
            <p className="text-lg font-semibold">{stats?.byType?.[MediaType.Audio] || 0}</p>
          </div>
        </div>
      </div>

      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <DocumentIcon className="w-6 h-6 text-yellow-600" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.documents')}</p>
            <p className="text-lg font-semibold">{stats?.byType?.[MediaType.Document] || 0}</p>
          </div>
        </div>
      </div>

      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <ArchiveBoxIcon className="w-6 h-6 text-gray-600" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.total')}</p>
            <p className="text-lg font-semibold">{stats?.totalFiles || 0}</p>
          </div>
        </div>
      </div>

      <div className="bg-white p-4 rounded-lg shadow-sm border">
        <div className="flex items-center space-x-2">
          <div className="w-6 h-6 bg-blue-600 rounded-full" />
          <div>
            <p className="text-sm text-gray-600">{t('stats.size')}</p>
            <p className="text-lg font-semibold">
              {stats?.totalSize ? `${(stats.totalSize / (1024 * 1024)).toFixed(1)} MB` : '0 MB'}
            </p>
          </div>
        </div>
      </div>
    </div>
  );

  if (loading && !mediaFiles.length) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className={`space-y-6 ${className}`}>
      {/* Header */}
      <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{t('title')}</h1>
          <p className="text-gray-600">{t('description')}</p>
        </div>
        
        <div className="flex items-center space-x-4">
          {renderViewModeToggle()}
          {renderActionButtons()}
        </div>
      </div>

      {/* Stats */}
      {renderStats()}

      {/* Search and Filters */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <div className="lg:col-span-2">
          <MediaSearch
            searchTerm={searchTerm}
            onSearch={handleSearch}
            onClear={clearSearch}
          />
        </div>
        <div>
          <MediaFilters
            filters={filters}
            onFiltersChange={setFilters}
            onApply={handleFilterApply}
            onClear={clearFilters}
            categories={categories}
            collections={collections}
          />
        </div>
      </div>

      {/* Error Display */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <div className="flex items-center space-x-2">
            <div className="w-5 h-5 bg-red-500 rounded-full flex items-center justify-center">
              <span className="text-white text-xs">!</span>
            </div>
            <p className="text-red-800">{error}</p>
          </div>
        </div>
      )}

      {/* Media Content */}
      <div className="bg-white rounded-lg shadow-sm border">
        {renderViewMode()}
      </div>

      {/* Modals */}
      {showUploader && (
        <MediaUploader
          isOpen={showUploader}
          onClose={() => setShowUploader(false)}
          onUpload={handleMediaUpload}
          onError={setError}
        />
      )}

      {showCategoryManager && (
        <MediaCategoryManager
          isOpen={showCategoryManager}
          onClose={() => setShowCategoryManager(false)}
          onRefresh={refreshData}
        />
      )}

      {showCollectionManager && (
        <MediaCollectionManager
          isOpen={showCollectionManager}
          onClose={() => setShowCollectionManager(false)}
          onRefresh={refreshData}
        />
      )}

      {previewMedia && (
        <MediaPreview
          media={previewMedia}
          isOpen={!!previewMedia}
          onClose={() => setPreviewMedia(null)}
        />
      )}
    </div>
  );
};

export default MediaManager;

