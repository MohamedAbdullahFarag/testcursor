import { ComponentProps, SVGProps } from 'react'
import { THEME } from '../models/enums'
import ErrorBoundary from './ErrorBoundary'
import { useThemeStore } from 'mada-design-system'

export type StreamLineIconType = (props: ComponentProps<'svg'>) => JSX.Element
interface Props extends SVGProps<SVGSVGElement> {
    light?: StreamLineIconType
    dark?: StreamLineIconType
}

const fallback = <div className="h-space-05 w-space-05 rounded-1 bg-background" />

const StreamLineIcon = ({ light: Light, dark: Dark, ...props }: Props) => {
    const theme = useThemeStore(s => s.theme)
    if (!Light || !Dark) return fallback

    return <ErrorBoundary fallbackComponent={fallback}>{+theme === THEME.LIGHT ? <Light {...props} /> : <Dark {...props} />}</ErrorBoundary>
}

export default StreamLineIcon
