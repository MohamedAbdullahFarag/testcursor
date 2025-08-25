import Logo from '@/assets/images/LogoME.svg'

import { strings } from '@/shared/locales'
import { pathNames } from '@/shared/constants'
import { Person } from 'google-material-icons/outlined'
import {
    Button,
    NavigationAction,
    NavigationActions,
    NavigationHeader,
    NavigationHeaderLogo,
    NavigationMain,
    NavigationMenu,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    NavigationMobileBody,
    NavigationMobileFooter,
    NavigationMobileHeader,
    NavigationMobileLink,
    NavigationMobileSideBar,
    NavigationSwitchLanguage,
    SecondNavHeader,
    SecondNavHeaderAction,
    SecondNavHeaderContent,
    Stack,
    useLanguage,
} from 'mada-design-system'
import { Link, NavLink } from 'react-router-dom'
import { useToggle } from 'usehooks-ts'

const PortalTopHeader = () => {
    const { dir } = useLanguage()
    
    // Enhanced menu items with more navigation options
    const menuItems = [
        { title: strings.common?.home || 'Home', id: 1, href: pathNames.portal },
        { title: strings.support?.title || 'Support', id: 2, href: pathNames.portalSupport },
        { title: strings.faq?.title || 'FAQ', id: 3, href: pathNames.faqs },
        { title: strings.eParticipation?.title || 'E-Participation', id: 4, href: pathNames.eparticipation }
    ]
    
    const [open, toggle] = useToggle(false)
    return (
        <>
            <SecondNavHeader colors="gray">
                <SecondNavHeaderContent />
                <SecondNavHeaderAction />
            </SecondNavHeader>
            <NavigationHeader divider className="sticky top-0 z-[1000]">
                <NavigationMain className="!gap-space-02" collapsed={open} onToggleCollapsed={toggle}>
                    <Link to={'/'} className="flex gap-space-02">
                        <img className="w-auto lg:flex" src={Logo} alt="logo" />
                    </Link>
                    <NavigationMenu dir={dir}>
                        <NavigationMenuList>
                            {[...menuItems].map(m => (
                                <NavigationMenuItem key={m.title}>
                                    {
                                        <NavigationMenuLink asChild>
                                            <Link key={m.id} to={m?.href}>
                                                {m.title}
                                            </Link>
                                        </NavigationMenuLink>
                                    }
                                </NavigationMenuItem>
                            ))}
                        </NavigationMenuList>
                    </NavigationMenu>
                    <NavigationActions className="ms-auto gap-space-02">
                        {/* <NavigationSearch onSearch={() => console.log('search')} /> */}
                        <div className="hidden md:flex">
                            <NavigationSwitchLanguage />
                        </div>
                            <NavigationAction className="gap-space-01" asChild>
                            <NavLink to={pathNames.login}>
                                <Person />
                                <span className="sr-only lg:not-sr-only">{strings.login || 'Login'}</span>
                            </NavLink>
                        </NavigationAction>
                        <NavigationMobileSideBar open={open} onOpenChange={toggle}>
                            <NavigationMobileHeader>
                                <Stack gap={2}>
                                    <NavigationHeaderLogo className="flex w-fit" logoSrc={Logo} logoAlt="logo" />
                                </Stack>
                            </NavigationMobileHeader>
                            <NavigationMobileBody>
                                <Stack direction={'col'}>
                                    {[...menuItems].map(m => (
                                        <NavigationMobileLink key={m.id} asChild>
                                            <NavLink key={m.id} to={m?.href}>
                                                {m.title}
                                            </NavLink>
                                        </NavigationMobileLink>
                                    ))}
                                </Stack>
                            </NavigationMobileBody>
                            <NavigationMobileFooter>
                                <Button rounded={'default'} asChild>
                                    <NavigationMobileLink asChild>
                                        <NavLink to={'/login'}>
                                            <span>{strings.login || 'Login'}</span>
                                        </NavLink>
                                    </NavigationMobileLink>
                                </Button>
                            </NavigationMobileFooter>
                        </NavigationMobileSideBar>
                    </NavigationActions>
                </NavigationMain>
            </NavigationHeader>
        </>
    )
}

export default PortalTopHeader
