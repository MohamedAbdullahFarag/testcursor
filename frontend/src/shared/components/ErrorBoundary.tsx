import React, { Component, ReactNode } from 'react'

interface ErrorInfo {
  componentStack: string
}

interface ErrorBoundaryProps {
  children: ReactNode
  fallback?: React.ComponentType<{ error: Error; resetError: () => void }>
  fallbackComponent?: ReactNode
  onError?: (error: Error, errorInfo: ErrorInfo) => void
  resetKey?: string | number
}

interface ErrorBoundaryState {
  hasError: boolean
  error: Error | null
}

// Default fallback component
const DefaultFallback: React.FC<{ error: Error; resetError: () => void }> = ({ resetError }) => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 text-center">
        <div>
          <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
            Something went wrong
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            We're sorry, but something unexpected happened.
          </p>
        </div>
        <div>
          <button
            onClick={resetError}
            type="button"
            className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Try again
          </button>
        </div>
      </div>
    </div>
  )
}

class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  private resetTimeoutId: number | null = null

  constructor(props: ErrorBoundaryProps) {
    super(props)
    this.state = { hasError: false, error: null }
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    // Update state so the next render will show the fallback UI
    return { hasError: true, error }
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    // Call the onError callback if provided
    this.props.onError?.(error, errorInfo)
  }

  componentDidUpdate(prevProps: ErrorBoundaryProps) {
    const { resetKey, children } = this.props
    const { hasError } = this.state
    
    // Reset error state when resetKey changes
    if (hasError && prevProps.resetKey !== resetKey) {
      this.setState({ hasError: false, error: null })
    }
    
    // Reset error state when children change and we were in error state
    if (hasError && prevProps.children !== children) {
      this.setState({ hasError: false, error: null })
    }
  }

  componentWillUnmount() {
    if (this.resetTimeoutId) {
      clearTimeout(this.resetTimeoutId)
    }
  }

  resetError = () => {
    this.setState({ hasError: false, error: null })
  }

  render() {
    const { hasError, error } = this.state
    const { children, fallback, fallbackComponent } = this.props

    if (hasError && error) {
      // Use custom fallback component if provided (legacy prop)
      if (fallbackComponent) {
        return fallbackComponent
      }

      // Use custom fallback component if provided
      if (fallback) {
        const FallbackComponent = fallback
        return <FallbackComponent error={error} resetError={this.resetError} />
      }

      // Use default fallback
      return <DefaultFallback error={error} resetError={this.resetError} />
    }

    return children
  }
}

export default ErrorBoundary

// ❌ DON'T: Catch all errors indiscriminately
// ❌ DON'T: Render error boundaries too high in the tree
// ❌ DON'T: Forget to handle error logging
// ❌ DON'T: Skip providing retry functionality
// ❌ DON'T: Use error boundaries for flow control
