import { pathNames } from '@/shared/constants/pathNames'
import { Navigate, Outlet } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import { AuthProvider } from '../hooks/useAuth'

const PortalProvider = () => {
    const isAuthenticated = useAuthStore(state => state?.isAuthenticated)
    

    if (isAuthenticated) return <Navigate to={pathNames.dashboard} />

    return (
        <AuthProvider>
            <Outlet />
        </AuthProvider>
    )
}

export default PortalProvider
