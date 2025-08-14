import { strings } from '@/shared/locales'
import { downloadFile, formatFileSize } from '@/shared/utils'
import { Description } from 'google-material-icons/filled'
import { FileDownload } from 'google-material-icons/outlined'
import { Button } from 'mada-design-system'
import { ServiceNowAttachment } from '../models/interfaces'

const AttachmentFile = ({ file }: { file: ServiceNowAttachment }) => {
    return (
        <div className="flex items-center gap-space-02 rounded border border-border-primary bg-background-neutral-100 p-space-02" key={file.fileName}>
            <div className="rounded border border-border-primary bg-card p-space-02">
                <Description />
            </div>
            <div className="flex flex-col">
                <p className="text-body-02">{file?.fileName}</p>
                <span className="inline-block text-caption-01 text-foreground-tertiary"> {formatFileSize(Number(file?.sizeBytes || 0))}</span>
            </div>
            <Button size={'icon-sm'} variant={'text'} asChild tooltip={strings.shared.download}>
                <a href={downloadFile(file?.file)} download>
                    <FileDownload />
                </a>
            </Button>
        </div>
    )
}

export default AttachmentFile
