import { APIResponse, PaginatedAPIResponse } from '@/shared/models/interfaces'

export interface SupportCategory {
    categoryDescription: string
    categoryNumber: string
    categoryRoles: string
    categoryValue: string
}
export interface SupportSubCategory {
    subCategoryDescription: string
    subCategoryNum: string
    subCategoryRoles: string
    subCategoryValue: string
}

export type GetSupportCategoriesResponse = APIResponse<SupportCategory[]>
export type GetSupportSubCategoriesResponse = APIResponse<SupportSubCategory[]>
export type GetCasesByUserIdResponse = {
    data: TicketData[]
    metadata: {
        resultset: {
            count: string
            limit: string
            offset: string
        }
    }
}

export interface SubmitTicketResponse {
    data: {
        data: {
            caseId: string
            caseNum: string
        }
        status: string
    }
}
export interface Captcha {
    token: string
}

export type GetCaseByCaseNumAndSourceAndUserIdResponse = APIResponse<TicketData>

export interface GetCaseByCaseNumAndSourceAndUserIdPayload {
    Captcha: Captcha
}
export type StatusCode = 1 | 2 | 3 | 13 | 6 | 7

export interface TicketData {
    caseId: string
    caseNum: string
    category: string
    clientIP: string
    clientLocation: string
    clientMachine: string
    contactType: string
    createdOn: string
    createdBy: string
    customFields: {
        key: string
        value: string
    }[]
    customerEmail: string
    customerName: string
    description: string
    id: string
    lastComment: string
    lastCommentUpdated: string
    lastCommentUpdatedBy: string
    lastUpdated: string
    lastUpdatedBy: string
    loginId: string
    mobile: string
    priority: string
    repeatedCase: string
    resolutionNotes: string
    shortDescription: string
    source: string
    state: string
    stateId: StatusCode
    subcategory: string
    workNotes: string
}

export interface SupportAddAttachmentPayload {
    file: string
    fileName: string
    source: string
}

export interface ServiceNowAttachment {
    averageImageColor: string
    caseId: string
    compressed: string
    contentType: string
    file: string
    fileName: string
    hash: string
    ImageWidth: string
    ImageHeight: string
    sizeBytes: string
    sizeCompressed: string
    state: string
    sysCreatedBy: string
    sysCreatedOn: string
    sysModCount: string
    sysTags: string
    sysUpdatedBy: string
    sysUpdatedOn: string
    tableName: string
    tableSysId: string
}
export type GetAttachmentsByCaseIdResponse = APIResponse<ServiceNowAttachment[]>

export type GetTotalActiveTicketsResponse = APIResponse<{
    incidentCount: string
    requestCount: string
}>

export interface CaseComment {
    record_number: number
    comment: string
    user: string
    date: string
}
export type CaseCommentsResponse = PaginatedAPIResponse<CaseComment[]>
