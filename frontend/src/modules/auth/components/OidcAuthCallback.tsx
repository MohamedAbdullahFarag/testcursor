/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { PageLoader } from '@/shared/components/PageLoader'
import { authService } from '@/shared/constants/oidcConfig'
import { hasAuthParams } from 'mada-design-system'
import { useEffect } from 'react'
import { createPortal } from 'react-dom'
import { useAuthStore } from '../store/authStore'

const OidcAuthCallback = () => {
    const login = useAuthStore(state => state?.login)

    useEffect(() => {
        if (hasAuthParams()) {
            authService.completeSignIn().then((user: any) => {
                console.log(user)
                console.log(user?.profile)
                console.log(user?.profile?.['NationalID'])
                if (user?.id_token && user?.profile['NationalID']) {
                    localStorage.setItem('accessToken', user?.id_token)
                    login({
                        user: { email: user?.profile?.sub, id: user?.profile?.['NationalID'] as string, name: '' },
                        accessToken: user?.id_token,
                        refreshToken: '',
                    })
                    return user
                }
            })
        } else {
            console.log('something went wrong')
        }
    }, [])

    return createPortal(
        <div className="fixed inset-0 z-[10000] flex items-center justify-center bg-white">
            <PageLoader />
        </div>,
        document.body,
    )
}

export default OidcAuthCallback
