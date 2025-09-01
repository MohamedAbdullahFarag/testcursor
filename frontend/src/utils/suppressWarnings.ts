/**
 * React Warning Suppression Utility
 * Temporarily suppresses specific React warnings from external libraries
 * Only active in development mode
 */

// Store original console methods outside the condition
const originalWarn = console.warn;
const originalError = console.error;

// Only apply suppression in development
if (import.meta.env.DEV) {
  // Override console.warn to filter out specific mada-design-system warnings
  console.warn = (...args: any[]) => {
    const message = args.join(' ');
    
    // Suppress key prop warnings from mada-design-system components
    if (message.includes('Warning: Each child in a list should have a unique "key" prop')) {
      // Check if it's from mada-design-system (S23, Footer, K16, or other components)
      if (message.includes('Check the render method of `S23`') ||
          message.includes('mada-design-system') ||
          message.includes('at S23') ||
          message.includes('at Footer') ||
          message.includes('at K16') ||
          message.includes('node_modules/.vite/deps/mada-design-system.js')) {
        // Log a note about the suppression for debugging
        console.debug('🔇 Suppressed mada-design-system key prop warning:', message.split('\n')[0]);
        return; // Suppress this specific warning
      }
    }
    
    // Also suppress any React warnings that mention mada-design-system in the stack trace
    if ((message.includes('Warning:') || message.includes('ReactDOM')) && 
        message.includes('mada-design-system.js')) {
      console.debug('🔇 Suppressed mada-design-system React warning:', message.split('\n')[0]);
      return;
    }
    
    // Allow all other warnings through
    originalWarn.apply(console, args);
  };

  // Also override console.error for React warnings that come through as errors
  console.error = (...args: any[]) => {
    const message = args.join(' ');
    
    // Suppress key prop errors from mada-design-system components
    if (message.includes('Warning: Each child in a list should have a unique "key" prop')) {
      // Check if it's from mada-design-system
      if (message.includes('Check the render method of `S23`') ||
          message.includes('mada-design-system') ||
          message.includes('at S23') ||
          message.includes('at Footer') ||
          message.includes('at K16') ||
          message.includes('node_modules/.vite/deps/mada-design-system.js')) {
        // Log a note about the suppression for debugging
        console.debug('🔇 Suppressed mada-design-system key prop error:', message.split('\n')[0]);
        return; // Suppress this specific error
      }
    }
    
    // Also suppress any React warnings that mention mada-design-system in the stack trace
    if ((message.includes('Warning:') || message.includes('ReactDOM')) && 
        message.includes('mada-design-system.js')) {
      console.debug('🔇 Suppressed mada-design-system React error:', message.split('\n')[0]);
      return;
    }
    
    // Allow all other errors through
    originalError.apply(console, args);
  };
}

// Export for potential cleanup in tests
export const restoreConsoleWarn = () => {
  console.warn = originalWarn;
  console.error = originalError;
};

export default {};
