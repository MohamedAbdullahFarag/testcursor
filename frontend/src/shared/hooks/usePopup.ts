import { useState, useCallback, useRef, useEffect } from 'react';

export interface UsePopupOptions {
  /** Initial open state */
  defaultOpen?: boolean;
  /** Callback fired when popup opens */
  onOpen?: () => void;
  /** Callback fired when popup closes */
  onClose?: () => void;
  /** Auto-close after specified milliseconds */
  autoClose?: number;
}

export interface UsePopupReturn {
  /** Whether the popup is open */
  isOpen: boolean;
  /** Open the popup */
  open: () => void;
  /** Close the popup */
  close: () => void;
  /** Toggle the popup open/closed state */
  toggle: () => void;
  /** Set the open state directly */
  setOpen: (open: boolean) => void;
}

/**
 * Custom hook for managing popup state and behavior
 * 
 * Features:
 * - Simple open/close state management
 * - Auto-close functionality with timer
 * - Event callbacks for open/close
 * - Memoized functions to prevent unnecessary re-renders
 * - Cleanup on unmount
 * 
 * @param options - Configuration options
 * @returns Popup state and control functions
 */
export const usePopup = (options: UsePopupOptions = {}): UsePopupReturn => {
  const {
    defaultOpen = false,
    onOpen,
    onClose,
    autoClose
  } = options;

  const [isOpen, setIsOpen] = useState(defaultOpen);
  const autoCloseTimerRef = useRef<NodeJS.Timeout | null>(null);

  // Clear auto-close timer
  const clearAutoCloseTimer = useCallback(() => {
    if (autoCloseTimerRef.current) {
      clearTimeout(autoCloseTimerRef.current);
      autoCloseTimerRef.current = null;
    }
  }, []);

  // Open popup
  const open = useCallback(() => {
    setIsOpen(true);
    onOpen?.();

    // Set auto-close timer if specified
    if (autoClose && autoClose > 0) {
      clearAutoCloseTimer();
      autoCloseTimerRef.current = setTimeout(() => {
        setIsOpen(false);
        onClose?.();
      }, autoClose);
    }
  }, [onOpen, autoClose, clearAutoCloseTimer, onClose]);

  // Close popup
  const close = useCallback(() => {
    clearAutoCloseTimer();
    setIsOpen(false);
    onClose?.();
  }, [onClose, clearAutoCloseTimer]);

  // Toggle popup
  const toggle = useCallback(() => {
    if (isOpen) {
      close();
    } else {
      open();
    }
  }, [isOpen, open, close]);

  // Set open state directly
  const setOpen = useCallback((openState: boolean) => {
    if (openState) {
      open();
    } else {
      close();
    }
  }, [open, close]);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      clearAutoCloseTimer();
    };
  }, [clearAutoCloseTimer]);

  return {
    isOpen,
    open,
    close,
    toggle,
    setOpen
  };
};

export default usePopup;
