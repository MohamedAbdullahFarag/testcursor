export function downloadFile(base64: string) {
    let base64Modified = base64
    if (base64.startsWith('JVB')) {
        base64Modified = 'data:application/pdf;base64,' + base64
    } else if (base64.startsWith('/9j')) {
        base64Modified = 'data:image/jpg;base64,' + base64
    } else if (base64.startsWith('iVB')) {
        base64Modified = 'data:image/png;base64,' + base64
    }
    return base64Modified
}

export const toBase64 = (file: File) =>
    new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.readAsDataURL(file)
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
    })

export const formatFileSize = (sizeBytes: number): string => {
    if (sizeBytes >= 1024 * 1024) {
        return `${(sizeBytes / 1024 / 1024).toFixed(2)} MB`
    } else if (sizeBytes >= 1024) {
        return `${(sizeBytes / 1024).toFixed(2)} KB`
    } else {
        return `${sizeBytes} bytes`
    }
}