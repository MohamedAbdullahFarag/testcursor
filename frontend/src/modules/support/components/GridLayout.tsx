import classNames from 'classnames'
import { ReactNode } from 'react'

interface GridProps {
    children: ReactNode
    className?: string
}

const GridLayout = ({ children, className }: GridProps) => {
    return (
        <div
            className={classNames(
                'grid grid-cols-1 gap-space-04 md:grid-cols-2 lg:grid-cols-3 2xl:grid-cols-4',

                className,
            )}>
            {children}
        </div>
    )
}

export default GridLayout
