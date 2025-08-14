import { PageLoader } from '@/shared/components'
import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { FilterAlt } from 'google-material-icons/filled'
import { DateRange, DonutLarge, FilePresent, QuestionAnswer, StickyNote2, Toc } from 'google-material-icons/outlined'
import { Badge, Breadcrumbs, dateFormatter, formatterTime, Stack, Tabs, TabsContent, TabsList, TabsTrigger, useLanguage } from 'mada-design-system'
import { ReactNode } from 'react'
import { createPortal } from 'react-dom'
import { Link, Navigate, useLocation } from 'react-router-dom'
import AttachmentFile from '../components/AttachmentFile'
import Sidebar from '../components/Sidebar'
import StatusBadge from '../components/StatusBadge'
import { useGetAttachmentsByCaseId, useGetCaseByCaseId } from '../services/useServiceNow'

const TicketDetails = () => {
    const { dir } = useLanguage()
    const { state } = useLocation() as { state: { caseId: string } }
    const { data, isLoading, isFetching, isError } = useGetCaseByCaseId(state?.caseId)
    const {
        data: attachmentsData,
        isError: attachmentsError,
        isLoading: attachmentsLoading,
        isFetching: attachmentsFetching,
    } = useGetAttachmentsByCaseId({ caseId: data?.data?.caseId, isEnabled: !!data?.data?.caseId })

    if ((isLoading || isFetching || attachmentsLoading || attachmentsFetching) && !!state?.caseId)
        return createPortal(
            <div className="fixed inset-0 z-[10000] flex items-center justify-center bg-white">
                <PageLoader />
            </div>,
            document.body,
        )
    if (isError || !state?.caseId) return <Navigate to="/" />
    return (
        <Stack gap={7} className="relative" direction={'col'}>
            <PortalHeaderPage
                titlePage={strings.support.ticketDetails}
                descriptionPage={strings.support.ticketDetailsDesc}
                breadcrumbs={
                    <Breadcrumbs
                        items={[
                            {
                                title: strings.shared.Main,
                                render: <Link to="/">{strings.shared.Main}</Link>,
                            },
                            {
                                title: strings.support.fullTitle,
                                render: <Link to={pathNames.portalSupport}>{strings.support.fullTitle}</Link>,
                            },
                            {
                                title: strings.support.ticketDetails,
                            },
                        ]}
                    />
                }
            />
            <Stack className="px-space-04 xl:px-space-06">
                <Stack
                    gap={6}
                    justifyContent={'between'}
                    alignItems={'start'}
                    className="mx-auto w-full max-w-container"
                    data-testid="ticketCardContainer">
                    <div className="grow pb-space-07" data-testid="ticketCardTest">
                        <Stack direction={'col'}>
                            <h1 className={'text-subtitle-01 font-bold'}>
                                {strings.formatString(strings.support.numberOfTicket, data?.data?.caseNum ?? '')}
                            </h1>
                            <section className="grid grid-cols-[13rem_1fr] gap-x-space-02 gap-y-space-04 ltr:grid-cols-[17rem_1fr]">
                                <DataTitle icon={<DonutLarge />} title={strings.support.ticketStatus} />
                                <StatusBadge statusCode={data?.data?.stateId ?? 1} />
                                <DataTitle icon={<DateRange />} title={strings.support.ticketDate} />
                                <p className="text-body-02 text-foreground">{dateFormatter({ date: data?.data?.createdOn ?? '', format: 'long' })}</p>
                                <DataTitle icon={<StickyNote2 />} title={strings.support.issueTitle} />
                                <p className="break-words text-body-02 text-foreground">{data?.data?.shortDescription}</p>
                                <DataTitle icon={<FilterAlt />} title={strings.support.serviceCategory} />
                                <div className="flex flex-wrap items-center gap-space-02">
                                    <Badge variant="ghost" colors="info" size="sm">
                                        {data?.data?.category}
                                    </Badge>
                                    <Badge variant="ghost" colors="tertiary" size="sm">
                                        {data?.data?.subcategory}
                                    </Badge>
                                </div>
                                <DataTitle icon={<Toc />} title={strings.shared.description} />
                                <p className="break-words text-body-02 text-foreground">{data?.data?.description}</p>
                            </section>
                            <Tabs defaultValue="reply" dir={dir}>
                                <TabsList variant={'underline'} underline>
                                    <TabsTrigger value="reply">
                                        <QuestionAnswer size={16} />
                                        {strings.support.reply}
                                    </TabsTrigger>
                                    <TabsTrigger value="attachments">
                                        <FilePresent size={16} /> {strings.attachments.attachments}
                                    </TabsTrigger>
                                </TabsList>
                                <TabsContent value={'reply'}>
                                    {!data?.data?.lastComment && (
                                        <section className="flex flex-col gap-space-05">
                                            <h2 className="text-subtitle-02 font-bold">{strings.support.customerSupportResponse}</h2>
                                            <div className="flex flex-col items-center gap-space-04 rounded-3 bg-background p-space-05">
                                                <div className="flex h-space-10 w-space-10 items-center justify-center rounded-full bg-secondary-container">
                                                    <QuestionAnswer className="h-space-07 w-space-07 text-secondary-dark" />
                                                </div>
                                                <div className="flex flex-col items-center gap-space-01 text-center">
                                                    <h2 className="text-subtitle-02 font-bold text-foreground">{strings.support.waitSupport}</h2>
                                                    <p className="text-body-02 font-normal text-foreground">{strings.support.responseWillShow}</p>
                                                </div>
                                            </div>
                                        </section>
                                    )}
                                    {data?.data?.lastComment && (
                                        <section className="flex flex-col gap-space-05">
                                            <h2 className="text-subtitle-02 font-bold">{strings.support.customerSupportResponse}</h2>
                                            <div className="flex flex-col gap-space-04 rounded-3 bg-background p-space-05">
                                                <div className="flex flex-col items-center gap-space-04 text-center">
                                                    <p className="text-body-01 text-foreground">
                                                        {dateFormatter({ date: data?.data?.lastCommentUpdated, format: 'long' })}
                                                    </p>
                                                    <div className="self-stretch pl-space-05 sm:pl-space-10">
                                                        <div className="flex-col gap-space-02 rounded-4 bg-secondary-container p-space-03">
                                                            <p className="text-right text-body-02 text-secondary-oncontainer">
                                                                {data?.data?.lastComment}
                                                            </p>
                                                            <p className="text-left text-caption-01">
                                                                {formatterTime(document.dir).format(new Date(data?.data?.lastCommentUpdated))}
                                                            </p>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </section>
                                    )}
                                </TabsContent>
                                <TabsContent value="attachments">
                                    <div className="col-span-2 flex flex-wrap gap-space-03 sm:col-span-1">
                                        {attachmentsData?.data &&
                                            !isError &&
                                            attachmentsData?.data?.length > 0 &&
                                            attachmentsData?.data?.map(file => <AttachmentFile key={file.fileName} file={file} />)}
                                        {(!attachmentsData?.data || attachmentsError) && (
                                            <p className="break-words text-body-02 text-foreground">-</p>
                                        )}
                                    </div>
                                </TabsContent>
                            </Tabs>
                        </Stack>
                    </div>

                    <Sidebar />
                </Stack>
            </Stack>
        </Stack>
    )
}

export default TicketDetails

const DataTitle = ({ title, icon }: { title: string; icon: ReactNode }) => {
    return (
        <div className="flex items-center gap-space-01 text-body-02 text-foreground-secondary">
            {icon}
            <h3>{title}:</h3>
        </div>
    )
}
