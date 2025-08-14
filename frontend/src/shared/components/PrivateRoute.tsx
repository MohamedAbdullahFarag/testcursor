/**
 * PrivateRoute component for routes that require authentication
 * Redirects unauthenticated users to login page
 */

import React from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '@/modules/auth/hooks/useAuth'
import { PageLoader } from './PageLoader'

interface PrivateRouteProps {
    children: React.ReactNode
    redirectTo?: string
    requiredRoles?: string[]
    requiredPermissions?: string[]
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ 
    children, 
    redirectTo = '/login',
    requiredRoles = [],
    requiredPermissions = [] // eslint-disable-line @typescript-eslint/no-unused-vars
}) => {
    const { user, isAuthenticated, isLoading } = useAuth()
    const location = useLocation()

    // Show loading while checking authentication
    if (isLoading) {
        return <PageLoader />
    }

    // Redirect to login if not authenticated
    if (!isAuthenticated || !user) {
        return (
            <Navigate 
                to={redirectTo} 
                state={{ from: location }} 
                replace 
            />
        )
    }

    // Check role-based access if required roles are specified
    if (requiredRoles.length > 0) {
        const hasRequiredRole = requiredRoles.some(role => 
            user.roles?.includes(role)
        )

        if (!hasRequiredRole) {
            return <Navigate to="/unauthorized" replace />
        }
    }

    // TODO: Add permission checking when permission system is implemented
    // if (requiredPermissions.length > 0) {
    //     const hasRequiredPermission = requiredPermissions.some(perm => 
    //         user.permissions?.includes(perm)
    //     )
    //     
    //     if (!hasRequiredPermission) {
    //         return <Navigate to="/unauthorized" replace />
    //     }
    // }

    return <>{children}</>
}

export default PrivateRoute
