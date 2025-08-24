import Logo from '@/assets/images/Logo ME.svg'
import { pathNames } from '@/shared/constants/pathNames'
import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { Dashboard, ExpandLess } from 'google-material-icons/filled'
import { 
    BorderColor, 
    Close, 
    LibraryBooks, 
    Settings, 
    People, 
    Shield, 
    History, 
    Support, 
    Image, 
    QuestionAnswer,
    Assessment,
    Notifications
} from 'google-material-icons/outlined'
import {
    accessibilityTools,
    Button,
    Collapsible,
    CollapsibleContent,
    CollapsibleTrigger,
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarMenu,
    SidebarMenuItem,
    Stack,
    useSidebar,
} from 'mada-design-system'
import { useState } from 'react'
import { Else, If, Then } from 'react-if'
import { NavLink } from 'react-router-dom'

const navigations = [
    {
        label: strings.sideBar.dashboard,
        href: pathNames.dashboard,
        icon: <Dashboard className="size-space-05" />,
    },
    {
        label: 'Content Management',
        href: pathNames.contentManagment,
        icon: <BorderColor className="size-space-05" />,
        subLinks: [
            {
                label: 'Content Features',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.contentManagment,
            },
        ],
    },
    {
        label: 'Platform Management',
        href: pathNames.userManagement,
        icon: <Settings className="size-space-05" />,
        subLinks: [
            {
                label: strings.sideBar.userManagement,
                icon: <People className="size-[20px]" />,
                href: pathNames.userManagement,
            },
            {
                label: strings.sideBar.roleManagement,
                icon: <Shield className="size-[20px]" />,
                href: pathNames.roleManagement,
            },
        ],
    },
    {
        label: 'Audit & Compliance',
        href: pathNames.auditLogs,
        icon: <History className="size-space-05" />,
        subLinks: [
            {
                label: 'Audit Logs',
                icon: <History className="size-[20px]" />,
                href: pathNames.auditLogs,
            },
        ],
    },
    {
        label: 'Support System',
        href: pathNames.support,
        icon: <Support className="size-space-05" />,
        subLinks: [
            {
                label: 'Support Dashboard',
                icon: <Support className="size-[20px]" />,
                href: pathNames.support,
            },
        ],
    },
    {
        label: 'Media Management',
        href: pathNames.mediaManagement,
        icon: <Image className="size-space-05" />,
        subLinks: [
            {
                label: 'Media Library',
                icon: <Image className="size-[20px]" />,
                href: pathNames.mediaManagement,
            },
            {
                label: 'Collections',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.mediaCollections,
            },
        ],
    },
    {
        label: 'Question Bank',
        href: pathNames.questionBank,
        icon: <QuestionAnswer className="size-space-05" />,
        subLinks: [
            {
                label: 'Question Tree',
                icon: <Assessment className="size-[20px]" />,
                href: pathNames.questionBankTree,
            },
            {
                label: 'Categories',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.questionBankCategories,
            },
        ],
    },
    {
        label: 'Notifications',
        href: pathNames.notifications,
        icon: <Notifications className="size-space-05" />,
        subLinks: [
            {
                label: 'Notification Center',
                icon: <Notifications className="size-[20px]" />,
                href: pathNames.notifications,
            },
            {
                label: 'Preferences',
                icon: <Settings className="size-[20px]" />,
                href: pathNames.notificationPreferences,
            },
        ],
    },
    {
        label: 'System',
        href: pathNames.system,
        icon: <Settings className="size-space-05" />,
        subLinks: [
            {
                label: 'System Settings',
                icon: <Settings className="size-[20px]" />,
                href: pathNames.systemSettings,
            },
            {
                label: 'API Documentation',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.systemApiDocs,
            },
            {
                label: 'System Health',
                icon: <Assessment className="size-[20px]" />,
                href: pathNames.systemHealth,
            },
        ],
    },
    {
        label: 'Analytics',
        href: pathNames.analytics,
        icon: <Assessment className="size-space-05" />,
        subLinks: [
            {
                label: 'Dashboard Analytics',
                icon: <Assessment className="size-[20px]" />,
                href: pathNames.analyticsDashboard,
            },
            {
                label: 'User Analytics',
                icon: <People className="size-[20px]" />,
                href: pathNames.analyticsUsers,
            },
            {
                label: 'Content Analytics',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.analyticsContent,
            },
        ],
    },
    {
        label: 'Customer Experience',
        href: pathNames.customerExperience,
        icon: <People className="size-space-05" />,
        subLinks: [
            {
                label: 'Surveys',
                icon: <Assessment className="size-[20px]" />,
                href: pathNames.customerExperienceSurveys,
            },
            {
                label: 'Feedback',
                icon: <Support className="size-[20px]" />,
                href: pathNames.customerExperienceFeedback,
            },
        ],
    },
    {
        label: 'E-Participation',
        href: pathNames.eParticipation,
        icon: <People className="size-space-05" />,
        subLinks: [
            {
                label: 'Participation Portal',
                icon: <People className="size-[20px]" />,
                href: pathNames.eParticipationPortal,
            },
            {
                label: 'Initiatives',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.eParticipationInitiatives,
            },
        ],
    },
    {
        label: 'Help & Legal',
        href: pathNames.help,
        icon: <LibraryBooks className="size-space-05" />,
        subLinks: [
            {
                label: 'FAQ',
                icon: <QuestionAnswer className="size-[20px]" />,
                href: pathNames.helpFaq,
            },
            {
                label: 'Terms & Conditions',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.helpTerms,
            },
            {
                label: 'Privacy Policy',
                icon: <Shield className="size-[20px]" />,
                href: pathNames.helpPrivacy,
            },
        ],
    },
]

const DashboardSideBar = () => {
    const isActive = accessibilityTools(state => state.isActive)
    const { isMobile } = useSidebar()
    const [openSection, setOpenSection] = useState<string>('')

    const handleSectionToggle = (sectionId: string) => {
        setOpenSection(sectionId === openSection ? '' : sectionId)
    }

    return (
        <Sidebar
            side="right"
            className={classNames({
                grayscale: isActive,
            })}>
            <If condition={isMobile}>
                <Then>
                    <SideBarMobile handleSectionToggle={handleSectionToggle} openSection={openSection} />
                </Then>
                <Else>
                    <SidebarContent>
                        <SidebarGroup>
                            <SidebarGroupContent>
                                <SidebarMenu>
                                    {navigations.map(item => (
                                        <SidebarMenuItem key={item.label}>
                                            <NavLink end to={item.href} className="group/link">
                                                {({ isActive }) => (
                                                    <div
                                                        className={classNames(
                                                            'flex items-center gap-space-01 py-space-02 text-center xl:flex-col xl:py-space-00',
                                                            {
                                                                'rounded-full bg-card font-bold text-primary xl:rounded-none xl:bg-transparent':
                                                                    isActive,
                                                            },
                                                        )}>
                                                        <span
                                                            className={classNames(
                                                                'inline-block rounded-full p-space-02',
                                                                'group-hover/link:bg-card-hover',
                                                                {
                                                                    'bg-card': isActive,
                                                                },
                                                            )}>
                                                            {item?.icon}
                                                        </span>
                                                        {item?.label}
                                                    </div>
                                                )}
                                            </NavLink>
                                        </SidebarMenuItem>
                                    ))}
                                </SidebarMenu>
                            </SidebarGroupContent>
                        </SidebarGroup>
                    </SidebarContent>
                    <SidebarFooter>
                        <NavLink
                            to={'/'}
                            className="pointer-events-none flex cursor-pointer items-center gap-space-01 text-caption-02 text-foreground-secondary xl:flex-col">
                            <div className="p-space-02">
                                <Settings className="size-space-05" />
                            </div>
                            <span>{strings.sideBar.settings}</span>
                        </NavLink>
                    </SidebarFooter>
                </Else>
            </If>
        </Sidebar>
    )
}

export default DashboardSideBar

interface SidebarMobileProps {
    handleSectionToggle: (sectionId: string) => void
    openSection: string
}

const SideBarMobile = ({ handleSectionToggle, openSection }: SidebarMobileProps) => {
    const { setOpenMobile } = useSidebar()

    return (
        <>
            <Stack justifyContent="between" className="border-b p-space-04 text-card-foreground">
                <Stack gap={1} alignItems="center">
                    <img src={Logo} alt="Logo" className="h-auto: w-[60px]" />
                    <Stack direction="col" gap={'none'} alignItems="start" className="text-card-foreground">
                        <h1 className="text-caption-02 font-bold">{strings.common?.unifiedPortal || 'Unified Portal'}</h1>
                        <p className="whitespace-nowrap text-caption-01">{strings.common?.headerDesc || 'Ministry of Education Services'}</p>
                    </Stack>
                    <p
                        className="flex items-center justify-center whitespace-nowrap rounded-full bg-primary-container px-[6px] py-[2px] text-[10px]"
                        data-testid="trialVersionTest">
                        {strings.common?.trialVersion || 'Beta version'}
                    </p>
                </Stack>
                <Button size="icon-sm" onClick={() => setOpenMobile(false)} variant="ghost" colors="gray" className="self-start">
                    <span className="sr-only">Close menu</span>
                    <Close className="size-space-05" />
                </Button>
            </Stack>
            <SidebarContent>
                {navigations?.map(item => (
                    <SidebarGroup key={item?.label}>
                        <SidebarGroupContent>
                            <SidebarMenu>
                                <If condition={!!item?.subLinks?.length}>
                                    <Then>
                                        <Collapsible
                                            open={openSection === item?.label}
                                            onOpenChange={() => handleSectionToggle(item?.label)}
                                            className="flex flex-col gap-space-02">
                                            <CollapsibleTrigger
                                                className={classNames({
                                                    'flex w-full items-center justify-between px-space-04 py-space-03 text-body-01 transition-all duration-300':
                                                        true,
                                                    'font-bold text-primary': openSection === item?.label,
                                                })}>
                                                <Stack gap={1} alignItems="center">
                                                    {item?.icon}
                                                    {item?.label}
                                                </Stack>
                                                <ExpandLess
                                                    className={classNames({
                                                        'transition-all duration-300': true,
                                                        'rotate-180': openSection !== item?.label,
                                                    })}
                                                />
                                            </CollapsibleTrigger>
                                            <CollapsibleContent>
                                                {item?.subLinks?.map(sub => (
                                                    <NavLink
                                                        end
                                                        to={sub?.href}
                                                        className="group/link"
                                                        key={sub?.label}
                                                        onClick={() => setOpenMobile(false)}>
                                                        {({ isActive }) => (
                                                            <div
                                                                className={classNames(
                                                                    'flex items-center gap-space-01 rounded-1 bg-background px-space-03 py-space-02 text-center text-body-01 transition-all duration-300',
                                                                    {
                                                                        'rounded-full bg-card font-bold text-primary': isActive,
                                                                    },
                                                                )}>
                                                                {sub?.icon}
                                                                {sub?.label}
                                                            </div>
                                                        )}
                                                    </NavLink>
                                                ))}
                                            </CollapsibleContent>
                                        </Collapsible>
                                    </Then>
                                    <Else>
                                        <NavLink
                                            end
                                            to={item?.href}
                                            className="group/link"
                                            key={item?.label}
                                            onClick={() => {
                                                handleSectionToggle('')
                                                setOpenMobile(false)
                                            }}>
                                            {({ isActive }) => (
                                                <div
                                                    className={classNames(
                                                        'flex items-center gap-space-01 px-space-03 py-space-02 text-center text-foreground-secondary',
                                                        {
                                                            'rounded-full bg-card font-bold text-primary': isActive && !openSection,
                                                        },
                                                    )}>
                                                    {item?.icon}
                                                    {item?.label}
                                                </div>
                                            )}
                                        </NavLink>
                                    </Else>
                                </If>
                            </SidebarMenu>
                        </SidebarGroupContent>
                    </SidebarGroup>
                ))}
            </SidebarContent>
            <SidebarFooter>
                <NavLink
                    to={'/'}
                    className="pointer-events-none flex w-full cursor-pointer items-center gap-space-01 text-body-01 text-foreground-secondary transition-all duration-300">
                    <div className="p-space-02">
                        <Settings className="size-space-05" />
                    </div>
                    <span>{strings.sideBar.settings}</span>
                </NavLink>
            </SidebarFooter>
        </>
    )
}
