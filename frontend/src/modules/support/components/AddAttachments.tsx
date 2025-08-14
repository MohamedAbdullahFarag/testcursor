import { CheckCircle } from 'google-material-icons/outlined'
import { When } from 'react-if'

import { useAuthStore } from '@/modules/auth/store/authStore'
import { strings } from '@/shared/locales'
import { Alert, AlertDescription, AlertTitle, Skeleton } from 'mada-design-system'
import { useGetCaseByCaseId } from '../services/useServiceNow'
import Attachments from './Attachments'
import TicketDetails from './TicketDetails'

export interface AddAttachmentsProps {
    CaseId: string
    setFile: React.Dispatch<React.SetStateAction<File | null>>
    setIsError: React.Dispatch<React.SetStateAction<boolean>>
    isSuccess?: boolean
}

const AddAttachments = ({ CaseId, setFile, setIsError, isSuccess }: AddAttachmentsProps) => {
    const { isAuthenticated } = useAuthStore()
    const { data, isLoading, isFetching } = useGetCaseByCaseId(CaseId)

    if (isLoading || isFetching) return <AddAttachmentsSkeleton />

    if (isSuccess)
        return (
            <div className="flex flex-col gap-space-05">
                <Alert variant="outline" colors="primary">
                    <CheckCircle className="h-8 w-8 text-primary" />
                    <div className="flex flex-col">
                        <AlertTitle>{strings.support.fileSubmittedSuccessfully}</AlertTitle>
                        <AlertDescription>{strings.support.fileSubmittedSuccessfullyDesc}</AlertDescription>
                    </div>
                </Alert>
                <TicketDetails Ticket={data?.data} />
            </div>
        )

    return (
        <div className="flex flex-col gap-space-05">
            <When condition={isAuthenticated}>
                <Alert variant="outline" colors="primary">
                    <CheckCircle className="h-8 w-8 text-primary" />
                    <div className="flex flex-col">
                        <AlertTitle>{strings.formatString(strings.support.ticketSubmittedSuccessfully, data?.data?.caseNum ?? '')}</AlertTitle>
                        <AlertDescription>{strings.support.youCanAddAttachment}</AlertDescription>
                    </div>
                </Alert>
            </When>
            <TicketDetails Ticket={data?.data}>
                <Attachments setFile={setFile} setIsError={setIsError} isSuccess={isSuccess} />
            </TicketDetails>
        </div>
    )
}

export default AddAttachments

const AddAttachmentsSkeleton = () => {
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
