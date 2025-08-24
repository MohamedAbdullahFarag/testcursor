import React, { useState, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { 
  Search, 
  Filter, 
  Grid3X3, 
  List, 
  Download, 
  Trash2, 
  Edit3,
  Eye,
  MoreHorizontal
} from 'lucide-react';
import { 
  Button, 
  Input, 
  Card, 
  CardContent, 
  Badge,
  Checkbox,
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue
} from 'mada-design-system';

import { useMediaFiles } from '../hooks/useMediaFiles';
import { useDeleteMediaFile, useBulkDeleteMedia } from '../hooks/useMediaFiles';
import { MediaFileDto, MediaType, MediaStatus, MediaFileFilter } from '../types';
import { formatFileSize, formatDate, getMediaTypeIcon } from '../utils/mediaUtils';

export const MediaLibrary: React.FC = () => {
  const { t } = useTranslation('media-management');
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [selectedFiles, setSelectedFiles] = useState<number[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filters, setFilters] = useState<MediaFileFilter>({});
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  // Search parameters
  const searchParams = useMemo(() => ({
    searchTerm: searchTerm || undefined,
    mediaType: filters.mediaType,
    categoryId: filters.categoryId,
    status: filters.status,
    uploadedBy: filters.uploadedBy,
    page: currentPage,
    pageSize,
    sortBy: 'createdAt',
    sortDescending: true
  }), [searchTerm, filters, currentPage, pageSize]);

  // Fetch media files
  const { data: mediaFilesData, isLoading, error } = useMediaFiles(searchParams);
  const deleteMediaFile = useDeleteMediaFile();
  const bulkDeleteMedia = useBulkDeleteMedia();

  // Handle file selection
  const handleFileSelect = (fileId: number, checked: boolean) => {
    if (checked) {
      setSelectedFiles(prev => [...prev, fileId]);
    } else {
      setSelectedFiles(prev => prev.filter(id => id !== fileId));
    }
  };

  // Handle select all
  const handleSelectAll = (checked: boolean) => {
    if (checked && mediaFilesData?.items) {
      setSelectedFiles(mediaFilesData.items.map(file => file.id));
    } else {
      setSelectedFiles([]);
    }
  };

  // Handle bulk delete
  const handleBulkDelete = async () => {
    if (selectedFiles.length === 0) return;
    
    if (confirm(t('confirm.bulkDelete', { count: selectedFiles.length }))) {
      await bulkDeleteMedia.mutateAsync(selectedFiles);
      setSelectedFiles([]);
    }
  };

  // Handle single file delete
  const handleDeleteFile = async (fileId: number) => {
    if (confirm(t('confirm.deleteFile'))) {
      await deleteMediaFile.mutateAsync(fileId);
    }
  };

  // Handle download
  const handleDownload = (file: MediaFileDto) => {
    // TODO: Implement download functionality
    console.log('Downloading file:', file.originalFileName);
  };

  // Handle view
  const handleView = (file: MediaFileDto) => {
    // TODO: Implement view functionality
    console.log('Viewing file:', file.originalFileName);
  };

  // Handle edit
  const handleEdit = (file: MediaFileDto) => {
    // TODO: Implement edit functionality
    console.log('Editing file:', file.originalFileName);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-gray-600">{t('loading.mediaFiles')}</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <p className="text-red-600">{t('error.loadingMediaFiles')}</p>
      </div>
    );
  }

  const mediaFiles = mediaFilesData?.items || [];
  const totalCount = mediaFilesData?.totalCount || 0;

  return (
    <div className="space-y-6">
      {/* Search and Filters */}
      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
          <Input
            placeholder={t('search.placeholder')}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        
        <Select value={filters.mediaType?.toString()} onValueChange={(value) => setFilters(prev => ({ ...prev, mediaType: value ? parseInt(value) : undefined }))}>
          <SelectTrigger className="w-40">
            <SelectValue placeholder={t('filters.allTypes')} />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">{t('filters.allTypes')}</SelectItem>
            <SelectItem value={MediaType.Image.toString()}>{t('filters.images')}</SelectItem>
            <SelectItem value={MediaType.Video.toString()}>{t('filters.videos')}</SelectItem>
            <SelectItem value={MediaType.Audio.toString()}>{t('filters.audio')}</SelectItem>
            <SelectItem value={MediaType.Document.toString()}>{t('filters.documents')}</SelectItem>
          </SelectContent>
        </Select>

        <Select value={filters.status?.toString()} onValueChange={(value) => setFilters(prev => ({ ...prev, status: value ? parseInt(value) : undefined }))}>
          <SelectTrigger className="w-40">
            <SelectValue placeholder={t('filters.allStatuses')} />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">{t('filters.allStatuses')}</SelectItem>
            <SelectItem value={MediaStatus.Ready.toString()}>{t('filters.ready')}</SelectItem>
            <SelectItem value={MediaStatus.Processing.toString()}>{t('filters.processing')}</SelectItem>
            <SelectItem value={MediaStatus.Failed.toString()}>{t('filters.failed')}</SelectItem>
          </SelectContent>
        </Select>

        <div className="flex items-center space-x-2">
          <Button
            variant={viewMode === 'grid' ? 'default' : 'outline'}
            size="sm"
            onClick={() => setViewMode('grid')}
          >
            <Grid3X3 className="w-4 h-4" />
          </Button>
          <Button
            variant={viewMode === 'list' ? 'default' : 'outline'}
            size="sm"
            onClick={() => setViewMode('list')}
          >
            <List className="w-4 h-4" />
          </Button>
        </div>
      </div>

      {/* Bulk Actions */}
      {selectedFiles.length > 0 && (
        <div className="flex items-center justify-between p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
          <span className="text-sm text-blue-700 dark:text-blue-300">
            {t('bulk.selected', { count: selectedFiles.length })}
          </span>
          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handleBulkDelete}
              disabled={bulkDeleteMedia.isPending}
            >
              <Trash2 className="w-4 h-4 mr-2" />
              {t('bulk.delete')}
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setSelectedFiles([])}
            >
              {t('bulk.clear')}
            </Button>
          </div>
        </div>
      )}

      {/* Media Files Grid/List */}
      {viewMode === 'grid' ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-4">
          {mediaFiles.map((file) => (
            <MediaFileCard
              key={file.id}
              file={file}
              selected={selectedFiles.includes(file.id)}
              onSelect={handleFileSelect}
              onDelete={handleDeleteFile}
              onDownload={handleDownload}
              onView={handleView}
              onEdit={handleEdit}
            />
          ))}
        </div>
      ) : (
        <div className="space-y-2">
          {mediaFiles.map((file) => (
            <MediaFileRow
              key={file.id}
              file={file}
              selected={selectedFiles.includes(file.id)}
              onSelect={handleFileSelect}
              onDelete={handleDeleteFile}
              onDownload={handleDownload}
              onView={handleView}
              onEdit={handleEdit}
            />
          ))}
        </div>
      )}

      {/* Pagination */}
      {totalCount > pageSize && (
        <div className="flex items-center justify-between">
          <div className="text-sm text-gray-600">
            {t('pagination.showing', { 
              from: (currentPage - 1) * pageSize + 1, 
              to: Math.min(currentPage * pageSize, totalCount), 
              total: totalCount 
            })}
          </div>
          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
              disabled={currentPage === 1}
            >
              Previous
            </Button>
            <span className="text-sm text-gray-600">
              Page {currentPage} of {Math.ceil(totalCount / pageSize)}
            </span>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage(Math.min(Math.ceil(totalCount / pageSize), currentPage + 1))}
              disabled={currentPage >= Math.ceil(totalCount / pageSize)}
            >
              Next
            </Button>
          </div>
        </div>
      )}

      {/* Empty State */}
      {mediaFiles.length === 0 && !isLoading && (
        <div className="text-center py-12">
          <div className="mx-auto w-24 h-24 bg-gray-100 dark:bg-gray-800 rounded-full flex items-center justify-center mb-4">
            <Eye className="w-12 h-12 text-gray-400" />
          </div>
          <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
            {t('empty.title')}
          </h3>
          <p className="text-gray-600 dark:text-gray-400 mb-4">
            {t('empty.description')}
          </p>
          <Button>
            {/* Assuming Upload icon is available, otherwise remove or replace with a placeholder */}
            {/* <Upload className="w-4 h-4 mr-2" /> */}
            {t('empty.uploadFirst')}
          </Button>
        </div>
      )}
    </div>
  );
};

// Media File Card Component
interface MediaFileCardProps {
  file: MediaFileDto;
  selected: boolean;
  onSelect: (fileId: number, checked: boolean) => void;
  onDelete: (fileId: number) => void;
  onDownload: (file: MediaFileDto) => void;
  onView: (file: MediaFileDto) => void;
  onEdit: (file: MediaFileDto) => void;
}

const MediaFileCard: React.FC<MediaFileCardProps> = ({
  file,
  selected,
  onSelect,
  onDelete,
  onDownload,
  onView,
  onEdit
}) => {
  const { t } = useTranslation('media-management');

  return (
    <Card className={`relative group hover:shadow-lg transition-shadow ${selected ? 'ring-2 ring-blue-500' : ''}`}>
      <CardContent className="p-3">
        {/* Selection Checkbox */}
        <div className="absolute top-2 left-2 z-10">
          <Checkbox
            checked={selected}
            onCheckedChange={(checked) => onSelect(file.id, checked as boolean)}
            className="bg-white/80 backdrop-blur-sm"
          />
        </div>

        {/* File Preview */}
        <div className="relative aspect-square bg-gray-100 dark:bg-gray-800 rounded-lg mb-3 overflow-hidden">
          {file.mediaType === MediaType.Image ? (
            <img
              src={`/api/media/${file.id}/thumbnail`}
              alt={file.title || file.originalFileName}
              className="w-full h-full object-cover"
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center">
              {(() => {
                const iconData = getMediaTypeIcon(file.mediaType, 'w-12 h-12 text-gray-400');
                const IconComponent = iconData.component;
                return <IconComponent className={iconData.className} />;
              })()}
            </div>
          )}

          {/* Status Badge */}
          <div className="absolute top-2 right-2">
            <Badge variant={file.status === MediaStatus.Ready ? 'default' : 'outline'}>
              {t(`status.${file.status}`)}
            </Badge>
          </div>

          {/* Actions Overlay */}
          <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
            <div className="flex space-x-2">
                          <Button size="sm" variant="outline" onClick={() => onView(file)}>
              <Eye className="w-4 h-4" />
            </Button>
            <Button size="sm" variant="outline" onClick={() => onDownload(file)}>
              <Download className="w-4 h-4" />
            </Button>
            <Button size="sm" variant="outline" onClick={() => onEdit(file)}>
              <Edit3 className="w-4 h-4" />
            </Button>
            </div>
          </div>
        </div>

        {/* File Info */}
        <div className="space-y-2">
          <h4 className="font-medium text-sm truncate" title={file.title || file.originalFileName}>
            {file.title || file.originalFileName}
          </h4>
          <div className="text-xs text-gray-500 space-y-1">
            <p>{formatFileSize(file.fileSizeBytes)}</p>
            <p>{formatDate(file.createdAt)}</p>
          </div>
        </div>

        {/* More Actions */}
        <div className="absolute top-2 right-2 z-10">
          <div className="flex space-x-1">
            <Button variant="ghost" size="sm" onClick={() => onView(file)}>
              <Eye className="w-4 h-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => onDownload(file)}>
              <Download className="w-4 h-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => onEdit(file)}>
              <Edit3 className="w-4 h-4" />
            </Button>
            <Button variant="ghost" size="sm" onClick={() => onDelete(file.id)}>
              <Trash2 className="w-4 h-4" />
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

// Media File Row Component
interface MediaFileRowProps {
  file: MediaFileDto;
  selected: boolean;
  onSelect: (fileId: number, checked: boolean) => void;
  onDelete: (fileId: number) => void;
  onDownload: (file: MediaFileDto) => void;
  onView: (file: MediaFileDto) => void;
  onEdit: (file: MediaFileDto) => void;
}

const MediaFileRow: React.FC<MediaFileRowProps> = ({
  file,
  selected,
  onSelect,
  onDelete,
  onDownload,
  onView,
  onEdit
}) => {
  const { t } = useTranslation('media-management');

  return (
    <div className={`flex items-center space-x-4 p-4 border rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 ${selected ? 'bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-700' : ''}`}>
      <Checkbox
        checked={selected}
        onCheckedChange={(checked) => onSelect(file.id, checked as boolean)}
      />
      
      <div className="w-16 h-16 bg-gray-100 dark:bg-gray-800 rounded-lg flex items-center justify-center flex-shrink-0">
        {file.mediaType === MediaType.Image ? (
          <img
            src={`/api/media/${file.id}/thumbnail`}
            alt={file.title || file.originalFileName}
            className="w-full h-full object-cover rounded-lg"
          />
        ) : (
          (() => {
            const iconData = getMediaTypeIcon(file.mediaType, 'w-8 h-8 text-gray-400');
            const IconComponent = iconData.component;
            return <IconComponent className={iconData.className} />;
          })()
        )}
      </div>

      <div className="flex-1 min-w-0">
        <h4 className="font-medium truncate" title={file.title || file.originalFileName}>
          {file.title || file.originalFileName}
        </h4>
        <p className="text-sm text-gray-500">
          {formatFileSize(file.fileSizeBytes)} • {formatDate(file.createdAt)}
        </p>
      </div>

      <Badge variant={file.status === MediaStatus.Ready ? 'default' : 'outline'}>
        {t(`status.${file.status}`)}
      </Badge>

      <div className="flex items-center space-x-2">
        <Button size="sm" variant="ghost" onClick={() => onView(file)}>
          <Eye className="w-4 h-4" />
        </Button>
        <Button size="sm" variant="ghost" onClick={() => onDownload(file)}>
          <Download className="w-4 h-4" />
        </Button>
        <Button size="sm" variant="ghost" onClick={() => onEdit(file)}>
          <Edit3 className="w-4 h-4" />
        </Button>
        <Button size="sm" variant="ghost" onClick={() => onDelete(file.id)}>
          <Trash2 className="w-4 h-4" />
        </Button>
      </div>
    </div>
  );
};
