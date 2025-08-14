import { useAuthStore } from '@/modules/auth/store/authStore'

import classNames from 'classnames'
import { ErrorOutline, Refresh } from 'google-material-icons/outlined'
import { Button, EmptySection } from 'mada-design-system'
import { When } from 'react-if'
import { useLocation, useNavigate } from 'react-router-dom'
import { strings } from '../locales'

interface BaseSeriveErrorProps {
    onUpdate?: () => void
    ServiceSubCategory?: number
    isUnAvailable?: boolean
    CustomMessage?: string
    CustomMessageDesc?: string
    layout?: 'horizontal' | 'vertical'
    background?: 'transparent' | 'white' | 'gray'
    isBordered?: boolean
    className?: string
    isSupprtBtnVisible?: boolean
    noDefaultCategory?: boolean
}
export type SeriveErrorProps =
    | (BaseSeriveErrorProps & {
          isSupprtBtnVisible: true
          ServiceCategory: number
      })
    | (BaseSeriveErrorProps & {
          isSupprtBtnVisible?: false
          ServiceCategory?: number
      })

const ServiceError = ({
    onUpdate = () => {},
    layout = 'vertical',
    background,
    isBordered,
    className,
    isSupprtBtnVisible = true,
    ...props
}: SeriveErrorProps) => {
    const { isAuthenticated } = useAuthStore()

    const { pathname } = useLocation()
    const navigate = useNavigate()

    const onNavigateToSupport = () => {
        navigate(isAuthenticated ? '/dashboard/support' : '/support', {
            state: {
                ServiceCategory: props?.ServiceCategory,
                ServiceSubCategory: props?.ServiceSubCategory,
                location: pathname,
                noDefaultCategory: props?.noDefaultCategory,
            },
        })
    }
    return (
        <EmptySection
            className={classNames('', className)}
            title={props?.CustomMessage ?? (props?.isUnAvailable ? strings.shared.serviceUnavailable : strings.shared.sorryTechnicalError)}
            message={
                props?.CustomMessageDesc ??
                (props?.isUnAvailable ? strings.shared.serviceNotWorkingContactSupport : strings.shared.sorryTechnicalErrorDesc)
            }
            icon={
                <span
                    className={classNames(`inline-block rounded-2 p-space-02`, {
                        'bg-card': background === 'gray',
                        'bg-background': true,
                    })}>
                    <ErrorOutline className="size-space-07" />
                </span>
            }
            layout={layout}
            background={background}
            bordered={isBordered}
            data-testid="service-error">
            <Button id="update" type="button" variant={'outline'} colors="primary" size="sm" onClick={onUpdate}>
                <Refresh className="h-space-05 w-space-05" />
                {strings.shared.refresh}
            </Button>
            <When condition={isSupprtBtnVisible}>
                <Button id="technicalSupportRedirect" type="button" variant="outline" size="sm" onClick={onNavigateToSupport}>
                    {strings.support.help}
                </Button>
            </When>
        </EmptySection>
    )
}

export default ServiceError
