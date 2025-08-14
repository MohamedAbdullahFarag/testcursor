import { Attachment } from 'google-material-icons/outlined'
import { useRef, useState } from 'react'
import { Case, Switch } from 'react-if'
import { useOnClickOutside } from 'usehooks-ts'

import { EmptyState } from '@/shared/components'
import { strings } from '@/shared/locales'
import { toBase64 } from '@/shared/utils'
import { Button, ScrollArea, Skeleton, Stack, useLanguage, useToast } from 'mada-design-system'
import { InboxGray } from 'streamline-icons'
import { TicketData } from '../models/interfaces'
import { useGetAttachmentsByCaseId, useUploadAttachment } from '../services/useServiceNow'
import AttachmentFile from './AttachmentFile'
import Attachments from './Attachments'
const ViewTicketAddAttatchments = ({ ticket }: { ticket?: TicketData }) => {
    const ref = useRef(null)

    const { dir } = useLanguage()
    const [showDropZone, setShowDropZone] = useState(false)
    const [file, setFile] = useState<File | null>(null)
    const [isUploadFileError, setIsUploadFileError] = useState(false)
    const { toast } = useToast()

    const { data, isError, isLoading } = useGetAttachmentsByCaseId({ caseId: ticket?.caseId, isEnabled: !!ticket?.caseId })
    const {
        mutateAsync: uploadAttachment,
        isPending: attachmentIsPending,
        // isSuccess: isSuccessAttachment,
        reset: resetAttachment,
    } = useUploadAttachment({ caseId: ticket?.caseId ?? '' })

    const onUploadAttachment = async () => {
        if (isUploadFileError) return
        if (file) {
            try {
                await toBase64(file).then(async base64 => {
                    await uploadAttachment({ file: (base64 as string)?.split('base64,')[1], fileName: file?.name, source: 'unified_portal' })
                        .then(() => {
                            setFile(null)
                            resetAttachment()
                            setShowDropZone(false)
                        })
                        .catch(err => {
                            toast({
                                title: err?.data?.errors?.[0]?.message,
                                variant: 'destructive',
                            })
                        })
                        .catch(err => {
                            console.log(err)
                        })
                })
            } catch (error) {
                console.log(error)
            }
        }
    }

    const handleClickOutside = () => setShowDropZone(false)

    useOnClickOutside(ref, handleClickOutside)

    return (
        <>
            <div className="col-span-2 mb-[100px] flex flex-wrap items-center gap-space-03 sm:col-span-1">
                <Switch>
                    <Case condition={isLoading}>
                        <Skeleton className="mb-space-01 h-[72px] w-full" />
                        <Skeleton className="h-[72px] w-full" />
                    </Case>
                    <Case condition={isError || !data?.data?.length}>
                        <div className="w-full">
                            <EmptyState
                                title={strings.support.noAttatchments}
                                description={strings.support.noAttatchmentsMsg}
                                icon={{ light: InboxGray, dark: InboxGray }}
                                showBackground
                            />
                        </div>
                    </Case>
                    <Case condition={!!data?.data.length}>
                        <ScrollArea dir={dir} className="max-h-[440px]">
                            <Stack gap={2} className="flex-wrap">
                                {data?.data?.map((file, i) => <AttachmentFile key={`file${i}`} file={file} />)}
                            </Stack>
                        </ScrollArea>
                    </Case>
                </Switch>
            </div>
            <div className="absolute bottom-0 left-0 right-0 bg-background p-space-05 pt-space-04">
                <div className="w-full">
                    {showDropZone ? (
                        <div ref={ref}>
                            <Attachments setFile={setFile} setIsError={setIsUploadFileError} />
                            <Button
                                className="mt-space-03 w-full"
                                colors="primary"
                                onClick={onUploadAttachment}
                                disabled={attachmentIsPending}
                                isLoading={attachmentIsPending}>
                                {strings.support.attachFile}
                            </Button>
                        </div>
                    ) : (
                        <Button className="mt-space-03 w-full gap-space-01" variant="outline" onClick={() => setShowDropZone(true)}>
                            <Attachment />
                            {strings.support.attachFile}
                        </Button>
                    )}
                </div>
            </div>
        </>
    )
}

export default ViewTicketAddAttatchments
