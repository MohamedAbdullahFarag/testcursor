import { type QueryKey, type QueryStatus, useQuery, useQueryClient } from '@tanstack/react-query'

/**
 * Represents the state of a query.
 *
 * @template T - The type of data being queried.
 */
type QueryState<T> = {
    /**
     * The cached data for the query, if available.
     */
    data?: T
    /**
     * The current status of the query.
     * Can be 'idle', "pending" | "error" | "success", or any other status provided by `react-query`.
     */
    status: QueryStatus | 'idle'
    /**
     * Indicates if the query is currently in a pending state.
     * This is a custom state that can be used to differentiate between idle and pending states.
     */
    isPending: boolean
    /**
     * Indicates if the query is currently being fetched.
     * This reflects whether a network request is in progress.
     */
    isFetching: boolean
    /**
     * Indicates if the query has successfully fetched data.
     * This is true if the query has completed successfully and data is available.
     */
    isSuccess: boolean
    /**
     * Indicates if the query has incountered any error.
     */
    isError: boolean
    /**
     * The error associated with the query, if any.
     */
    error?: unknown
    /**
     * refetch quiery based on provided key
     */
    refetch: () => void
}

/**
 * Custom hook to retrieve cached data and its associated state.
 * This version properly subscribes to cache updates.
 *
 * @template T - The type of data being queried.
 * @param {QueryKey} queryKey - The key used to identify the query in the cache.
 * @param {(data: T | undefined) => T | undefined} [transform] - Optional transformation function to apply to the cached data.
 * @returns {QueryState<T>} The state of the query, including the data, status, and any errors.
 */
export function useSubscribeCachedData<T>(queryKey: QueryKey, transform?: (data: T | undefined) => T | undefined): QueryState<T> {
    const queryClient = useQueryClient()

    // Use useQuery to properly subscribe to cache updates
    // This is the key change - we're now using useQuery instead of directly accessing the cache
    const {
        data: cachedData,
        status,
        error,
        isFetching,
        isPending,
        isSuccess,
        isError,
    } = useQuery<T>({
        queryKey,
        // This is important - we're not actually fetching data, just subscribing to the cache
        queryFn: () => queryClient.getQueryData<T>(queryKey) as Promise<T>,
        // Don't refetch on window focus, etc.
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
        // Only execute this query if data already exists in the cache
        enabled: !!queryClient.getQueryData(queryKey),
        // Don't retry on error
        retry: false,
        // Use stale data while revalidating
        staleTime: Number.POSITIVE_INFINITY,
    })

    // Apply transformation if needed
    const transformedData = transform ? transform(cachedData) : cachedData

    return {
        data: transformedData,
        status: status ?? 'idle',
        isError,
        error,
        isFetching,
        isPending: isPending,
        isSuccess,
        refetch: () => {
            queryClient.resetQueries({ queryKey })
        },
    }
}
