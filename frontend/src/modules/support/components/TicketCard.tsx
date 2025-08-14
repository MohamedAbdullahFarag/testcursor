import { strings } from '@/shared/locales'

import { StreamLineIcon } from '@/shared/components'
import { DateRange, Tag } from 'google-material-icons/filled'
import { Badge, Card, Stack, dateFormatter } from 'mada-design-system'
import { When } from 'react-if'
import { HelpDark, HelpLight } from 'streamline-icons'
import { TicketData } from '../models/interfaces'
import StatusBadge from './StatusBadge'

const TicketCard = ({ Ticket }: { Ticket: TicketData }) => {
    return (
        <Card className="border-border">
            <Stack justifyContent="between" alignItems="start">
                <StreamLineIcon light={HelpLight} dark={HelpDark} />
                <StatusBadge statusCode={Ticket?.stateId} size="sm" />
            </Stack>
            <Stack gap={2} direction="col" alignItems="start">
                <h1 className="self-start text-body-01 font-semibold text-card-foreground">{Ticket?.subcategory}</h1>
                <p className="line-clamp-1 break-all text-caption-01 text-foreground-secondary">{Ticket?.shortDescription}</p>
                <When condition={!!Ticket?.category}>
                    <div className="flex items-center gap-space-02">
                        <Badge variant="ghost" colors="info" size="sm">
                            {Ticket?.category}
                        </Badge>
                    </div>
                </When>
            </Stack>
            <div className="flex items-center gap-space-01 text-caption-01 text-foreground-secondary">
                <span className="flex items-center gap-space-01">
                    <DateRange className="size-space-04" />
                    {dateFormatter({ date: Ticket?.createdOn, format: 'long' })}
                </span>
                <div className="size-[5px] rounded-full bg-foreground-secondary opacity-60" />
                <span className="flex items-center gap-space-01">
                    <Tag className="size-space-04" />
                    {strings.support.ticket} {Ticket?.caseNum}
                </span>
            </div>
        </Card>
    )
}

export default TicketCard
