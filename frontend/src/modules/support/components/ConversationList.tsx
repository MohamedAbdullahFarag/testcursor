import { EmptyState } from '@/shared/components'
import { MessageGray } from 'streamline-icons'

import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { Send } from 'google-material-icons/filled'
import { HeadsetMic, Person } from 'google-material-icons/outlined'
import { Button, dateFormatter, Input, ScrollArea, ScrollBar, Skeleton, useLanguage } from 'mada-design-system'
import moment from 'moment'
import React, { useState } from 'react'
import { Case, Switch, When } from 'react-if'
import { TICKET_STATUS } from '../models/enums'
import { TicketData } from '../models/interfaces'
import { useCommentOnTicket, useGetTicketComments } from '../services/useServiceNow'

interface ChatMessageProps {
    isUser: boolean
    message: string
    timestamp: string
    date: string
}

const ChatMessage: React.FC<ChatMessageProps> = ({ isUser, message, timestamp, date }) => {
    return (
        <>
            <p className="text-center text-body-01 font-normal">{date}</p>
            <div className={classNames('flex justify-start', { 'flex-row-reverse': !isUser })}>
                <div className="flex flex-col justify-end">
                    <div
                        className={classNames('flex items-center justify-center rounded-full p-space-02', {
                            'me-space-01 bg-secondary-container': isUser,
                            'ms-space-01 bg-primary-container': !isUser,
                        })}>
                        {isUser ? <Person className="text-secondary-oncontainer" /> : <HeadsetMic className="text-primary-oncontainer" />}
                    </div>
                </div>
                <div
                    className={classNames('rounded-lg p-space-03', {
                        'bg-secondary-container': isUser,
                        'bg-primary-container': !isUser,
                    })}>
                    <p className="text-body-02 font-normal">{message}</p>
                    <div
                        className={classNames('text-caption-01 font-normal', {
                            'text-end text-secondary-oncontainer': isUser,
                            'text-start text-primary-dark': !isUser,
                        })}>
                        {timestamp}
                    </div>
                </div>
            </div>
        </>
    )
}

const ConversationList = ({ ticket }: { ticket?: TicketData }) => {
    const { dir } = useLanguage()
    const { data, isError, isLoading } = useGetTicketComments(ticket?.caseId)
    const { mutateAsync: addCommentFn, isPending: isAddCommentPending } = useCommentOnTicket(ticket?.caseId)
    const [message, setMessage] = useState('')

    const handleAddComment = () => {
        addCommentFn(message).then(() => {
            setMessage('')
        })
    }
    const reversedArray = data?.slice().reverse()
    const commentsAllowed = String(ticket?.stateId) === String(TICKET_STATUS.resolved) || String(ticket?.stateId) === String(TICKET_STATUS.onHold)
    return (
        <>
            <div
                className={classNames('px-space-04 py-space-05', {
                    'mb-[88px]': commentsAllowed,
                    'rounded-3 bg-background': !isError,
                })}>
                <ScrollArea dir={dir} className="max-h-[440px] overflow-auto">
                    <Switch>
                        <Case condition={isLoading}>
                            <Skeleton className="h-[220px] w-full" />
                        </Case>
                        <Case condition={isError || !data?.length}>
                            <EmptyState
                                title={strings.support.waitSupport}
                                description={strings.support.responseWillShow}
                                icon={{ light: MessageGray, dark: MessageGray }}
                                showBackground
                            />
                        </Case>
                        <Case condition={!!data?.length}>
                            <div className="flex items-center justify-center">
                                <div className="grow space-y-space-04">
                                    {reversedArray?.map(comment => (
                                        <ChatMessage
                                            key={comment.date}
                                            isUser={comment.user == 'External Integration'}
                                            message={comment.comment}
                                            timestamp={moment(comment.date).format('hh:mm A')}
                                            date={dateFormatter({ date: comment.date, format: 'long' })}
                                        />
                                    ))}
                                </div>
                            </div>
                        </Case>
                    </Switch>
                    <ScrollBar orientation="vertical" />
                </ScrollArea>
            </div>
            <When condition={commentsAllowed}>
                <div className="absolute bottom-0 left-0 right-0 bg-background p-space-05 pt-space-04">
                    <div className="flex gap-space-02">
                        <Input
                            className="grow self-center pe-space-02"
                            rounded="full"
                            type="text"
                            placeholder={`${strings.support.addCommentPlacholder}...`}
                            variant="default"
                            value={message}
                            onChange={ev => setMessage(ev.target.value)}
                        />
                        <Button colors="primary" size="icon" onClick={handleAddComment} disabled={isAddCommentPending}>
                            <Send />
                        </Button>
                    </div>
                </div>
            </When>
        </>
    )
}

export default ConversationList
