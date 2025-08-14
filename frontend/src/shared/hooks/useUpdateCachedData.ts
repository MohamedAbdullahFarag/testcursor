import { type QueryKey, useQueryClient } from '@tanstack/react-query'
import { type Draft, produce } from 'immer'
import { useCallback } from 'react'

/**
 * A function that updates cached data based on the old data.
 *
 * @template T - The type of the cached data.
 * @param {QueryClient} queryClient - The queryClient instance.
 * @param {T | undefined} oldData - The current cached data or undefined if no data exists.
 * @returns {T} - The updated data.
 */
type UpdaterFunction<T> = (draft: Draft<T> | undefined) => void

/**
 * A custom hook to update cached data in React Query.
 *
 * @template T - The type of the data in the cache.
 * @returns {(key: QueryKey, updater: T | UpdaterFunction<T>) => void} - A function to update the cached data.
 */
export const useUpdateCachedData = <T>() => {
    const queryClient = useQueryClient()
    /**
     * Updates the cached data for a specific query key.
     *
     * @param {QueryKey} key - The key that identifies the cached data to update.
     * @param {T | UpdaterFunction<T>} updater - The new data or a function that returns the updated data.
     */
    return useCallback(
        (key: QueryKey, updater: T | UpdaterFunction<T>) => {
            queryClient.setQueryData<T>(key, oldData => {
                // Apply the updater function to modify the old data
                if (typeof updater !== 'function') return updater

                return produce(oldData, draft => {
                    ;(updater as UpdaterFunction<T>)(draft)
                })
            })
        },
        [queryClient],
    )
}
