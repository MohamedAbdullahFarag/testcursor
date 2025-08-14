import { Else, If, Then } from 'react-if'
import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuthStore } from '@/modules/auth/store/authStore'
import { UserTypes } from '../models/enums'

export const ProtectRoute = ({ roles }: { roles: UserTypes[] }) => {
    const { user } = useAuthStore()
    const { state } = useLocation() as { state: { ServiceName?: { NameAr?: string; NameEn?: string; Id?: number } } }

    return (
        <If condition={user?.roleCode && roles.some(role => role === user?.roleCode)}>
            <Then>
                <Outlet />
            </Then>
            <Else>
                <Navigate to="/dashboard/notallowed" state={{ ServiceName: state?.ServiceName }} />
            </Else>
        </If>
    )
}

export default ProtectRoute
