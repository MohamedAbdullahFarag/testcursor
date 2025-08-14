import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { CheckCircle as CheckCircleFilled } from 'google-material-icons/filled'
import { CheckCircle, HighlightOff, Info, NewLabel, Schedule, Timeline } from 'google-material-icons/outlined'
import { Badge } from 'mada-design-system'
import React from 'react'
import { StatusCode } from '../models/interfaces'

type StatusDetails = {
    color: 'secondary' | 'tertiary' | 'error' | 'warning' | 'primary' | 'info' | 'gray' | 'success'
    icon: typeof Info
    status: string
}

const status: Record<StatusCode, StatusDetails> = {
    1: { color: 'secondary', icon: NewLabel, status: strings.caseStatus.new },
    2: { color: 'warning', icon: Timeline, status: strings.caseStatus.inProgress },
    3: { color: 'success', icon: CheckCircleFilled, status: strings.caseStatus.closed },
    13: { color: 'info', icon: Schedule, status: strings.caseStatus.onHold },
    6: { color: 'success', icon: CheckCircle, status: strings.caseStatus.resolved },
    7: { color: 'gray', icon: HighlightOff, status: strings.caseStatus.cancelled },
}
const StatusBadge = ({ statusCode, size = 'sm' }: { statusCode: StatusCode; size?: 'sm' | 'default' }) => {
    return (
        <Badge
            variant={'outline'}
            colors={status[statusCode].color}
            className={classNames({
                'max-w-max gap-space-01': true,
            })}
            size={size}>
            {React.createElement(status[statusCode].icon, {
                className: classNames({ 'size-space-04': size === 'sm', 'size-space-05': size === 'default' }),
            })}
            <span className="font-bold">{status[statusCode]?.status}</span>
        </Badge>
    )
}

export default StatusBadge
