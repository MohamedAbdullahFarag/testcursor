import classNames from 'classnames'
import React from 'react'
import { When } from 'react-if'
import {NotificationOffGray} from 'streamline-icons'
import { Stack } from 'mada-design-system'
import StreamLineIcon, { StreamLineIconType } from './StreamLineIcon'
interface EmptyStateProps {
    buttons?: JSX.Element[]
    alignment?: 'Vertical' | 'Horizontal'
    showBackground?: boolean
    description?: string
    title: string
    icon?: { dark: StreamLineIconType; light: StreamLineIconType }
}

const EmptyState = ({ buttons, alignment = 'Vertical', showBackground, title, description, icon }: EmptyStateProps) => {
    if (alignment === 'Vertical')
        return (
            <Stack
                direction="col"
                alignItems="center"
                justifyContent="center"
                className={classNames({
                    'h-full md:p-space-04': true,
                    'rounded-3 bg-background': showBackground,
                })}>
                <StreamLineIcon {...(icon ?? { light: NotificationOffGray, dark: NotificationOffGray })} />
                <Stack direction="col" gap={1} alignItems="center" justifyContent="center">
                    <h1 className="text-body-02 font-bold text-card-foreground">{title}</h1>
                    <When condition={!!description}>
                        <p className="text-body-01 text-foreground-secondary">{description}</p>
                    </When>
                </Stack>
                <When condition={!!buttons?.length}>
                    <Stack gap={2} alignItems="center" justifyContent="center">
                        {buttons?.map((btn, i) => <React.Fragment key={'emptyStatBtn' + i}>{btn}</React.Fragment>)}
                    </Stack>
                </When>
            </Stack>
        )

    return (
        <Stack
            alignItems="start"
            gap={3}
            className={classNames({
                'p-space-04': true,
                'bg-background': showBackground,
            })}>
            <StreamLineIcon {...icon} />
            <Stack direction="col" gap={2}>
                <Stack direction="col" gap={1}>
                    <h1 className="text-body-02 font-bold text-card-foreground">{title}</h1>
                    <When condition={!!description}>
                        <p className="text-body-01 text-foreground-secondary">{description}</p>
                    </When>
                </Stack>
                <When condition={!!buttons?.length}>
                    <Stack gap={2} alignItems="center">
                        {buttons?.map((btn, i) => <React.Fragment key={'emptyStatBtn' + i}>{btn}</React.Fragment>)}
                    </Stack>
                </When>
            </Stack>
        </Stack>
    )
}

export default EmptyState
