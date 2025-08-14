import { Skeleton } from 'mada-design-system'

import { useMemo } from 'react'
import useGridColumns from '../hooks/useGridColumns'
import GridLayout from './GridLayout'

export const MainSkeleton = () => {
    const cardsTotal = useGridColumns()

    const skeletonCards = useMemo(() => {
        const arr = new Array(cardsTotal).fill(undefined)
        return arr
    }, [cardsTotal])

    return (
        <main className="flex flex-col gap-space-05">
            <GridLayout>
                {skeletonCards.map((_, i) => (
                    <Skeleton key={`support+${i}`} className="h-[198px] w-full rounded-4" />
                ))}
            </GridLayout>
            <Skeleton className="mx-auto h-16 w-64" />
        </main>
    )
}

export const CardsSkeleton = () => {
    return (
        <div className="grid grid-cols-none gap-space-05 gap-x-space-04 md:grid-cols-2 md:gap-x-space-04 xl:grid-cols-3">
            <Skeleton className="h-[27rem] w-full rounded-4" />
            <Skeleton className="h-[27rem] w-full rounded-4" />
            <Skeleton className="h-[27rem] w-full rounded-4" />
            <Skeleton className="h-[27rem] w-full rounded-4" />
            <Skeleton className="h-[27rem] w-full rounded-4" />
            <Skeleton className="h-[27rem] w-full rounded-4" />
        </div>
    )
}

export const AddAttachmentsSkeleton = () => {
    return (
        <div className="flex flex-col gap-space-04">
            <Skeleton className="h-16 w-full" />
            <div className="grid grid-cols-[13rem_1fr] gap-x-space-02 gap-y-space-04 ltr:grid-cols-[17rem_1fr]">
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
                <Skeleton className="h-10 w-full" />
            </div>
        </div>
    )
}
