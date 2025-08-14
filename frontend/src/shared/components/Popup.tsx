import React, { useEffect, useRef, useState, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { useTranslation } from 'react-i18next';

export interface PopupProps {
  /** Whether the popup is open */
  isOpen: boolean;
  /** Callback fired when popup should close */
  onClose: () => void;
  /** Popup title */
  title?: string;
  /** Popup description (for accessibility) */
  description?: string;
  /** Popup content */
  children: React.ReactNode;
  /** Size variant */
  size?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '4xl';
  /** Whether clicking overlay closes popup */
  closeOnOverlayClick?: boolean;
  /** Whether escape key closes popup */
  closeOnEscape?: boolean;
  /** Whether to show close button */
  showCloseButton?: boolean;
  /** Additional CSS classes for content */
  className?: string;
  /** Additional CSS classes for overlay */
  overlayClassName?: string;
  /** Animation variant */
  animation?: 'fade' | 'scale' | 'none';
  /** Custom portal container */
  portalContainer?: HTMLElement;
  /** Callback fired when popup opens */
  onOpen?: () => void;
  /** Callback fired after popup closes */
  onClosed?: () => void;
  /** Accessibility role override */
  role?: string;
  /** ARIA labelledby override */
  ariaLabelledBy?: string;
  /** ARIA describedby override */
  ariaDescribedBy?: string;
  /** Z-index override */
  zIndex?: number;
}

/**
 * Consolidated Popup component with comprehensive RTL/LTR support
 * 
 * Features:
 * - Full RTL/LTR internationalization support
 * - Configurable animations and transitions
 * - Comprehensive accessibility features
 * - Focus management and restoration
 * - Keyboard navigation support
 * - Portal rendering for proper z-index stacking
 * - Responsive design with multiple size variants
 * - Custom styling through CSS classes
 * - Body scroll prevention
 * - Escape key handling
 * - Overlay click handling
 * 
 * This component consolidates Modal.tsx, RTLModal.tsx, and RTLDialog.tsx
 * into a single, maintainable component following the established patterns.
 */
const Popup: React.FC<PopupProps> = ({
  isOpen,
  onClose,
  title,
  description,
  children,
  size = 'md',
  closeOnOverlayClick = true,
  closeOnEscape = true,
  showCloseButton = true,
  className = '',
  overlayClassName = '',
  animation = 'scale',
  portalContainer,
  onOpen,
  onClosed,
  role = 'dialog',
  ariaLabelledBy,
  ariaDescribedBy,
  zIndex,
}) => {
  const popupRef = useRef<HTMLDivElement>(null);
  const previousFocusRef = useRef<HTMLElement | null>(null);
  const [isAnimating, setIsAnimating] = useState(false);
  const [shouldRender, setShouldRender] = useState(isOpen);
  
  const { i18n } = useTranslation();
  const isRTL = i18n.language === 'ar';

  // Handle open/close state changes
  useEffect(() => {
    if (isOpen) {
      setShouldRender(true);
      setIsAnimating(true);
      onOpen?.();
      
      // Small delay to ensure DOM is ready for animation
      setTimeout(() => {
        setIsAnimating(false);
      }, 50);
    } else if (shouldRender) {
      setIsAnimating(true);
      
      // Wait for exit animation before removing from DOM
      setTimeout(() => {
        setShouldRender(false);
        setIsAnimating(false);
        onClosed?.();
      }, animation === 'none' ? 0 : 200);
    }
  }, [isOpen, shouldRender, animation, onOpen, onClosed]);

  // Handle escape key press
  useEffect(() => {
    if (!closeOnEscape || !isOpen) return;

    const handleEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        event.preventDefault();
        onClose();
      }
    };

    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, closeOnEscape, onClose]);

  // Focus management
  useEffect(() => {
    if (isOpen) {
      // Store the previously focused element
      previousFocusRef.current = document.activeElement as HTMLElement;
      
      // Focus the popup when it opens (slight delay for proper rendering)
      setTimeout(() => {
        popupRef.current?.focus();
      }, 10);
      
      // Prevent body scroll
      document.body.style.overflow = 'hidden';
      document.body.classList.add('popup-no-scroll');
    } else {
      // Restore focus when popup closes
      if (previousFocusRef.current) {
        setTimeout(() => {
          previousFocusRef.current?.focus();
        }, 10);
      }
      
      // Restore body scroll
      document.body.style.overflow = '';
      document.body.classList.remove('popup-no-scroll');
    }

    // Cleanup on unmount
    return () => {
      document.body.style.overflow = '';
      document.body.classList.remove('popup-no-scroll');
    };
  }, [isOpen]);

  // Handle overlay click
  const handleOverlayClick = useCallback((event: React.MouseEvent) => {
    if (closeOnOverlayClick && event.target === event.currentTarget) {
      onClose();
    }
  }, [closeOnOverlayClick, onClose]);

  // Handle close button click
  const handleCloseClick = useCallback(() => {
    onClose();
  }, [onClose]);

  // Don't render if not open and not animating
  if (!shouldRender) return null;

  // Get animation classes
  const getAnimationClasses = () => {
    if (animation === 'none') return '';
    
    if (isOpen && !isAnimating) {
      return animation === 'fade' ? 'popup-enter' : 'popup-visible';
    }
    
    if (!isOpen && isAnimating) {
      return 'popup-exit';
    }
    
    return '';
  };

  // Build CSS classes
  const overlayClasses = [
    'popup-overlay',
    overlayClassName
  ].filter(Boolean).join(' ');

  const contentClasses = [
    'popup-content',
    `popup-content--${size}`,
    getAnimationClasses(),
    className
  ].filter(Boolean).join(' ');

  const headerClasses = [
    'popup-header',
    isRTL ? 'flex-row-reverse' : 'flex-row'
  ].filter(Boolean).join(' ');

  const titleClasses = [
    'popup-title',
    isRTL ? 'ml-4' : 'mr-4'
  ].filter(Boolean).join(' ');

  const closeButtonClasses = [
    'popup-close-button',
    isRTL ? 'mr-auto' : 'ml-auto'
  ].filter(Boolean).join(' ');

  const popupContent = (
    <div
      data-testid="popup-overlay"
      className={overlayClasses}
      onClick={handleOverlayClick}
      role="presentation"
      aria-hidden={!isOpen}
      data-state={isOpen ? 'open' : 'closed'}
      style={{
        zIndex: zIndex || undefined,
        ...(isRTL ? { direction: 'rtl' } : { direction: 'ltr' })
      }}
    >
      <div
        ref={popupRef}
        data-testid="popup-content"
        className={contentClasses}
        tabIndex={-1}
        role={role}
        aria-modal="true"
        aria-labelledby={ariaLabelledBy || (title ? 'popup-title' : undefined)}
        aria-describedby={ariaDescribedBy || (description ? 'popup-description' : undefined)}
        dir={isRTL ? 'rtl' : 'ltr'}
      >
        {/* Header */}
        {(title || showCloseButton) && (
          <div className={headerClasses}>
            {title && (
              <h2 
                id="popup-title"
                className={titleClasses}
              >
                {title}
              </h2>
            )}
            {showCloseButton && (
              <button
                type="button"
                onClick={handleCloseClick}
                className={closeButtonClasses}
                aria-label={isRTL ? 'إغلاق النافذة' : 'Close popup'}
              >
                <svg 
                  className="w-6 h-6" 
                  fill="none" 
                  stroke="currentColor" 
                  viewBox="0 0 24 24"
                  aria-hidden="true"
                >
                  <path 
                    strokeLinecap="round" 
                    strokeLinejoin="round" 
                    strokeWidth={2} 
                    d="M6 18L18 6M6 6l12 12" 
                  />
                </svg>
              </button>
            )}
          </div>
        )}

        {/* Description (for accessibility) */}
        {description && (
          <div 
            id="popup-description" 
            className="sr-only"
          >
            {description}
          </div>
        )}

        {/* Content */}
        <div className="popup-body">
          {children}
        </div>
      </div>
    </div>
  );

  // Render popup in portal
  const container = portalContainer || document.body;
  return createPortal(popupContent, container);
};

export default Popup;
