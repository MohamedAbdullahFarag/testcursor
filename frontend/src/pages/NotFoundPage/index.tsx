import NotFound from '@/assets/images/error/NotFound.webp'
import { useAuthStore } from '@/modules/auth/store/authStore'
import { strings } from '@/shared/locales'
import { Button, Stack } from 'mada-design-system'
import { useNavigate } from 'react-router-dom'
const NotFoundPage = () => {
    const navigate = useNavigate()

    const { isAuthenticated } = useAuthStore()
    return (
        <div className="mb-space-09 flex w-full flex-col">
            <Stack direction={'col'} gap={9} justifyContent={'center'} alignItems={'center'}>
                <img src={NotFound} className="h-[324px] w-[458px]" />
                <div className="flex flex-col items-center gap-space-04">
                    <h1 className="text-subtitle-02 font-bold">{strings.shared.error}</h1>
                    <p className="text-subtitle-02">{strings.shared.errorDescription}</p>
                </div>

                <Button rounded={'default'} onClick={() => navigate(isAuthenticated ? '/dashboard' : '/', { replace: true })}>
                    {strings.shared.backToHome}
                </Button>
            </Stack>
        </div>
    )
}

export default NotFoundPage
