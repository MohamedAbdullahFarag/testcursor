import Logo from '@/assets/images/Logo ME.svg'
import { pathNames } from '@/shared/constants/pathNames'
import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { Dashboard, ExpandLess } from 'google-material-icons/filled'
import { BorderColor, Close, LibraryBooks, Settings } from 'google-material-icons/outlined'
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
        href: pathNames.dasbhaord,
        icon: <Dashboard className="size-space-05" />,
    },
    {
        label: strings.sideBar.contentManagement,
        href: pathNames.contentManagment,
        icon: <BorderColor className="size-space-05" />,
        subLinks: [
            {
                label: 'ادارة مزايا',
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.contentManagment,
            },
        ],
    },
    {
        label: strings.sideBar.platformManagement,
        href: pathNames.userManagement,
        icon: <Settings className="size-space-05" />,
        subLinks: [
            {
                label: strings.sideBar.userManagement,
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.userManagement,
            },
            {
                label: strings.sideBar.roleManagement,
                icon: <LibraryBooks className="size-[20px]" />,
                href: pathNames.roleManagement,
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
                        <h1 className="text-caption-02 font-bold">{strings.shared.unifiedPortal}</h1>
                        <p className="whitespace-nowrap text-caption-01">{strings.shared.headerDesc}</p>
                    </Stack>
                    <p
                        className="flex items-center justify-center whitespace-nowrap rounded-full bg-primary-container px-[6px] py-[2px] text-[10px]"
                        data-testid="trialVersionTest">
                        {strings.shared.trialVersion}
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
