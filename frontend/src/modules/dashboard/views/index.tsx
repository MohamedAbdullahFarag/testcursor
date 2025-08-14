import MainPanel from '@/shared/components/MainPanel'
import { strings } from '@/shared/locales'
import { Dashboard as DashobardIcon } from 'google-material-icons/filled'
import { Create, OpenInNew, Work } from 'google-material-icons/outlined'
import { Badge, Button, Card, Stack } from 'mada-design-system'
import { When } from 'react-if'
const Dashboard = () => {
    return (
        <MainPanel>
            <h1 className="title-01 font-bold text-card-foreground">{strings.dashboard.title}</h1>
            <Stack direction="col" gap={4}>
                <h2 className="text-subtitle-01 font-bold text-card-foreground">{strings.dashboard.recentlyUsedServices}</h2>
                <div className="grid grid-cols-1 gap-space-04 sm:grid-cols-2 xl:grid-cols-4">
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                </div>
            </Stack>
            <Stack direction="col" gap={4}>
                <Stack justifyContent="between">
                    <h2 className="text-subtitle-01 font-bold text-card-foreground">{strings.dashboard.quickAccess}</h2>
                    <Button variant="outline" size="sm" colors="primary" className="gap-space-01">
                        <Create className="size-[20px]" />
                        {strings.shared.edit}
                    </Button>
                </Stack>
                <div className="grid grid-cols-1 gap-space-04 sm:grid-cols-2 xl:grid-cols-4">
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                    <WidgetCard isExternal />
                    <WidgetCard />
                </div>
            </Stack>
        </MainPanel>
    )
}

export default Dashboard

const WidgetCard = ({ isExternal, title }: { isExternal?: boolean; title?: string }) => {
    return (
        <Card>
            <Stack justifyContent="between" alignItems="center">
                <DashobardIcon className="size-space-06 text-card-foreground" />
                <When condition={isExternal}>
                    <OpenInNew className="size-space-06 text-card-foreground" />
                </When>
            </Stack>
            <Stack direction="col" gap={1}>
                <h2 className="text-body-01 font-semibold">{title ?? strings.dashboard.testTitle}</h2>
                <p className="text-body-01 text-foreground-secondary">{strings.dashboard.testDesc}</p>
            </Stack>
            <Stack gap={2} alignItems="center">
                <Badge className="gap-space-01" variant="ghost" colors="gray" size="sm">
                    <Work className="size-space-04" />
                    {strings.sideBar.contentManagement}
                </Badge>
                <Badge className="gap-space-01" variant="ghost" colors="gray" size="sm">
                    <Work className="size-space-04" />
                    {strings.sideBar.requestsManagment}
                </Badge>
            </Stack>
        </Card>
    )
}
