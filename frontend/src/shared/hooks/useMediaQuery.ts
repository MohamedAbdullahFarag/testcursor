import { useState, useEffect } from 'react';

/**
 * Custom hook for responsive design that listens to media query changes
 * Follows SRP: Only handles media query matching logic
 * 
 * @param query - The media query string to match against
 * @returns boolean indicating if the media query matches
 */
export function useMediaQuery(query: string): boolean {
  // Initialize state with current match
  const [matches, setMatches] = useState<boolean>(() => {
    // Check if window and matchMedia are available (SSR safety)
    if (typeof window === 'undefined' || !window.matchMedia) {
      return false;
    }
    
    return window.matchMedia(query).matches;
  });

  useEffect(() => {
    // Check if window and matchMedia are available (SSR safety)
    if (typeof window === 'undefined' || !window.matchMedia) {
      return;
    }

    const mediaQueryList = window.matchMedia(query);
    
    // Handler for media query changes
    const handleChange = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    // Set initial value
    setMatches(mediaQueryList.matches);

    // Add event listener
    // Use the newer API if available, fallback to deprecated one
    if (mediaQueryList.addEventListener) {
      mediaQueryList.addEventListener('change', handleChange);
    } else {
      // Fallback for older browsers
      mediaQueryList.addListener(handleChange);
    }

    // Cleanup function
    return () => {
      if (mediaQueryList.removeEventListener) {
        mediaQueryList.removeEventListener('change', handleChange);
      } else {
        // Fallback for older browsers
        mediaQueryList.removeListener(handleChange);
      }
    };
  }, [query]);

  return matches;
}

// Convenience hooks for common breakpoints (following Tailwind CSS conventions)
export const useIsSmall = () => useMediaQuery('(max-width: 639px)');
export const useIsMedium = () => useMediaQuery('(min-width: 640px) and (max-width: 767px)');
export const useIsLarge = () => useMediaQuery('(min-width: 768px) and (max-width: 1023px)');
export const useIsExtraLarge = () => useMediaQuery('(min-width: 1024px) and (max-width: 1279px)');
export const useIs2ExtraLarge = () => useMediaQuery('(min-width: 1280px)');

// Mobile-first approach hooks
export const useIsMobile = () => useMediaQuery('(max-width: 767px)');
export const useIsTablet = () => useMediaQuery('(min-width: 768px) and (max-width: 1023px)');
export const useIsDesktop = () => useMediaQuery('(min-width: 1024px)');

// Orientation hooks
export const useIsPortrait = () => useMediaQuery('(orientation: portrait)');
export const useIsLandscape = () => useMediaQuery('(orientation: landscape)');

// Device-specific hooks
export const useIsTouchDevice = () => useMediaQuery('(hover: none) and (pointer: coarse)');
export const useCanHover = () => useMediaQuery('(hover: hover)');

// Dark mode preference hook
export const usePrefersDarkMode = () => useMediaQuery('(prefers-color-scheme: dark)');

// Reduced motion preference hook
export const usePrefersReducedMotion = () => useMediaQuery('(prefers-reduced-motion: reduce)');

// High contrast preference hook
export const usePrefersHighContrast = () => useMediaQuery('(prefers-contrast: high)');

export default useMediaQuery;
