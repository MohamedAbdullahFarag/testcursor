import { Facebook, Instgram, Linkedin, X, Youtube } from '@/layout/PortalLayout/SocialIcons'
import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { Call, Email, HearingDisabled, HelpOutline, OpenInNew } from 'google-material-icons/outlined'
import { Button, Link, Separator, Stack } from 'mada-design-system'
import { PropsWithChildren, ReactElement } from 'react'
import { Link as LinkRouter } from 'react-router-dom'
import { supportConstants } from '../models/constants'

const Sidebar = () => {
    return (
        <Stack
            direction={'col'}
            className="relative -top-[216px] hidden w-[416px] rounded-3 border border-border-neutral-primary bg-background-white p-space-07 lg:flex">
            <h2 className="text-subtitle-01 font-semibold">{strings.footer.contactUs}</h2>
            <SidebarElement icon={<Call className="text-icon-primary" />} title={strings.shared.phone}>
                <a href={`tel:${supportConstants.Phone}`}>
                    {supportConstants.Phone} <OpenInNew />
                </a>
            </SidebarElement>
            <SidebarElement icon={<Email className="text-icon-primary" />} title={strings.shared.email}>
                <a href={`mailto:${supportConstants.Email}`}>
                    {supportConstants.Email} <OpenInNew />
                </a>
            </SidebarElement>
            <h2 className="text-subtitle-01 font-semibold">{strings.footer.followus}</h2>
            <Stack gap={2}>
                {contactUS.map((c, idx) => (
                    <Button key={idx} asChild rounded={'default'} size={'icon-sm'} variant={'text'} colors={'gray'}>
                        <a href={c.target}>{c.icon}</a>
                    </Button>
                ))}
            </Stack>
            <Separator />
            <SidebarElement icon={<HelpOutline className="text-icon-primary" />} title={strings.faq.title}>
                <LinkRouter to={pathNames.faqs}>
                    {strings.faq.browseFAQ} <OpenInNew />
                </LinkRouter>
            </SidebarElement>
            <SidebarElement icon={<HearingDisabled className="text-icon-primary" />} title={strings.accessibility.signLanguageTitle}>
                <LinkRouter to={'https://deaf.dga.gov.sa/'}>
                    {strings.shared.externalLink} <OpenInNew />
                </LinkRouter>
            </SidebarElement>
            <Button variant={'ghost'} colors="neutral" rounded={'default'} className="self-start">
                {strings.shared.userGuide}
            </Button>
        </Stack>
    )
}
const contactUS = [
    {
        target: 'https://www.linkedin.com/company/ministry-of-education-saudi-arabia',
        icon: <Linkedin />,
    },
    {
        target: 'https://x.com/moe_gov_sa',
        icon: <X />,
    },
    {
        target: 'https://www.youtube.com/@moe_gov',
        icon: <Youtube />,
    },
    {
        target: 'https://www.facebook.com/moegov.sa',
        icon: <Facebook />,
    },
    {
        target: 'https://www.instagram.com/moe_gov_sa',
        icon: <Instgram />,
    },
]

interface SidebarElement {
    icon: ReactElement
    title: string
}
const SidebarElement = ({ icon, title, children }: PropsWithChildren<SidebarElement>) => (
    <Stack gap={2}>
        {icon}
        <Stack direction={'col'} gap={1}>
            <span className="text-body-02 font-bold">{title}</span>
            <Link asChild>{children}</Link>
        </Stack>
    </Stack>
)

export default Sidebar
