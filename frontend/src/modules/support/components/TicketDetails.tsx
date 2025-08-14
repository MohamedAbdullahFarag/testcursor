import { strings } from '@/shared/locales'
import { FilterAlt } from 'google-material-icons/filled'
import { DateRange, DonutLarge as Dount, FilePresent, QuestionAnswer, StickyNote2 as StickyNote, Toc } from 'google-material-icons/outlined'
import { Badge, dateFormatter, DescriptionItem, Stack, Tabs, TabsContent, TabsList, TabsTrigger, useLanguage } from 'mada-design-system'
import { ReactNode } from 'react'
import { TicketData } from '../models/interfaces'

import ConversationList from './ConversationList'
import StatusBadge from './StatusBadge'
import ViewTicketAddAttatchments from './ViewTicketAddAttatchments'

const TicketDetails = ({ Ticket, children }: { Ticket?: TicketData; children?: ReactNode }) => {
    const { dir } = useLanguage()

    return (
        <div className="flex flex-col gap-space-05">
            {children}
            <Stack direction={'col'} gap={4}>
                <h1 className={'text-subtitle-01 font-bold'}>{strings.formatString(strings.support.numberOfTicket, Ticket?.caseNum ?? '')}</h1>
                <DescriptionItem icon={Dount} title={strings.support.ticketStatus} lastItem padding={'none'}>
                    <StatusBadge statusCode={Ticket?.stateId ?? 1} size="sm" />
                </DescriptionItem>

                <DescriptionItem icon={DateRange} title={strings.support.ticketDate} lastItem padding={'none'}>
                    <p>{dateFormatter({ date: Ticket?.createdOn ?? '', format: 'long' })}</p>
                </DescriptionItem>

                <DescriptionItem icon={StickyNote} title={strings.support.issueTitle} lastItem padding={'none'}>
                    <p className="break-words">{Ticket?.shortDescription}</p>
                </DescriptionItem>
                <DescriptionItem icon={FilterAlt} title={strings.support.followupCategory} lastItem padding={'none'}>
                    <div className="flex flex-wrap items-center gap-space-02">
                        <Badge variant="ghost" colors="info" size="sm">
                            {Ticket?.category}
                        </Badge>
                        <Badge variant="ghost" colors="tertiary" size="sm">
                            {Ticket?.subcategory}
                        </Badge>
                    </div>
                </DescriptionItem>
                <DescriptionItem icon={Toc} title={strings.shared.description} lastItem padding={'none'} />
                <p className="break-words">{Ticket?.description}</p>
            </Stack>
            {!children && (
                <Tabs dir={dir} className="flex flex-col gap-space-05 overflow-hidden" defaultValue="chats">
                    <TabsList variant="underline" underline>
                        <TabsTrigger value="chats">
                            <QuestionAnswer />
                            {strings.support.chats}
                        </TabsTrigger>
                        <TabsTrigger value="attatchements">
                            <FilePresent />
                            {strings.attachments.attachments}
                        </TabsTrigger>
                    </TabsList>
                    <TabsContent value="chats">
                        <ConversationList ticket={Ticket} />
                    </TabsContent>
                    <TabsContent value="attatchements">
                        <ViewTicketAddAttatchments ticket={Ticket} />
                    </TabsContent>
                </Tabs>
            )}
        </div>
    )
}

export default TicketDetails
