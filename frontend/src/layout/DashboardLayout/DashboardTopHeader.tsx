import Logo from '@/assets/images/Logo ME.svg'
import useLogout from '@/modules/auth/hooks/useLogout'
import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { ChevronLeft, ExpandMore, Language, Logout, Menu } from 'google-material-icons/outlined'
import {
    accessibilityTools,
    Button,
    NavigationSwitchTheme,
    Popover,
    PopoverClose,
    PopoverContent,
    PopoverTrigger,
    SearchInput,
    Stack,
    useLanguage,
    useSidebar,
} from 'mada-design-system'
import { useState } from 'react'
import { When } from 'react-if'
import { useMediaQuery } from 'usehooks-ts'
import Accessibility from '../Accessibility'

const DashboardTopHeader = () => {
    const { setOpenMobile } = useSidebar()
    const { changeLang, lang } = useLanguage()
    const isSmScreen = useMediaQuery('(max-width: 1279px)')
    const logout = useLogout()
    const isActive = accessibilityTools(state => state.isActive)
    const [searchText, setSearchText] = useState('')

    const onSearch = (val: string) => {
        setSearchText(val)
    }
    return (
        <header
            className={classNames({
                'flex !h-[72px] shrink-0 items-center justify-between p-space-03': true,
                grayscale: isActive,
            })}>
            <Stack gap={2} alignItems="center">
                <When condition={isSmScreen}>
                    <Button size="icon-sm" onClick={() => setOpenMobile(true)} colors="gray" variant="ghost">
                        <span className="sr-only">Open menu</span>
                        <Menu className="size-space-05" />
                    </Button>
                </When>
                <Stack direction="col" gap={1}>
                    <img src={Logo} alt="Logo" />
                    <p
                        className="hidden items-center justify-center rounded-full bg-primary-container px-[6px] py-[2px] text-[10px] text-primary xl:flex"
                        data-testid="trialVersionTest">
                        {strings.shared.trialVersion}
                    </p>
                </Stack>
                <SearchInput
                    type="onButton"
                    placeholder={strings.shared.srachForRequiredService}
                    onSearch={onSearch}
                    value={searchText}
                    className="hidden xl:inline-flex"
                />
            </Stack>
            <Stack gap={'none'} alignItems="center">
                <div className="hidden sm:inline-block">
                    <Accessibility />
                </div>
                <div className="hidden sm:inline-block">
                    <NavigationSwitchTheme colors={'gray'} />
                </div>
                <Button
                    variant="text"
                    size={'sm'}
                    colors="gray"
                    className="gap-space-02"
                    onClick={() => {
                        changeLang(lang === 'ar' ? 'en' : 'ar')
                    }}>
                    {lang === 'ar' ? 'English' : 'العربية'}
                    <Language />
                </Button>
                <Popover>
                    <PopoverTrigger asChild>
                        <Button variant="outline" colors="primary" className="gap-space-01">
                            <span className="text-body-01 text-card-foreground">عبدلله الماجد</span>
                            <ExpandMore className="size-space-05 text-card-foreground" />
                        </Button>
                    </PopoverTrigger>
                    <PopoverContent align="end" side="bottom" className="flex flex-col border-none bg-background">
                        <PopoverClose
                            className="flex items-center rounded-3 bg-card px-space-03 py-space-02 text-foreground transition duration-300 hover:bg-card-hover"
                            onClick={logout}>
                            <div className="p-space-02">
                                <Logout className="text-primary" />
                            </div>
                            <span className="inline-block flex-grow text-right text-body-01">{strings.auth.logout}</span>
                            <div className="p-space-02">
                                <ChevronLeft className="ltr:rotate-180" />
                            </div>
                        </PopoverClose>
                    </PopoverContent>
                </Popover>
            </Stack>
        </header>
    )
}

export default DashboardTopHeader
