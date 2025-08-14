import { renderHook, act } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { useMediaQuery } from '../../hooks/useMediaQuery'

// Mock matchMedia
const mockMatchMedia = (matches: boolean) => {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: vi.fn().mockImplementation((query) => ({
      matches,
      media: query,
      onchange: null,
      addListener: vi.fn(), // deprecated
      removeListener: vi.fn(), // deprecated
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })),
  })
}

describe('useMediaQuery Hook', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('should return true when media query matches', () => {
    mockMatchMedia(true)
    
    const { result } = renderHook(() => useMediaQuery('(min-width: 768px)'))

    expect(result.current).toBe(true)
  })

  it('should return false when media query does not match', () => {
    mockMatchMedia(false)
    
    const { result } = renderHook(() => useMediaQuery('(min-width: 768px)'))

    expect(result.current).toBe(false)
  })

  it('should call matchMedia with the correct query', () => {
    const matchMediaSpy = vi.fn().mockReturnValue({
      matches: true,
      media: '(min-width: 768px)',
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })
    
    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: matchMediaSpy,
    })

    renderHook(() => useMediaQuery('(min-width: 768px)'))

    expect(matchMediaSpy).toHaveBeenCalledWith('(min-width: 768px)')
  })

  it('should handle common breakpoint queries correctly', () => {
    // Test mobile
    mockMatchMedia(true)
    const { result: mobileResult } = renderHook(() => 
      useMediaQuery('(max-width: 767px)')
    )
    expect(mobileResult.current).toBe(true)

    // Test tablet
    const { result: tabletResult } = renderHook(() => 
      useMediaQuery('(min-width: 768px) and (max-width: 1023px)')
    )
    expect(tabletResult.current).toBe(true)

    // Test desktop
    const { result: desktopResult } = renderHook(() => 
      useMediaQuery('(min-width: 1024px)')
    )
    expect(desktopResult.current).toBe(true)
  })

  it('should handle orientation queries', () => {
    mockMatchMedia(true)
    
    const { result: portraitResult } = renderHook(() => 
      useMediaQuery('(orientation: portrait)')
    )
    expect(portraitResult.current).toBe(true)

    const { result: landscapeResult } = renderHook(() => 
      useMediaQuery('(orientation: landscape)')
    )
    expect(landscapeResult.current).toBe(true)
  })

  it('should handle prefers-color-scheme queries', () => {
    mockMatchMedia(true)
    
    const { result: darkResult } = renderHook(() => 
      useMediaQuery('(prefers-color-scheme: dark)')
    )
    expect(darkResult.current).toBe(true)

    const { result: lightResult } = renderHook(() => 
      useMediaQuery('(prefers-color-scheme: light)')
    )
    expect(lightResult.current).toBe(true)
  })

  it('should update when media query changes', () => {
    const mediaQueryList: any = {
      matches: false,
      media: '(min-width: 768px)',
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
    }

    const matchMediaSpy = vi.fn().mockReturnValue(mediaQueryList)
    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: matchMediaSpy,
    })

    const { result, rerender } = renderHook(() => useMediaQuery('(min-width: 768px)'))

    expect(result.current).toBe(false)

    // Simulate media query change
    mediaQueryList.matches = true
    const changeHandler = mediaQueryList.addEventListener.mock.calls[0][1]
    
    act(() => {
      changeHandler({ matches: true })
    })

    expect(result.current).toBe(true)
  })

  it('should clean up event listeners on unmount', () => {
    const removeEventListenerSpy = vi.fn()
    const mediaQueryList = {
      matches: true,
      media: '(min-width: 768px)',
      addEventListener: vi.fn(),
      removeEventListener: removeEventListenerSpy,
    }

    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: vi.fn().mockReturnValue(mediaQueryList),
    })

    const { unmount } = renderHook(() => useMediaQuery('(min-width: 768px)'))

    unmount()

    expect(removeEventListenerSpy).toHaveBeenCalledWith('change', expect.any(Function))
  })

  it('should return false when matchMedia is not supported', () => {
    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: undefined,
    })

    const { result } = renderHook(() => useMediaQuery('(min-width: 768px)'))

    expect(result.current).toBe(false)
  })
})
