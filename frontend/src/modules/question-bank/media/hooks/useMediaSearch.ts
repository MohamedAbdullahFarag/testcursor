import { useState, useCallback, useMemo } from 'react';
import { MediaFileDto, MediaFileSearchDto } from '../types/media.types';
import { mediaApiService } from '../services/mediaApi.service';

export const useMediaSearch = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState<MediaFileDto[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);
  const [searchHistory, setSearchHistory] = useState<string[]>([]);
  const [recentSearches, setRecentSearches] = useState<string[]>([]);

  // Perform search with API
  const performSearch = useCallback(async (searchDto: MediaFileSearchDto) => {
    try {
      setIsSearching(true);
      setSearchError(null);

      const results = await mediaApiService.searchMedia(searchDto);
      setSearchResults(results.items);

      // Add to search history if it's a meaningful search
      if (searchDto.searchTerm && searchDto.searchTerm.trim()) {
        const newSearchTerm = searchDto.searchTerm.trim();
        setSearchHistory(prev => {
          const filtered = prev.filter(term => term !== newSearchTerm);
          return [newSearchTerm, ...filtered].slice(0, 10); // Keep last 10 searches
        });
      }

      return results;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Search failed';
      setSearchError(errorMessage);
      setSearchResults([]);
      throw error;
    } finally {
      setIsSearching(false);
    }
  }, []);

  // Perform quick search (client-side)
  const performQuickSearch = useCallback((query: string, mediaFiles: MediaFileDto[]) => {
    if (!query.trim()) {
      setSearchResults([]);
      return [];
    }

    const lowercaseQuery = query.toLowerCase();
    const results = mediaFiles.filter(file =>
      file.fileName.toLowerCase().includes(lowercaseQuery) ||
      file.originalFileName.toLowerCase().includes(lowercaseQuery) ||
      file.mimeType.toLowerCase().includes(lowercaseQuery) ||
      (file.metadata && file.metadata.some(meta => 
        meta.metadataValue.toLowerCase().includes(lowercaseQuery)
      ))
    );

    setSearchResults(results);
    return results;
  }, []);

  // Clear search
  const clearSearch = useCallback(() => {
    setSearchTerm('');
    setSearchResults([]);
    setSearchError(null);
  }, []);

  // Update search term
  const updateSearchTerm = useCallback((term: string) => {
    setSearchTerm(term);
  }, []);

  // Add to recent searches
  const addToRecentSearches = useCallback((term: string) => {
    if (term.trim()) {
      setRecentSearches(prev => {
        const filtered = prev.filter(t => t !== term);
        return [term, ...filtered].slice(0, 5); // Keep last 5 recent searches
      });
    }
  }, []);

  // Remove from search history
  const removeFromSearchHistory = useCallback((term: string) => {
    setSearchHistory(prev => prev.filter(t => t !== term));
  }, []);

  // Clear search history
  const clearSearchHistory = useCallback(() => {
    setSearchHistory([]);
  }, []);

  // Get search suggestions based on current input
  const getSearchSuggestions = useCallback((input: string, mediaFiles: MediaFileDto[]) => {
    if (!input.trim() || input.length < 2) return [];

    const lowercaseInput = input.toLowerCase();
    const suggestions = new Set<string>();

    // Add matching file names
    mediaFiles.forEach(file => {
      if (file.fileName.toLowerCase().includes(lowercaseInput)) {
        suggestions.add(file.fileName);
      }
      if (file.originalFileName.toLowerCase().includes(lowercaseInput)) {
        suggestions.add(file.originalFileName);
      }
    });

    // Add matching MIME types
    mediaFiles.forEach(file => {
      if (file.mimeType.toLowerCase().includes(lowercaseInput)) {
        suggestions.add(file.mimeType);
      }
    });

    // Add from search history
    searchHistory.forEach(term => {
      if (term.toLowerCase().includes(lowercaseInput)) {
        suggestions.add(term);
      }
    });

    return Array.from(suggestions).slice(0, 8); // Limit to 8 suggestions
  }, [searchHistory]);

  // Check if search has results
  const hasSearchResults = useMemo(() => {
    return searchResults.length > 0;
  }, [searchResults]);

  // Get search result count
  const searchResultCount = useMemo(() => {
    return searchResults.length;
  }, [searchResults]);

  // Check if search is active
  const isSearchActive = useMemo(() => {
    return searchTerm.trim().length > 0;
  }, [searchTerm]);

  return {
    // State
    searchTerm,
    searchResults,
    isSearching,
    searchError,
    searchHistory,
    recentSearches,
    hasSearchResults,
    searchResultCount,
    isSearchActive,

    // Actions
    setSearchTerm,
    performSearch,
    performQuickSearch,
    clearSearch,
    updateSearchTerm,
    addToRecentSearches,
    removeFromSearchHistory,
    clearSearchHistory,
    getSearchSuggestions
  };
};

