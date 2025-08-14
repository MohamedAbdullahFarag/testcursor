import { RouteObject } from 'react-router-dom'
import dashboardRoutes from './DashboardRoutes'
import portalRoutes from './PortalRoutes'

const appRoutes: RouteObject[] = [portalRoutes, dashboardRoutes]

export default appRoutes
