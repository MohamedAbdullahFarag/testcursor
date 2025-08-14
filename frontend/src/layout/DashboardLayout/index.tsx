import classNames from 'classnames'
import { accessibilityTools, SidebarProvider, Toaster } from 'mada-design-system'
import { Outlet } from 'react-router-dom'
import DashboardSideBar from './DashboardSideBar'
import DashboardTopHeader from './DashboardTopHeader'

const DashboardLayout = () => {
    const isActive = accessibilityTools(state => state.isActive)

    return (
        <SidebarProvider className="relative flex min-h-screen flex-col bg-background">
            <DashboardTopHeader />
            <div className="flex flex-grow overflow-hidden">
                <DashboardSideBar />
                <main
                    className={classNames({
                        'flex-grow overflow-auto': true,
                        grayscale: isActive,
                    })}>
                    <div className="flex h-full w-full gap-space-03 p-space-03">
                        <Outlet />
                    </div>

                </main>
                <Toaster/>
            </div>
        </SidebarProvider>
    )
}

export default DashboardLayout
