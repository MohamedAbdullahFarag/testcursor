import MOESpinner from '@/assets/images/MOESpinner.svg'
import classNames from 'classnames'
import { accessibilityTools } from 'mada-design-system'
export const PageLoader = () => {
    const isActive = accessibilityTools(state => state.isActive)

    return (
        <div
            className={classNames({
                'fixed left-0 top-0 z-[1000] flex h-full w-full items-center justify-center': true,
                grayscale: isActive,
            })}>
            <div className="relative size-[18rem]">
                {/* Spinner */}
                <div className="absolute inset-0 z-50 animate-spin rounded-full border-4 border-t-cyan-600"></div>
                {/* Image + Padding */}
                <div className="absolute inset-0 flex items-center justify-center rounded-full bg-card p-space-07">
                    <div className="h-full w-full rounded-full">
                        <img src={MOESpinner} alt="Loading" className="h-full w-full" />
                    </div>
                </div>
            </div>
        </div>
    )
}
