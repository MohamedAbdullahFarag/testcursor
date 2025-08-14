import { AxiosError, isAxiosError } from 'axios'
import { strings } from '../locales'
import { APIResponseError } from '../models/interfaces'

export const getResponseErrorMessage = (error: unknown): string => {
    const locale = document.dir ?? ('rtl' as 'ltr' | 'rtl')
    if (isAxiosError(error)) {
        const axiosError = error as AxiosError<APIResponseError>

        if (axiosError.response && axiosError.response.data) {
            const errorData = axiosError.response.data

            if (errorData.errors && errorData.errors.length > 0) {
                return locale === 'rtl' ? errorData.errors[0]?.messageAr : (errorData.errors[0]?.messageEn ?? errorData.errors[0]?.messageAr)
            }
        }
        return strings.shared.serviceUnavailable
    }
    return strings.shared.serverError
}
