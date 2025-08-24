import { useState, useCallback } from 'react';
import { MediaType, MediaStatus, MediaFileSearchDto } from '../types/media.types';

export interface MediaFilters {
  mediaType?: MediaType;
  status?: MediaStatus;
  categoryId?: number;
  collectionId?: number;
  uploadedBy?: number;
  dateFrom?: string;
  dateTo?: string;
  minSizeBytes?: number;
  maxSizeBytes?: number;
  tags?: string[];
}

export const useMediaFilters = () => {
  const [filters, setFilters] = useState<MediaFilters>({});
  const [appliedFilters, setAppliedFilters] = useState<MediaFilters>({});

  const updateFilter = useCallback((key: keyof MediaFilters, value: any) => {
    setFilters(prev => ({
      ...prev,
      [key]: value
    }));
  }, []);

  const clearFilter = useCallback((key: keyof MediaFilters) => {
    setFilters(prev => {
      const newFilters = { ...prev };
      delete newFilters[key];
      return newFilters;
    });
  }, []);

  const clearAllFilters = useCallback(() => {
    setFilters({});
    setAppliedFilters({});
  }, []);

  const applyFilters = useCallback(async (newFilters?: MediaFilters) => {
    const filtersToApply = newFilters || filters;
    setAppliedFilters(filtersToApply);
    
    // Convert filters to search DTO format
    const searchDto: MediaFileSearchDto = {
      page: 1,
      pageSize: 20,
      sortBy: 'uploadedAt',
      sortOrder: 'desc',
      ...filtersToApply
    };

    // You can return this search DTO or call an API directly
    return searchDto;
  }, [filters]);

  const hasActiveFilters = useCallback(() => {
    return Object.keys(filters).length > 0;
  }, [filters]);

  const getFilterCount = useCallback(() => {
    return Object.keys(filters).length;
  }, [filters]);

  const getActiveFilters = useCallback(() => {
    return Object.entries(filters).filter(([_, value]) => 
      value !== undefined && value !== null && value !== ''
    );
  }, [filters]);

  return {
    filters,
    appliedFilters,
    setFilters,
    updateFilter,
    clearFilter,
    clearAllFilters,
    applyFilters,
    hasActiveFilters,
    getFilterCount,
    getActiveFilters
  };
};

