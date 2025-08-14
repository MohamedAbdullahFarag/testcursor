import { usePanelStore } from '@/shared/store/usePanelStore'
import classNames from 'classnames'
import { ScrollArea, useLanguage } from 'mada-design-system'
import { ReactNode, useLayoutEffect } from 'react'
import { Else, If, Then } from 'react-if'
import { useLocation } from 'react-router-dom'
import { useMediaQuery } from 'usehooks-ts'

interface MainPanelProps {
    children: ReactNode
    className?: string
    hasPadding?: boolean
}

const MainPanel = ({ children, className, hasPadding = true }: MainPanelProps) => {
    const { dir } = useLanguage()
    const location = useLocation()
    const isSmallScreen = useMediaQuery('(max-width: 1279px)')
    const { setCurrentRoute, isShowingMobile } = usePanelStore()

    useLayoutEffect(() => {
        setCurrentRoute(location.pathname)
    }, [location.pathname, setCurrentRoute])

    const isVisible = isSmallScreen ? !isShowingMobile : true

    return (
        <div
            className={classNames(
                'min-w-0 flex-1 rounded-4 bg-card shadow-surface-01 transition-all duration-300',
                {
                    'hidden xl:block': !isVisible, // Hide on mobile when side panel is visible
                },
                className,
            )}>
            <If condition={isSmallScreen}>
                <Then>
                    <div
                        className={classNames({
                            'flex flex-col gap-space-05 px-space-04 py-space-05 sm:px-space-06': hasPadding,
                            'pb-space-05': hasPadding,
                        })}>
                        {children}
                    </div>
                </Then>
                <Else>
                    <ScrollArea className="h-full rounded-4 bg-card" dir={dir}>
                        <div
                            className={classNames('flex flex-col gap-space-05', {
                                'p-space-05': hasPadding,
                                'pb-space-05': !hasPadding,
                            })}>
                            {children}
                        </div>
                    </ScrollArea>
                </Else>
            </If>
        </div>
    )
}

export default MainPanel
