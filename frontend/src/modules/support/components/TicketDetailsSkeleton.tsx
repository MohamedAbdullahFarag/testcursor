import { Skeleton } from 'mada-design-system'

const TicketDetailsSkeleton = () => {
    return (
        <div className="flex flex-col gap-space-08">
            <div className="flex flex-col gap-space-04">
                <Skeleton className="h-16 w-6/12" />
                <section className="grid grid-cols-[13rem_1fr] gap-x-space-02 gap-y-space-04 ltr:grid-cols-[17rem_1fr]">
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                    <Skeleton className="title-01 h-7 w-full" />
                </section>
            </div>
            <Skeleton className="title-01 mt-auto h-36 w-full" />
        </div>
    )
}

export default TicketDetailsSkeleton
