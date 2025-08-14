/**
 * Central export file for all shared models and types
 * This file serves as the single import point for type definitions
 */

import type { ReactNode } from 'react'

// Re-export all API types
export * from './api'

// Re-export existing enum types
export * from './enums'

// Re-export existing interface types  
export * from './interfaces'

// Common utility types
export type Nullable<T> = T | null
export type Optional<T> = T | undefined
export type DeepPartial<T> = {
    [P in keyof T]?: T[P] extends object ? DeepPartial<T[P]> : T[P]
}

/**
 * Event handler types for common interactions
 */
export type EventHandler<T = Event> = (event: T) => void
export type ChangeHandler<T = string> = (value: T) => void
export type SubmitHandler<T = unknown> = (data: T) => void | Promise<void>

/**
 * Component prop types
 */
export interface ComponentProps {
    className?: string
    children?: ReactNode
    testId?: string
}

/**
 * Modal and dialog types
 */
export interface ModalProps extends ComponentProps {
    isOpen: boolean
    onClose: () => void
    title?: string
    size?: 'sm' | 'md' | 'lg' | 'xl'
    closeOnEscape?: boolean
    closeOnOverlayClick?: boolean
}

/**
 * Toast notification types
 */
export interface ToastProps {
    id: string
    type: 'success' | 'error' | 'warning' | 'info'
    title: string
    message?: string
    duration?: number
    actions?: ToastAction[]
}

export interface ToastAction {
    label: string
    onClick: () => void
    variant?: 'primary' | 'secondary'
}
