import React, { useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from 'mada-design-system';
import { useTranslation } from 'react-i18next';

export interface RTLDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title?: string;
  description?: string;
  children: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '4xl';
  className?: string;
  contentClassName?: string;
}

/**
 * Enhanced Dialog wrapper for mada-design-system with RTL positioning fixes
 * 
 * This component wraps the mada-design-system Dialog and applies positioning fixes
 * to ensure modals appear correctly in both RTL and LTR layouts.
 */
const RTLDialog: React.FC<RTLDialogProps> = ({
  open,
  onOpenChange,
  title,
  description,
  children,
  size = 'md',
  className = '',
  contentClassName = '',
}) => {
  const { i18n } = useTranslation();
  const isRTL = i18n.language === 'ar';

  // Size classes for consistent sizing
  const sizeClasses = {
    sm: 'sm:max-w-sm',
    md: 'sm:max-w-md',
    lg: 'sm:max-w-lg',
    xl: 'sm:max-w-xl',
    '2xl': 'sm:max-w-2xl',
    '4xl': 'sm:max-w-4xl',
  };

  // Apply positioning fixes when dialog opens
  useEffect(() => {
    if (open) {
      // Add a small delay to ensure the dialog is rendered before applying fixes
      const timer = setTimeout(() => {
        // Find all dialog elements and apply positioning fixes
        const dialogs = document.querySelectorAll('[role="dialog"]');
        dialogs.forEach((dialog) => {
          const element = dialog as HTMLElement;
          element.style.left = 'auto';
          element.style.right = 'auto';
          element.style.transform = 'none';
          element.style.margin = 'auto';
          element.style.position = 'relative';
        });

        // Fix overlay positioning for Radix UI dialogs
        const overlays = document.querySelectorAll('[data-radix-portal] > div[data-state="open"], [data-radix-dialog-overlay]');
        overlays.forEach((overlay) => {
          const element = overlay as HTMLElement;
          element.style.position = 'fixed';
          element.style.inset = '0';
          element.style.left = '0';
          element.style.right = '0';
          element.style.top = '0';
          element.style.bottom = '0';
          element.style.display = 'flex';
          element.style.alignItems = 'center';
          element.style.justifyContent = 'center';
          element.style.zIndex = '9999';
        });

        // Fix content positioning
        const contents = document.querySelectorAll('[data-radix-dialog-content]');
        contents.forEach((content) => {
          const element = content as HTMLElement;
          element.style.left = 'auto';
          element.style.right = 'auto';
          element.style.transform = 'none';
          element.style.margin = 'auto';
          element.style.position = 'relative';
          element.style.maxHeight = '90vh';
          element.style.overflowY = 'auto';
        });
      }, 10);

      return () => clearTimeout(timer);
    }
  }, [open]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent 
        className={`
          ${sizeClasses[size]} max-h-[90vh] overflow-y-auto
          ${isRTL ? 'text-right' : 'text-left'}
          ${contentClassName}
        `}
        style={{
          left: 'auto',
          right: 'auto',
          transform: 'none',
          margin: 'auto',
          position: 'relative',
        }}
      >
        {(title || description) && (
          <DialogHeader className={isRTL ? 'text-right' : 'text-left'}>
            {title && (
              <DialogTitle className={isRTL ? 'text-right' : 'text-left'}>
                {title}
              </DialogTitle>
            )}
            {description && (
              <DialogDescription className={isRTL ? 'text-right' : 'text-left'}>
                {description}
              </DialogDescription>
            )}
          </DialogHeader>
        )}
        
        <div className={`${isRTL ? 'text-right' : 'text-left'} ${className}`}>
          {children}
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default RTLDialog;
