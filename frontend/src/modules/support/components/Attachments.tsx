import { FileInfo, FileUpload } from 'mada-design-system'

interface AttachmentsProps {
    setFile: React.Dispatch<React.SetStateAction<File | null>>
    setIsError: React.Dispatch<React.SetStateAction<boolean>>
    isSuccess?: boolean
}
const Attachments = ({ setFile, setIsError }: AttachmentsProps) => {
    const onFileSelected = (
        files: FileInfo[],
        updateCallback: (id: string, progress: number, status: 'uploading' | 'success' | 'error', error?: string) => void,
    ) => {
        console.log(files)
        if (files.length) {
            setFile(files?.[0].file)
            updateCallback(files?.[0].id, 100, 'success')
            setIsError(false)
            return
        }
        setIsError(true)
    }

    return (
        <div className="flex flex-col gap-space-02">
            <FileUpload onFileSelect={onFileSelected} maxSize={5} />
        </div>
    )
}

export default Attachments
