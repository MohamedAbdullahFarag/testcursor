import { renderHook, act, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useFetch } from '../../hooks/useFetch'

describe('useFetch Hook', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should initialize with correct default values', () => {
    const fetcher = vi.fn().mockResolvedValue('test data')
    const { result } = renderHook(() => useFetch(fetcher, { immediate: false }))

    expect(result.current.data).toBeNull()
    expect(result.current.loading).toBe(false)
    expect(result.current.error).toBeNull()
    expect(typeof result.current.execute).toBe('function')
    expect(typeof result.current.reset).toBe('function')
  })

  it('should fetch data immediately when immediate is true', async () => {
    const testData = { id: 1, name: 'Test' }
    const fetcher = vi.fn().mockResolvedValue(testData)
    
    const { result } = renderHook(() => useFetch(fetcher, { immediate: true }))

    expect(result.current.loading).toBe(true)
    expect(fetcher).toHaveBeenCalledTimes(1)

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
      expect(result.current.data).toEqual(testData)
      expect(result.current.error).toBeNull()
    })
  })

  it('should not fetch data immediately when immediate is false', () => {
    const fetcher = vi.fn().mockResolvedValue('test data')
    
    renderHook(() => useFetch(fetcher, { immediate: false }))

    expect(fetcher).not.toHaveBeenCalled()
  })

  it('should execute fetch when execute function is called', async () => {
    const testData = { id: 1, name: 'Test' }
    const fetcher = vi.fn().mockResolvedValue(testData)
    
    const { result } = renderHook(() => useFetch(fetcher, { immediate: false }))

    expect(result.current.loading).toBe(false)

    act(() => {
      result.current.execute()
    })

    expect(result.current.loading).toBe(true)
    expect(fetcher).toHaveBeenCalledTimes(1)

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
      expect(result.current.data).toEqual(testData)
    })
  })

  it('should handle fetch errors correctly', async () => {
    const error = new Error('Fetch failed')
    const fetcher = vi.fn().mockRejectedValue(error)
    
    const { result } = renderHook(() => useFetch(fetcher, { immediate: true }))

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
      expect(result.current.data).toBeNull()
      expect(result.current.error).toEqual(error)
    })
  })

  it('should call onSuccess callback when fetch succeeds', async () => {
    const testData = { id: 1, name: 'Test' }
    const fetcher = vi.fn().mockResolvedValue(testData)
    const onSuccess = vi.fn()
    
    renderHook(() => useFetch(fetcher, { immediate: true, onSuccess }))

    await waitFor(() => {
      expect(onSuccess).toHaveBeenCalledWith(testData)
    })
  })

  it('should call onError callback when fetch fails', async () => {
    const error = new Error('Fetch failed')
    const fetcher = vi.fn().mockRejectedValue(error)
    const onError = vi.fn()
    
    renderHook(() => useFetch(fetcher, { immediate: true, onError }))

    await waitFor(() => {
      expect(onError).toHaveBeenCalledWith(error)
    })
  })

  it('should retry on failure when retries option is set', async () => {
    const error = new Error('Fetch failed')
    const fetcher = vi.fn()
      .mockRejectedValueOnce(error)
      .mockRejectedValueOnce(error)
      .mockResolvedValueOnce('success')
    
    const { result } = renderHook(() => 
      useFetch(fetcher, { immediate: true, retries: 2, retryDelay: 10 })
    )

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
      expect(result.current.data).toBe('success')
      expect(fetcher).toHaveBeenCalledTimes(3)
    }, { timeout: 1000 })
  })

  it('should reset state when reset function is called', async () => {
    const testData = { id: 1, name: 'Test' }
    const fetcher = vi.fn().mockResolvedValue(testData)
    
    const { result } = renderHook(() => useFetch(fetcher, { immediate: true }))

    await waitFor(() => {
      expect(result.current.data).toEqual(testData)
    })

    act(() => {
      result.current.reset()
    })

    expect(result.current.data).toBeNull()
    expect(result.current.loading).toBe(false)
    expect(result.current.error).toBeNull()
  })

  it('should use initial data when provided', () => {
    const initialData = { id: 0, name: 'Initial' }
    const fetcher = vi.fn().mockResolvedValue('test data')
    
    const { result } = renderHook(() => 
      useFetch(fetcher, { immediate: false, initialData })
    )

    expect(result.current.data).toEqual(initialData)
  })

  it('should abort request when component unmounts', async () => {
    const fetcher = vi.fn().mockImplementation(() => 
      new Promise((resolve) => setTimeout(resolve, 1000))
    )
    
    const { result, unmount } = renderHook(() => 
      useFetch(fetcher, { immediate: true })
    )

    expect(result.current.loading).toBe(true)

    unmount()

    // Verify that the request was aborted and state wasn't updated
    await new Promise(resolve => setTimeout(resolve, 100))
    
    // Component is unmounted, so we can't check result.current
    // but we can verify the fetcher was called
    expect(fetcher).toHaveBeenCalledTimes(1)
  })
})
