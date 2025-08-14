import { strings } from '@/shared/locales'
import { ScrollArea, ScrollBar, ToggleGroup, ToggleGroupItem, useLanguage } from 'mada-design-system'

import { useState } from 'react'
import { TicketFilterValues } from '../models/enums'

const Filters = ({ onChange }: { onChange: (value: TicketFilterValues) => void }) => {
    const { dir } = useLanguage()
    const [activeTab, setActiveTab] = useState(TicketFilterValues.All)
    return (
        <ScrollArea dir={dir} className="w-full self-start">
            <ToggleGroup
                defaultValue={TicketFilterValues.All}
                type="single"
                dir={dir}
                className="items-start justify-start gap-space-02 whitespace-nowrap text-nowrap"
                onValueChange={(val: TicketFilterValues) => {
                    if (!val) return
                    onChange(val)
                    setActiveTab(val)
                }}
                value={activeTab}
                size="sm">
                <ToggleGroupItem value={TicketFilterValues.All} aria-label="Toggle bold" variant="outline">
                    {strings.caseStatus.all}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.New} variant="outline">
                    {strings.caseStatus.new}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.InProgress} variant="outline">
                    {strings.caseStatus.inProgress}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.OnHold} variant="outline">
                    {strings.caseStatus.onHold}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.Closed} variant="outline">
                    {strings.caseStatus.closed}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.Resolved} variant="outline">
                    {strings.caseStatus.resolved}
                </ToggleGroupItem>
                <ToggleGroupItem value={TicketFilterValues.Cancelled} variant="outline">
                    {strings.caseStatus.cancelled}
                </ToggleGroupItem>
            </ToggleGroup>
            <ScrollBar orientation="horizontal" />
        </ScrollArea>
    )
}

export default Filters
