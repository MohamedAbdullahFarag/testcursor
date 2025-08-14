import { useState } from 'react'
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3'

export const useRecaptcha = (action: string = 'Post') => {
    const { executeRecaptcha } = useGoogleReCaptcha()
    const [isRecaptchaPending, setIsRecaptchaPending] = useState(false)

    const getToken = async () => {
        if (!executeRecaptcha) {
            throw new Error('reCAPTCHA not available')
        }

        try {
            setIsRecaptchaPending(true)
            return await executeRecaptcha(action)
        } finally {
            setIsRecaptchaPending(false)
        }
    }

    return {
        getToken,
        isRecaptchaPending,
    }
}
