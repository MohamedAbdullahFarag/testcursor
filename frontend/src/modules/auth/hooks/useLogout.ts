import { authService } from '@/shared/constants/oidcConfig'
import { useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'

function useLogout() {
    const queryClient = useQueryClient()
    const navigate = useNavigate()
    const clearAuth = useAuthStore(state => state.clearAuth)

    const logout = async () => {
        authService.signout().finally(() => {
            clearAuth()
            navigate('/')
        })

        queryClient.clear()
    }

    return logout
}

export default useLogout
