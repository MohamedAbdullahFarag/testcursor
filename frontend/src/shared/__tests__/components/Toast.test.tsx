import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import Toast from '../../components/Toast'

describe('Toast Component', () => {
  const defaultProps = {
    id: 'test-toast',
    type: 'success' as const,
    title: 'Success Message',
    message: 'Operation completed successfully',
    duration: 3000,
    onRemove: vi.fn()
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should render toast with correct content', () => {
    render(<Toast {...defaultProps} />)

    expect(screen.getByText('Success Message')).toBeInTheDocument()
    expect(screen.getByText('Operation completed successfully')).toBeInTheDocument()
  })

  it('should display correct icon for each toast type', () => {
    const { rerender } = render(<Toast {...defaultProps} type="success" />)
    expect(screen.getByTestId('toast-icon')).toHaveClass('text-green-400')

    rerender(<Toast {...defaultProps} type="error" />)
    expect(screen.getByTestId('toast-icon')).toHaveClass('text-red-400')

    rerender(<Toast {...defaultProps} type="warning" />)
    expect(screen.getByTestId('toast-icon')).toHaveClass('text-yellow-400')

    rerender(<Toast {...defaultProps} type="info" />)
    expect(screen.getByTestId('toast-icon')).toHaveClass('text-blue-400')
  })

  it('should call onRemove when close button is clicked', () => {
    render(<Toast {...defaultProps} />)

    const closeButton = screen.getByRole('button', { name: /close/i })
    fireEvent.click(closeButton)

    expect(defaultProps.onRemove).toHaveBeenCalledWith(defaultProps.id)
  })

  it('should auto-remove after duration', async () => {
    vi.useFakeTimers()
    
    render(<Toast {...defaultProps} duration={1000} />)

    expect(defaultProps.onRemove).not.toHaveBeenCalled()

    vi.advanceTimersByTime(1000)

    await waitFor(() => {
      expect(defaultProps.onRemove).toHaveBeenCalledWith(defaultProps.id)
    })

    vi.useRealTimers()
  })

  it('should not auto-remove when duration is 0', async () => {
    vi.useFakeTimers()
    
    render(<Toast {...defaultProps} duration={0} />)

    vi.advanceTimersByTime(5000)

    expect(defaultProps.onRemove).not.toHaveBeenCalled()

    vi.useRealTimers()
  })

  it('should render action buttons when provided', () => {
    const actions = [
      { label: 'Undo', onClick: vi.fn(), variant: 'primary' as const },
      { label: 'Dismiss', onClick: vi.fn(), variant: 'secondary' as const }
    ]

    render(<Toast {...defaultProps} actions={actions} />)

    expect(screen.getByRole('button', { name: 'Undo' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Dismiss' })).toBeInTheDocument()
  })

  it('should call action onClick when action button is clicked', () => {
    const actionClick = vi.fn()
    const actions = [
      { label: 'Undo', onClick: actionClick, variant: 'primary' as const }
    ]

    render(<Toast {...defaultProps} actions={actions} />)

    const actionButton = screen.getByRole('button', { name: 'Undo' })
    fireEvent.click(actionButton)

    expect(actionClick).toHaveBeenCalledTimes(1)
  })

  it('should have proper accessibility attributes', () => {
    render(<Toast {...defaultProps} />)

    const toast = screen.getByRole('alert')
    expect(toast).toBeInTheDocument()
    expect(toast).toHaveAttribute('aria-live', 'polite')
  })

  it('should pause auto-removal on hover', async () => {
    vi.useFakeTimers()
    
    render(<Toast {...defaultProps} duration={1000} />)

    const toast = screen.getByRole('alert')
    
    // Hover over toast
    fireEvent.mouseEnter(toast)
    
    vi.advanceTimersByTime(1000)
    expect(defaultProps.onRemove).not.toHaveBeenCalled()

    // Leave hover
    fireEvent.mouseLeave(toast)
    
    vi.advanceTimersByTime(1000)
    await waitFor(() => {
      expect(defaultProps.onRemove).toHaveBeenCalledWith(defaultProps.id)
    })

    vi.useRealTimers()
  })
})
