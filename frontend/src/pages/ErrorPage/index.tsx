import ErrorImage from '@/assets/images/error/Error.png'
import { MainPanel } from '@/shared/components'

import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { accessibilityTools, Button, Stack } from 'mada-design-system'
import { ScrollRestoration, useNavigate, useRouteError } from 'react-router-dom'

const ErrorPage = () => {
    const isActive = accessibilityTools(state => state.isActive)
    const naviagte = useNavigate()
    const error = useRouteError() as string
    console.log(error)

    return (
        <MainPanel className="h-screen">
            <Stack
                direction={'col'}
                justifyContent={'center'}
                alignItems={'center'}
                className={"absolute inset-0 rotate-180 bg-[url('@/assets/images/HeroSection-Background.svg')] bg-cover bg-no-repeat"}>
                <Stack gap={3} direction={'col'} alignItems={'center'} justifyContent={'center'} className={'rotate-180'}>
                    <ScrollRestoration />

                    <Stack
                        gap={'none'}
                        className={classNames({
                            'flex-grow': true,
                            grayscale: isActive,
                        })}>
                        <Stack direction={'col'} gap={6} justifyContent={'center'} alignItems={'center'}>
                            <h1>{strings.shared.serverError}</h1>
                            <img src={ErrorImage} alt="ErrorImage" />
                            <Button colors="primary" onClick={() => naviagte('/')}>
                                {strings.shared.backToHome}
                            </Button>
                        </Stack>
                    </Stack>
                </Stack>
            </Stack>
        </MainPanel>
    )
}

export default ErrorPage
