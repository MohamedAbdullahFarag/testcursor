import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import Modal from '../../components/Modal'

describe('Modal Component', () => {
  const defaultProps = {
    isOpen: true,
    onClose: vi.fn(),
    title: 'Test Modal'
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should render modal when isOpen is true', () => {
    render(
      <Modal {...defaultProps}>
        <div>Modal Content</div>
      </Modal>
    )

    expect(screen.getByText('Test Modal')).toBeInTheDocument()
    expect(screen.getByText('Modal Content')).toBeInTheDocument()
  })

  it('should not render modal when isOpen is false', () => {
    render(
      <Modal {...defaultProps} isOpen={false}>
        <div>Modal Content</div>
      </Modal>
    )

    expect(screen.queryByText('Test Modal')).not.toBeInTheDocument()
    expect(screen.queryByText('Modal Content')).not.toBeInTheDocument()
  })

  it('should call onClose when close button is clicked', () => {
    render(
      <Modal {...defaultProps}>
        <div>Modal Content</div>
      </Modal>
    )

    const closeButton = screen.getByRole('button', { name: /close/i })
    fireEvent.click(closeButton)

    expect(defaultProps.onClose).toHaveBeenCalledTimes(1)
  })

  it('should call onClose when escape key is pressed', () => {
    render(
      <Modal {...defaultProps} closeOnEscape={true}>
        <div>Modal Content</div>
      </Modal>
    )

    fireEvent.keyDown(document, { key: 'Escape', code: 'Escape' })

    expect(defaultProps.onClose).toHaveBeenCalledTimes(1)
  })

  it('should call onClose when overlay is clicked', () => {
    render(
      <Modal {...defaultProps} closeOnOverlayClick={true}>
        <div>Modal Content</div>
      </Modal>
    )

    const overlay = screen.getByTestId('modal-overlay')
    fireEvent.click(overlay)

    expect(defaultProps.onClose).toHaveBeenCalledTimes(1)
  })

  it('should not call onClose when modal content is clicked', () => {
    render(
      <Modal {...defaultProps} closeOnOverlayClick={true}>
        <div>Modal Content</div>
      </Modal>
    )

    const content = screen.getByText('Modal Content')
    fireEvent.click(content)

    expect(defaultProps.onClose).not.toHaveBeenCalled()
  })

  it('should apply correct size class', () => {
    const { rerender } = render(
      <Modal {...defaultProps} size="sm">
        <div>Content</div>
      </Modal>
    )

    expect(screen.getByTestId('modal-content')).toHaveClass('max-w-sm')

    rerender(
      <Modal {...defaultProps} size="lg">
        <div>Content</div>
      </Modal>
    )

    expect(screen.getByTestId('modal-content')).toHaveClass('max-w-lg')
  })

  it('should have proper accessibility attributes', () => {
    render(
      <Modal {...defaultProps}>
        <div>Modal Content</div>
      </Modal>
    )

    const modal = screen.getByRole('dialog')
    expect(modal).toHaveAttribute('aria-modal', 'true')
    expect(modal).toHaveAttribute('aria-labelledby', expect.stringContaining('modal-title'))
  })
})
