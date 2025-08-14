import React from 'react';
import Popup, { PopupProps } from './Popup';

export interface RTLModalProps extends Omit<PopupProps, 'role'> {
  /** Legacy prop compatibility */
  onOpenChange?: (open: boolean) => void;
}

/**
 * RTLModal component - Legacy wrapper for the new Popup component
 * 
 * This component maintains backward compatibility with existing RTLModal usage
 * while leveraging the new consolidated Popup component under the hood.
 * 
 * @deprecated Consider using the new Popup component directly for new code
 */
const RTLModal: React.FC<RTLModalProps> = ({
  onOpenChange,
  onClose,
  ...props
}) => {
  const handleClose = () => {
    onClose();
    onOpenChange?.(false);
  };

  return (
    <Popup
      {...props}
      onClose={handleClose}
      role="dialog"
    />
  );
};

export default RTLModal;
