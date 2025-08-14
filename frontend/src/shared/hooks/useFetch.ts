import { useState, useEffect, useCallback, useRef } from 'react';

export interface UseFetchOptions<T> {
  initialData?: T;
  immediate?: boolean;
  onSuccess?: (data: T) => void;
  onError?: (error: Error) => void;
  retries?: number;
  retryDelay?: number;
}

export interface UseFetchReturn<T> {
  data: T | null;
  loading: boolean;
  error: Error | null;
  execute: () => Promise<T | null>;
  reset: () => void;
}

/**
 * Custom hook for fetching data with loading, error states, and retry logic
 * Follows SRP: Only handles data fetching concerns
 * 
 * @param fetcher - Function that returns a Promise with the data
 * @param options - Configuration options for the fetch behavior
 * @returns Object with data, loading state, error state, and control functions
 */
export function useFetch<T>(
  fetcher: () => Promise<T>,
  options: UseFetchOptions<T> = {}
): UseFetchReturn<T> {
  const {
    initialData = null,
    immediate = true,
    onSuccess,
    onError,
    retries = 0,
    retryDelay = 1000,
  } = options;

  const [data, setData] = useState<T | null>(initialData);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<Error | null>(null);
  
  // Use ref to track if component is mounted to prevent state updates after unmount
  const isMountedRef = useRef<boolean>(true);
  const abortControllerRef = useRef<AbortController | null>(null);

  // Cleanup function to abort ongoing requests
  const cleanup = useCallback(() => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
      abortControllerRef.current = null;
    }
  }, []);

  // Execute the fetch operation with retry logic
  const execute = useCallback(async (): Promise<T | null> => {
    // Cancel any ongoing request
    cleanup();
    
    // Create new abort controller for this request
    abortControllerRef.current = new AbortController();
    
    setLoading(true);
    setError(null);

    let attempt = 0;
    let lastError: Error;

    while (attempt <= retries) {
      try {
        const result = await fetcher();
        
        // Only update state if component is still mounted
        if (isMountedRef.current) {
          setData(result);
          setLoading(false);
          onSuccess?.(result);
        }
        
        return result;
      } catch (err) {
        lastError = err instanceof Error ? err : new Error(String(err));
        
        // If it's an abort error, don't retry
        if (lastError.name === 'AbortError') {
          break;
        }
        
        attempt++;
        
        // If we have more retries left, wait before retrying
        if (attempt <= retries) {
          await new Promise(resolve => setTimeout(resolve, retryDelay));
        }
      }
    }

    // Only update state if component is still mounted
    if (isMountedRef.current) {
      setError(lastError!);
      setLoading(false);
      onError?.(lastError!);
    }

    return null;
  }, [fetcher, retries, retryDelay, onSuccess, onError, cleanup]);

  // Reset function to clear data and error state
  const reset = useCallback(() => {
    cleanup();
    setData(initialData);
    setError(null);
    setLoading(false);
  }, [initialData, cleanup]);

  // Execute immediately if requested
  useEffect(() => {
    if (immediate) {
      execute();
    }
    
    // Cleanup on unmount
    return () => {
      isMountedRef.current = false;
      cleanup();
    };
  }, [immediate, execute, cleanup]);

  // Update mounted status on unmount
  useEffect(() => {
    return () => {
      isMountedRef.current = false;
    };
  }, []);

  return {
    data,
    loading,
    error,
    execute,
    reset,
  };
}

// Convenience hook for GET requests with caching
export interface UseFetchWithCacheOptions<T> extends UseFetchOptions<T> {
  cacheKey?: string;
  cacheTime?: number; // Cache time in milliseconds
}

/**
 * Enhanced useFetch hook with simple in-memory caching
 * Useful for data that doesn't change frequently
 */
export function useFetchWithCache<T>(
  fetcher: () => Promise<T>,
  options: UseFetchWithCacheOptions<T> = {}
): UseFetchReturn<T> {
  const { cacheKey, cacheTime = 5 * 60 * 1000, ...fetchOptions } = options; // Default 5 minutes cache

  // Simple in-memory cache (in a real app, consider using a more sophisticated solution)
  const cache = useRef<Map<string, { data: T; timestamp: number }>>(new Map());

  const cachedFetcher = useCallback(async (): Promise<T> => {
    // If no cache key provided, fetch normally
    if (!cacheKey) {
      return fetcher();
    }

    // Check if we have cached data that's still valid
    const cached = cache.current.get(cacheKey);
    if (cached && Date.now() - cached.timestamp < cacheTime) {
      return cached.data;
    }

    // Fetch new data
    const data = await fetcher();
    
    // Cache the result
    cache.current.set(cacheKey, {
      data,
      timestamp: Date.now(),
    });

    return data;
  }, [fetcher, cacheKey, cacheTime]);

  return useFetch(cachedFetcher, fetchOptions);
}

export default useFetch;
