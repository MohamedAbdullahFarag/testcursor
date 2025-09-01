import { useAuthStore } from '@/modules/auth/store/authStore'
import { EmptyState, MainPanel, PageHeader, ServiceError } from '@/shared/components'

import { strings } from '@/shared/locales'
import { StatusCodes } from '@/shared/models/enums'
import { AxiosError } from 'axios'
import { Support as SupportIcon } from 'google-material-icons/filled'
import { CopyAll, Email, Forum, Phone } from 'google-material-icons/outlined'
import {
    Breadcrumbs,
    Button,
    Grid,
    GridItem,
    Pagination,
    Sheet,
    SheetBody,
    SheetContent,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
    Stack,
    useToast,
} from 'mada-design-system'
import { useState, useMemo } from 'react'
import { Case, Default, Switch } from 'react-if'
import { CertificatesGray } from 'streamline-icons'
import { Create, MainSkeleton, TicketCard, TicketDetails } from '../components'
import GridLayout from '../components/GridLayout'
import useGridColumns from '../hooks/useGridColumns'
import { supportConstants } from '../models/constants'
import { useGetCasesByUserId } from '../services/useServiceNow'

const DashboardSupport = () => {
    const { toast } = useToast()
    const shareLink = (text: string) => {
        navigator.clipboard.writeText(text).then(() => {
            toast({
                description: strings.shared.copiedSuccessfully,
            })
        })
    }
    return (
        <MainPanel>
            <main className="flex flex-col gap-space-05">
                <Breadcrumbs
                    items={[
                        {
                            title: strings.shared.Main,
                            path: `/dashboard`,
                        },
                        { title: strings.support.title },
                    ]}
                />
                <PageHeader
                    title={strings.support.title}
                    description={strings.support.customerSupportDescription}
                    icon={SupportIcon}
                    dataTestId="support-page-title"
                />
                <Grid gap={4} cols={6}>
                    <GridItem columns={{ base: 6, lg: 3, xl: 2 }} className="rounded-2 border border-border p-space-03">
                        <Stack gap={3} className="w-full" alignItems={'center'}>
                            <Stack gap={'none'} className="rounded-1 bg-background p-space-02">
                                <Email size={20} className="text-foreground-tertiary" />
                            </Stack>
                            <Stack direction={'col'} gap={1}>
                                <span className="text-body-01 font-bold">{supportConstants.Email}</span>
                                <span className="text-caption-01 text-foreground-secondary"> {strings.shared.email}</span>
                            </Stack>
                            <Button
                                type="button"
                                tooltip={strings.shared.copy}
                                onClick={() => shareLink(supportConstants.Email)}
                                variant={'ghost'}
                                size={'icon-sm'}
                                colors={'primary'}
                                className="ms-auto w-space-06">
                                <CopyAll size={18} />
                            </Button>
                        </Stack>
                    </GridItem>
                    <GridItem columns={{ base: 6, lg: 3, xl: 2 }} className="rounded-2 border border-border p-space-03">
                        <Stack gap={3} className="w-full" alignItems={'center'}>
                            <Stack gap={'none'} className="rounded-1 bg-background p-space-02">
                                <Phone size={20} className="text-foreground-tertiary" />
                            </Stack>
                            <Stack direction={'col'} gap={1}>
                                <span className="text-body-01 font-bold">
                                    <bdi>{supportConstants.Phone}</bdi>{' '}
                                </span>
                                <span className="text-caption-01 text-foreground-secondary"> {strings.support.contactNo}</span>
                            </Stack>
                            <Button
                                type="button"
                                tooltip={strings.shared.copy}
                                onClick={() => shareLink(supportConstants.Phone)}
                                variant={'ghost'}
                                size={'icon-sm'}
                                colors={'primary'}
                                className="ms-auto w-space-06">
                                <CopyAll size={18} />
                            </Button>
                        </Stack>
                    </GridItem>
                    <GridItem columns={{ base: 6, lg: 3, xl: 2 }} className="rounded-2 border border-border p-space-03">
                        <Stack gap={3} className="w-full" alignItems={'center'}>
                            <Stack gap={'none'} className="rounded-1 bg-background p-space-02">
                                <Forum size={20} className="text-foreground-tertiary" />
                            </Stack>
                            <Stack direction={'col'} gap={1}>
                                <span className="text-body-01 font-bold">{strings.support.openSupportTicket}</span>
                                <span className="text-caption-01 text-foreground-secondary"> {strings.support.supportTickets}</span>
                            </Stack>
                            <Stack gap={'none'} className="ms-auto">
                                <Create key={'createTicket'} />
                            </Stack>
                        </Stack>
                    </GridItem>
                </Grid>
                <Comp />
            </main>
        </MainPanel>
    )
}

const Comp = () => {
    const cardsTotal = useGridColumns()
    const { user } = useAuthStore(state => state)
    const [currentPage, setCurrentPage] = useState(1)
    
    // Debug logging to track re-renders and values
    console.log('Comp re-render:', { 
        cardsTotal, 
        userId: user?.id, 
        currentPage,
        userFullObject: user 
    })
    
    // Memoize the userId to prevent unnecessary re-renders if user object changes but id doesn't
    const userId = useMemo(() => user?.id || '', [user?.id])
    
    const { data, isLoading, error, isError, refetch } = useGetCasesByUserId({
        userId,
        Limit: cardsTotal,
        Offset: (currentPage - 1) * cardsTotal,
    })

    const onPageChange = (value: number) => {
        setCurrentPage(value)
    }

    return (
        <Switch>
            <Case condition={isLoading}>
                <MainSkeleton />
            </Case>
            <Case condition={(error as AxiosError)?.status === StatusCodes.NotFound}>
                <div className="rounded-3 border">
                    <EmptyState
                        title={strings.support.noData}
                        description={strings.support.noDataDesc}
                        icon={{ dark: CertificatesGray, light: CertificatesGray }}
                        buttons={[<Create key={'createTicket'} />]}
                    />
                </div>
            </Case>
            <Case condition={isError}>
                <div className="rounded-3 border">
                    <ServiceError
                        CustomMessage={strings.shared.serverError}
                        CustomMessageDesc=""
                        isSupprtBtnVisible={false}
                        onUpdate={refetch}
                    />
                </div>
            </Case>
            <Default>
                <GridLayout>
                    {data?.data?.map(ticket => (
                        <Sheet key={ticket?.caseNum}>
                            <SheetTrigger>
                                <TicketCard Ticket={ticket} />
                            </SheetTrigger>
                            <SheetContent>
                                <SheetHeader>
                                    <SheetTitle>{strings.support.ticketDetails}</SheetTitle>
                                </SheetHeader>
                                <SheetBody>
                                    <TicketDetails Ticket={ticket} />
                                </SheetBody>
                            </SheetContent>
                        </Sheet>
                    ))}
                </GridLayout>
                <Pagination
                    className="justify-center"
                    totalItems={Number(data?.metadata?.resultset?.count ?? 0)}
                    selectedPage={currentPage}
                    onPageChange={onPageChange}
                    itemsPerPage={cardsTotal}
                />
            </Default>
        </Switch>
    )
}

export default DashboardSupport
