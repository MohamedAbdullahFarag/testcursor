import classNames from 'classnames'
import { Close } from 'google-material-icons/outlined'
import { Button, ScrollArea, Stack, useLanguage } from 'mada-design-system'
import { ReactNode } from 'react'
import { Else, If, Then, When } from 'react-if'
import { useLocation } from 'react-router-dom'
import { useMediaQuery } from 'usehooks-ts'
import { usePanelStore } from '../store/usePanelStore'

interface SidePanelProps {
    children: ReactNode
    position?: 'left' | 'right'
    className?: string
    title?: string
    isClosable?: boolean
}

const SidePanel = ({ children, position = 'left', className, title, isClosable }: SidePanelProps) => {
    const { dir } = useLanguage()
    const isSmallScreen = useMediaQuery('(max-width: 1279px)')
    const location = useLocation()
    const { isSidePanelVisible, isShowingMobile, hideSidePanel } = usePanelStore()
    const isVisible = isSmallScreen ? isShowingMobile : isSidePanelVisible(location.pathname)

    return (
        <div
            className={classNames(
                'relative flex w-full shrink-0 flex-col gap-space-05 rounded-4 bg-card py-space-05 shadow-surface-01 transition-all duration-300 ease-in-out xl:w-[38rem]',
                {
                    'order-last': position === 'left',
                    'order-first': position === 'right',
                    hidden: !isVisible,
                },
                className,
            )}>
            <When condition={!!title}>
                <Stack alignItems="center" gap={2} justifyContent="between" className="border-b px-space-04 pb-space-03">
                    <h2 className="text-body-02 font-semibold text-card-foreground">{title}</h2>
                    <When condition={isClosable}>
                        <Button variant="ghost" colors="gray" size="icon-sm" onClick={hideSidePanel}>
                            <Close className="size-[20px]" />
                        </Button>
                    </When>
                </Stack>
            </When>
            <If condition={isSmallScreen}>
                <Then>
                    <div className="h-full">
                        <div className="flex flex-col gap-space-05 px-space-04 pb-space-09 sm:p-space-05">{children}</div>
                    </div>
                </Then>
                <Else>
                    <ScrollArea className="h-full rounded-4" dir={dir}>
                        <div className="flex flex-col gap-space-05 px-space-04">{children}</div>
                    </ScrollArea>
                </Else>
            </If>
        </div>
    )
}

export default SidePanel
