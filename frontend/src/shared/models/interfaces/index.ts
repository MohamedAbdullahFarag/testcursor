export * from './LookupOption'

interface MetadataCamelCase {
    resultSet: ResultSet
}

interface ResultSet {
    count: number
    limit: number
    offset: number
}

export interface Lookup<T = string> {
    status: 'true' | 'false'
    data: {
        Id: T
        nameAr: string
        nameEn: string
    }
}

/**
 * API Response
 * @template K - The type of status
 * @template T - The type of data returned in the API response.
 * @property {MetadataLowerCase} metadata - metadata refer with MetadataLowerCase interface for resultset
 */
export interface PaginatedAPIResponse<T> {
    status: boolean | 'true' | 'false'
    data: T[]
    metaData: MetadataCamelCase
}
export interface APIResponse<T> {
    status: boolean | 'true' | 'false'
    data: T
}
export interface Errors {
    data: ErrorsList
}

export interface ErrorsList {
    errors: {
        code: string
        message: string
        messageAr: string
    }[]
}

export interface ResponseMessage {
    messageEn: string
    messageAr: string
}

export interface APIResponseError {
    status: false | number
    errors: ResponseMessage[]
    logicalError: unknown
    exception: unknown
}
