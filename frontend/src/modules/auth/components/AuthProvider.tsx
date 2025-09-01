import { pathNames } from '@/shared/constants/pathNames'
import { Navigate, Outlet } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import { sessionRestorationService } from '../services/sessionRestorationService'
import { useEffect } from 'react'

const AuthProvider = () => {
    const isAuthenticated = useAuthStore(state => state?.isAuthenticated)

    useEffect(() => {
        // Initialize session restoration on mount
        sessionRestorationService.initialize();
    }, []);

    if (!isAuthenticated) return <Navigate to={pathNames.portal} />

    return <Outlet />
}

export default AuthProvider
