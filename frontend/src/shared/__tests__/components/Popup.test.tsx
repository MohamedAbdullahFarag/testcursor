import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { I18nextProvider } from 'react-i18next';
import i18n from '../../i18n';
import Popup from '../Popup';

// Mock portal container
const mockPortalContainer = document.createElement('div');
document.body.appendChild(mockPortalContainer);

// Test wrapper with i18n provider
const TestWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <I18nextProvider i18n={i18n}>
    {children}
  </I18nextProvider>
);

describe('Popup Component', () => {
  const defaultProps = {
    isOpen: true,
    onClose: jest.fn(),
    title: 'Test Popup',
    children: <div>Test Content</div>
  };

  beforeEach(() => {
    jest.clearAllMocks();
    // Reset body styles
    document.body.style.overflow = '';
    document.body.classList.remove('popup-no-scroll');
  });

  afterEach(() => {
    // Cleanup any remaining popups
    document.body.style.overflow = '';
    document.body.classList.remove('popup-no-scroll');
  });

  describe('Basic Rendering', () => {
    it('should render popup when isOpen is true', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(screen.getByText('Test Popup')).toBeInTheDocument();
      expect(screen.getByText('Test Content')).toBeInTheDocument();
      expect(screen.getByTestId('popup-overlay')).toBeInTheDocument();
      expect(screen.getByTestId('popup-content')).toBeInTheDocument();
    });

    it('should not render popup when isOpen is false', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} isOpen={false}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(screen.queryByText('Test Popup')).not.toBeInTheDocument();
      expect(screen.queryByText('Test Content')).not.toBeInTheDocument();
    });

    it('should render without title when title is not provided', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} title={undefined}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(screen.queryByRole('heading')).not.toBeInTheDocument();
      expect(screen.getByText('Test Content')).toBeInTheDocument();
    });

    it('should render description for accessibility', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} description="Test Description">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(screen.getByText('Test Description')).toBeInTheDocument();
      expect(screen.getByText('Test Description')).toHaveClass('sr-only');
    });
  });

  describe('Size Variants', () => {
    const sizes = ['sm', 'md', 'lg', 'xl', '2xl', '4xl'] as const;

    sizes.forEach(size => {
      it(`should apply correct size class for ${size}`, () => {
        render(
          <TestWrapper>
            <Popup {...defaultProps} size={size}>
              <div>Test Content</div>
            </Popup>
          </TestWrapper>
        );

        const content = screen.getByTestId('popup-content');
        expect(content).toHaveClass(`popup-content--${size}`);
      });
    });
  });

  describe('Animation Variants', () => {
    it('should apply scale animation by default', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveClass('popup-visible');
    });

    it('should apply fade animation when specified', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} animation="fade">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveClass('popup-enter');
    });

    it('should not apply animation classes when animation is none', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} animation="none">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).not.toHaveClass('popup-visible');
      expect(content).not.toHaveClass('popup-enter');
      expect(content).not.toHaveClass('popup-exit');
    });
  });

  describe('User Interactions', () => {
    it('should call onClose when close button is clicked', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const closeButton = screen.getByLabelText(/close popup/i);
      await userEvent.click(closeButton);

      expect(onClose).toHaveBeenCalledTimes(1);
    });

    it('should call onClose when overlay is clicked', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      await userEvent.click(overlay);

      expect(onClose).toHaveBeenCalledTimes(1);
    });

    it('should not call onClose when content is clicked', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      await userEvent.click(content);

      expect(onClose).not.toHaveBeenCalled();
    });

    it('should not call onClose when overlay is clicked and closeOnOverlayClick is false', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose} closeOnOverlayClick={false}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      await userEvent.click(overlay);

      expect(onClose).not.toHaveBeenCalled();
    });
  });

  describe('Keyboard Navigation', () => {
    it('should call onClose when Escape key is pressed', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      await userEvent.keyboard('{Escape}');

      expect(onClose).toHaveBeenCalledTimes(1);
    });

    it('should not call onClose when Escape key is pressed and closeOnEscape is false', async () => {
      const onClose = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onClose={onClose} closeOnEscape={false}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      await userEvent.keyboard('{Escape}');

      expect(onClose).not.toHaveBeenCalled();
    });

    it('should focus the popup content when opened', async () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      await waitFor(() => {
        const content = screen.getByTestId('popup-content');
        expect(content).toHaveFocus();
      });
    });
  });

  describe('Body Scroll Prevention', () => {
    it('should prevent body scroll when popup is open', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(document.body.style.overflow).toBe('hidden');
      expect(document.body).toHaveClass('popup-no-scroll');
    });

    it('should restore body scroll when popup is closed', () => {
      const { rerender } = render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(document.body.style.overflow).toBe('hidden');

      rerender(
        <TestWrapper>
          <Popup {...defaultProps} isOpen={false}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(document.body.style.overflow).toBe('');
      expect(document.body).not.toHaveClass('popup-no-scroll');
    });
  });

  describe('RTL Support', () => {
    beforeEach(() => {
      // Mock Arabic language
      i18n.changeLanguage('ar');
    });

    afterEach(() => {
      // Reset to English
      i18n.changeLanguage('en');
    });

    it('should apply RTL direction for Arabic language', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveAttribute('dir', 'rtl');
    });

    it('should use Arabic close button label', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const closeButton = screen.getByLabelText('إغلاق النافذة');
      expect(closeButton).toBeInTheDocument();
    });
  });

  describe('Custom Styling', () => {
    it('should apply custom className to content', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} className="custom-popup">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveClass('custom-popup');
    });

    it('should apply custom overlayClassName to overlay', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} overlayClassName="custom-overlay">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      expect(overlay).toHaveClass('custom-overlay');
    });

    it('should apply custom z-index when specified', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} zIndex={10000}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      expect(overlay).toHaveStyle({ zIndex: '10000' });
    });
  });

  describe('Event Callbacks', () => {
    it('should call onOpen when popup opens', () => {
      const onOpen = jest.fn();
      render(
        <TestWrapper>
          <Popup {...defaultProps} onOpen={onOpen}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      expect(onOpen).toHaveBeenCalledTimes(1);
    });

    it('should call onClosed after popup closes', async () => {
      const onClosed = jest.fn();
      const { rerender } = render(
        <TestWrapper>
          <Popup {...defaultProps} onClosed={onClosed}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      rerender(
        <TestWrapper>
          <Popup {...defaultProps} isOpen={false} onClosed={onClosed}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      await waitFor(() => {
        expect(onClosed).toHaveBeenCalledTimes(1);
      }, { timeout: 250 });
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA attributes', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} description="Test Description">
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveAttribute('role', 'dialog');
      expect(content).toHaveAttribute('aria-modal', 'true');
      expect(content).toHaveAttribute('aria-labelledby', 'popup-title');
      expect(content).toHaveAttribute('aria-describedby', 'popup-description');
    });

    it('should support custom ARIA attributes', () => {
      render(
        <TestWrapper>
          <Popup 
            {...defaultProps} 
            role="alertdialog"
            ariaLabelledBy="custom-title"
            ariaDescribedBy="custom-description"
          >
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const content = screen.getByTestId('popup-content');
      expect(content).toHaveAttribute('role', 'alertdialog');
      expect(content).toHaveAttribute('aria-labelledby', 'custom-title');
      expect(content).toHaveAttribute('aria-describedby', 'custom-description');
    });

    it('should hide close button when showCloseButton is false', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} showCloseButton={false}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const closeButton = screen.queryByLabelText(/close popup/i);
      expect(closeButton).not.toBeInTheDocument();
    });
  });

  describe('Portal Rendering', () => {
    it('should render in document.body by default', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      expect(overlay.parentElement).toBe(document.body);
    });

    it('should render in custom portal container when specified', () => {
      render(
        <TestWrapper>
          <Popup {...defaultProps} portalContainer={mockPortalContainer}>
            <div>Test Content</div>
          </Popup>
        </TestWrapper>
      );

      const overlay = screen.getByTestId('popup-overlay');
      expect(overlay.parentElement).toBe(mockPortalContainer);
    });
  });
});
