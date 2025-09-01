import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { Book, ContactSupport, FileDownload, HearingDisabled, OpenInNew, Quiz, SupportAgent } from 'google-material-icons/outlined'
import { Breadcrumbs, Button, Card, CardContent, CardFooter, CardHeader, Grid, GridItem, Stack } from 'mada-design-system'
import { PropsWithChildren, ReactElement } from 'react'
import { Link } from 'react-router-dom'

export default function PortalSupport() {
    return (
        <section className="px-space-04 py-space-07 xl:px-space-06">
            <Stack gap={6} direction={'col'} className="mx-auto max-w-container">
                <Stack gap={4} direction={'col'}>
                    <Breadcrumbs
                        items={[
                            { title: strings.shared.Main, render: <Link to="/">{strings.shared.Main}</Link> },
                            { title: strings.support.fullTitle },
                        ]}
                    />
                    <h1 className="title-03 font-bold" data-testid="portal-support-title">{strings.support.fullTitle}</h1>
                    <p className="text-body-02">{strings.support.supportBreifDesc}</p>
                </Stack>
                <Grid cols={6} gapX={5} gapY={6}>
                    {CARDS.map(card => (
                        <CardGridItem {...card} key={card.title} />
                    ))}
                </Grid>
            </Stack>
        </section>
    )
}

const FeaturedIcon = ({ children }: PropsWithChildren) => (
    <Stack
        justifyContent={'center'}
        alignItems={'center'}
        className="size-space-08 rounded-full bg-background-primary-50 text-icon-primary [&_svg]:size-space-05 [&_svg]:shrink-0">
        {children}
    </Stack>
)

const CARDS: CardGridItemProps[] = [
    {
        icon: <Quiz />,
        title: strings.faq.title,
        desc: strings.faq.faqDescription,
        actionRender: <Link to={pathNames.faqs}>{strings.faq.browseFAQ}</Link>,
    },
    {
        icon: <Book />,
        title: strings.shared.userGuide,
        desc: strings.shared.userGuideFullDesc,
        actionRender: (
            <Link to={pathNames.faqs}>
                <FileDownload /> {strings.shared.downloadUserGuide}
            </Link>
        ),
    },
    {
        icon: <SupportAgent />,
        title: strings.support.inquiryTicket,
        desc: strings.support.inquiryTicketDesc,
        actionRender: <Link to={pathNames.portalSupportCreate}>{strings.support.createTicket}</Link>,
    },
    {
        icon: <ContactSupport />,
        title: strings.support.ticketInquiry,
        desc: strings.support.inquiryDesc,
        actionRender: <Link to={pathNames.portalSupportInquiry}>{strings.support.inquiry}</Link>,
    },
    {
        icon: <HearingDisabled />,
        title: strings.accessibility.signLanguageTitle,
        desc: strings.accessibility.signLanguageDesc,
        actionRender: (
            <Link to={'https://deaf.dga.gov.sa/'} target="_blank" rel="noreferrer">
                {strings.shared.externalLinkL}
                <OpenInNew />
            </Link>
        ),
    },
]

interface CardGridItemProps {
    icon: ReactElement
    title: string
    desc: string
    actionRender: ReactElement
}
const CardGridItem = ({ icon, title, desc, actionRender }: CardGridItemProps) => (
    <GridItem columns={{ base: 6, sm: 3, lg: 2 }}>
        <Card className="h-[300px] gap-space-05 rounded-3 border-border-neutral-primary active:!bg-card">
            <CardHeader className="p-space-00">
                <FeaturedIcon>{icon}</FeaturedIcon>
            </CardHeader>
            <CardContent className="flex-col gap-space-02 p-space-00">
                <h2 className="text-subtitle-01 font-bold">{title}</h2>
                <p className="text-body-02 text-text-display">{desc}</p>
            </CardContent>
            <CardFooter className="mt-auto">
                <Button variant={'outline'} rounded={'default'} asChild>
                    {actionRender}
                </Button>
            </CardFooter>
        </Card>
    </GridItem>
)
