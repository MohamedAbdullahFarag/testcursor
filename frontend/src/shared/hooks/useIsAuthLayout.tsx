import { useLocation } from 'react-router-dom'

function useIsAuthLayout() {
    const { pathname } = useLocation()
    return pathname.indexOf('dashboard') >= 0
}
export default useIsAuthLayout
