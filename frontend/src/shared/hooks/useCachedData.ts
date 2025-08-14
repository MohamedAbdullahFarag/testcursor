import { QueryKey, QueryStatus, useIsFetching, useQueryClient } from '@tanstack/react-query'

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
 *
 * @template T - The type of data being queried.
 * @param {QueryClient} queryClient - The queryClient instance.
 * @param {QueryKey} queryKey - The key used to identify the query in the cache.
 * @param {(data: T | undefined) => T | undefined} [transform] - Optional transformation function to apply to the cached data.
 * @returns {QueryState<T>} The state of the query, including the data, status, and any errors.
 */
export function useCachedData<T>(queryKey: QueryKey, transform?: (data: T | undefined) => T | undefined): QueryState<T> {
    const queryClient = useQueryClient()
    // Get the cached data
    const cachedData = queryClient.getQueryData<T>(queryKey)

    // Get the query state
    const queryState = queryClient.getQueryState<T>(queryKey)
    const isFetching = useIsFetching({ queryKey }, queryClient)

    // Apply transformation if needed
    const transformedData = transform ? transform(cachedData) : cachedData

    return {
        data: transformedData,
        status: queryState?.status ?? 'idle', // Default to 'idle' if no status is available
        isError: !!queryState?.error,
        error: queryState?.error, // react-query error
        isFetching: !!isFetching, // Indicates if the query is currently in-progress
        isPending: queryState?.status === 'pending', // Custom pending state if applicable
        isSuccess: queryState?.status === 'success', // Success state if data is available
        refetch: () => {
            queryClient.resetQueries({ queryKey: queryKey })
        },
    }
}
