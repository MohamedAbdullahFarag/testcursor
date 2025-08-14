//import { logFailed } from '@/Shared/Lib/logger'
import { useAuthStore } from '@/modules/auth/store/authStore'
import { UserTypes } from '@/shared/models/enums'
import http from '@/shared/services/http'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AxiosResponse } from 'axios'
import { useLanguage } from 'mada-design-system'
import {
    CaseCommentsResponse,
    GetAttachmentsByCaseIdResponse,
    GetCaseByCaseNumAndSourceAndUserIdResponse,
    GetCasesByUserIdResponse,
    GetSupportCategoriesResponse,
    GetSupportSubCategoriesResponse,
    GetTotalActiveTicketsResponse,
    SubmitTicketResponse,
    SupportAddAttachmentPayload,
} from '../models/interfaces'
import { createTicketSchemaType, ticketInquirySchemaType } from '../schema'

const apiEndpointServiceNow = import.meta.env.VITE_UAP_API_SERVICENOW_PREFIX + 'serviceNow/v1/'
const source = 'unified_admission'

const serviceNowRoles: Record<UserTypes, number> = {
    [UserTypes.Representative]: 230,
    [UserTypes.Supervisor]: 220,
    [UserTypes.Candidate]: 250,
    [UserTypes.Anonymous]: 240,
}

const useGetCategories = ({ enabled, defaultRole }: { enabled?: boolean; defaultRole: UserTypes }) => {
    const { lang } = useLanguage()
    return useQuery({
        queryKey: [`supportCategories${defaultRole}`],
        queryFn: () =>
            http
                .get<GetSupportCategoriesResponse>(
                    `${apiEndpointServiceNow}categories?source=${source}&lang=${lang}&roles=${serviceNowRoles[defaultRole]}`,
                )
                .then(res => {
                    //   logFailed.info({ service: 'TicketSupport', operation: 'categories', type: 'XHR' })
                    return res.data
                })
                .catch(err => {
                    throw err
                }),
        // .catch(error => logFailed.error({ error, service: 'TicketSupport', operation: 'Categories', type: 'XHR' })),
        enabled,
    })
}

const useGetSubCategories = ({ categoryId, defaultRole }: { categoryId: string; defaultRole: UserTypes }) => {
    const { lang } = useLanguage()
    return useQuery({
        queryKey: [`supportCategories${defaultRole}`, `supportSubCategories${defaultRole}`, categoryId],
        queryFn: () =>
            http
                .get<GetSupportSubCategoriesResponse>(
                    `${apiEndpointServiceNow}subCategories/${categoryId}?source=${source}&lang=${lang}&roles=${serviceNowRoles[defaultRole]}`,
                )
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'subCategories', type: 'XHR' })
                    return res.data
                }),
        // .catch(error => logFailed.error({ error, service: 'TicketSupport', operation: 'SubCategories', type: 'XHR' })),
        enabled: !!categoryId,
    })
}

const useSubmitTicket = () => {
    const { isAuthenticated } = useAuthStore()
    const queryClient = useQueryClient()
    return useMutation({
        mutationKey: ['submitTicket'],
        mutationFn: (data: createTicketSchemaType) =>
            http
                .post<createTicketSchemaType, SubmitTicketResponse>(
                    `${apiEndpointServiceNow}${isAuthenticated ? 'loggedInUserCases' : 'cases'}?source=${source}`,
                    data,
                )
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'cases', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    // logFailed.error({ error, service: 'TicketSupport', operation: 'Cases', type: 'XHR' })
                    console.log(error)

                    throw error
                }),
        onSuccess: (res, inputReq) => {
            if (res?.data.caseId) {
                queryClient.invalidateQueries({ queryKey: ['getCaseByCaseNumAndUserId'] })
                queryClient.invalidateQueries({ queryKey: ['getCasesByUserId', inputReq.id] })
            }
        },
    })
}

const useGetCaseByCaseNumAndSourceAndUserId = () => {
    return useMutation({
        mutationKey: ['getCaseByCaseNumAndUserId'],
        mutationFn: (data: ticketInquirySchemaType) =>
            http
                .get<ticketInquirySchemaType, AxiosResponse<GetCaseByCaseNumAndSourceAndUserIdResponse>>(
                    `${apiEndpointServiceNow}cases/${data?.caseNum}?source=${source}&userId=${data?.userId}`,
                )
                .then(res => {
                    //logFailed.info({ service: 'TicketSupport', operation: 'casesByNumUser', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    // logFailed.error({ error, service: 'TicketSupport', operation: 'CasesByNumUser', type: 'XHR' })
                    throw error
                }),
    })
}
const useGetCaseByCaseId = (caseId?: string) => {
    return useQuery({
        queryKey: [`getCaseByCaseId-${caseId}`],
        queryFn: () =>
            http.get<GetCaseByCaseNumAndSourceAndUserIdResponse>(`${apiEndpointServiceNow}cases/${caseId}?source=${source}`).then(res => {
                //   logFailed.info({ service: 'TicketSupport', operation: 'CaseById', type: 'XHR' })
                return res.data
            }),
        //.catch(error => logFailed.error({ error, service: 'TicketSupport', operation: 'CaseById', type: 'XHR' })),

        retry: false,
    })
}

const useGetCasesByUserId = ({ userId, Limit, Offset }: { userId: string; Limit: number; Offset: number }) => {
    return useQuery({
        queryKey: ['getCasesByUserId', userId, Offset, Limit],
        queryFn: () =>
            http
                .get<GetCasesByUserIdResponse>(`${apiEndpointServiceNow}cases?source=${source}&userId=${userId}&limit=${Limit}&offset=${Offset}`)
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'casesByUser', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    //logFailed.error({ error, service: 'TicketSupport', operation: 'CasesByUser', type: 'XHR' })
                    throw error
                }),
        enabled: !!userId,
        retry: false,
        refetchOnWindowFocus: false,
    })
}

const useUploadAttachment = ({ caseId }: { caseId: string }) => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationKey: [`TicketAttatchements-${caseId}`],
        mutationFn: (data: SupportAddAttachmentPayload) =>
            http
                .post<SupportAddAttachmentPayload, SubmitTicketResponse>(`${apiEndpointServiceNow}cases/${caseId}/attachments?source=${source}`, data)
                .then(res => {
                    //logFailed.info({ service: 'TicketSupport', operation: 'attachment', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    //  logFailed.error({ error, service: 'TicketSupport', operation: 'Attachment', type: 'XHR' })
                    throw error
                }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [`TicketAttatchements-${caseId}`] })
        },
    })
}

const useGetAttachmentsByCaseId = ({ caseId, isEnabled = false }: { caseId?: string; isEnabled: boolean }) => {
    return useQuery({
        queryKey: [`TicketAttatchements-${caseId}`],
        queryFn: () =>
            http.get<GetAttachmentsByCaseIdResponse>(`${apiEndpointServiceNow}cases/${caseId}/attachments?source=${source}`).then(res => {
                //   logFailed.info({ service: 'TicketSupport', operation: 'AttachmentByCaseId', type: 'XHR' })
                return res.data
            }),
        // .catch(error => logFailed.error({ error, service: 'TicketSupport', operation: 'AttachmentByCaseId', type: 'XHR' })),

        enabled: isEnabled,
        retry: false,
    })
}

const useGetTotalActiveTickets = ({ email }: { email: string }) => {
    return useQuery({
        queryKey: ['getTotalActiveTickets'],
        queryFn: () =>
            http
                .get<GetTotalActiveTicketsResponse>(`${apiEndpointServiceNow}cases?source=${source}&email=${email}`)
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'ActiveTickets', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    //logFailed.error({ error, service: 'TicketSupport', operation: 'ActiveTickets', type: 'XHR' })
                    throw error
                }),
    })
}

const useGetTicketComments = (caseId?: string) => {
    return useQuery({
        queryKey: [`TicketComments-${caseId}`],
        queryFn: () =>
            http
                .get<CaseCommentsResponse>(`${apiEndpointServiceNow}cases/${caseId}/comments?source=${source}&limit=10&offset=1`)
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'TicketComments', type: 'XHR' })
                    return res?.data?.data || []
                })
                .catch(error => {
                    //logFailed.error({ error, service: 'TicketSupport', operation: 'TicketComments', type: 'XHR' })
                    throw error
                }),
        enabled: !!caseId,
    })
}

/**
 * add comment to specific Ticket
 * @param caseId string
 * @returns void
 */
const useCommentOnTicket = (caseId?: string) => {
    const queryClient = useQueryClient()

    return useMutation({
        mutationKey: ['add_ticket_comment'],
        mutationFn: (comments: string) =>
            http
                .post<{ comments: string }>(`${apiEndpointServiceNow}cases/${caseId}/comment?source=${source}`, { comments })
                .then(res => {
                    // logFailed.info({ service: 'TicketSupport', operation: 'add_ticket_comment', type: 'XHR' })
                    return res.data
                })
                .catch(error => {
                    //   logFailed.error({ error, service: 'TicketSupport', operation: 'add_ticket_comment', type: 'XHR' })
                    throw error
                }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [`TicketComments-${caseId}`] })
        },
    })
}

export {
    useCommentOnTicket,
    useGetAttachmentsByCaseId,
    useGetCaseByCaseId,
    useGetCaseByCaseNumAndSourceAndUserId,
    useGetCasesByUserId,
    useGetCategories,
    useGetSubCategories,
    useGetTicketComments,
    useGetTotalActiveTickets,
    useSubmitTicket,
    useUploadAttachment,
}
