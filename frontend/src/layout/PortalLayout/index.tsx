import { accessibilityTools, cn, Toaster } from 'mada-design-system'
import { Outlet, ScrollRestoration } from 'react-router-dom'
import PortalFooter from './PortalFooter'
import PortalTopHeader from './PortalTopHeader'

const PortalLayout = () => {
    const isActive = accessibilityTools(state => state.isActive)

    return (
        <div
            className={cn('relative flex min-h-screen flex-col bg-background', {
                grayscale: isActive,
            })}>
            <PortalTopHeader />
            <ScrollRestoration />
            <main className="flex-grow overflow-auto">
                <Outlet />
            </main>
            <Toaster />
            <PortalFooter />
        </div>
    )
}

export default PortalLayout
