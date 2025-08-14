/**
 * PublicRoute component for routes that should only be accessible when not authenticated
 * Redirects authenticated users to dashboard
 */

import React from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useIsAuthenticated } from '../store/authStore'

interface PublicRouteProps {
    children: React.ReactNode
    redirectTo?: string
}

export const PublicRoute: React.FC<PublicRouteProps> = ({ 
    children, 
    redirectTo = '/dashboard' 
}) => {
    const isAuthenticated = useIsAuthenticated()
    const location = useLocation()

    if (isAuthenticated) {
        // Redirect to dashboard or intended destination
        const from = location.state?.from?.pathname || redirectTo
        return <Navigate to={from} replace />
    }

    return <>{children}</>
}

export default PublicRoute
