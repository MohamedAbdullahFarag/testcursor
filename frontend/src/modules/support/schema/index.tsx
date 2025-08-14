import { strings } from '@/shared/locales'
import { langRegex } from 'mada-design-system'

import * as z from 'zod'

export const createTicketSchema = z.object({
    customerName: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.fullName.toLowerCase())}`)
        .max(100, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`)
        .regex(
            langRegex.bothArAndEnRegexonlyLetters,
            `${strings.formatString(strings.sharedValidation.acceptLetters, strings.shared.fullName?.toLowerCase())}`,
        )
        .optional(),
    customerEmail: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.email.toLowerCase())}`)
        .max(50, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`)
        .email(`${strings.formatString(strings.sharedValidation.valid, strings.shared.email)}`),
    mobile: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.mobileNumber.toLowerCase())}`)
        .max(9, strings.sharedValidation.exceedsTheAllowedLimit)
        .regex(langRegex.mobileNumber, `${strings.formatString(strings.sharedValidation.valid, strings.shared.mobileNumber)}`),
    id: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.nationalId + '/' + strings.shared.residentID)}`)
        .max(10, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`)
        .regex(
            langRegex.nationalOrResidentId,
            `${strings.formatString(strings.sharedValidation.valid, strings.shared.nationalId + '/' + strings.shared.residentID)}`,
        )
        .optional(),
    category: z.string().min(1, `${strings.formatString(strings.sharedValidation.requiredSelectThe, strings.support.serviceCategory.toLowerCase())}`),
    subCategory: z.string().min(1, `${strings.formatString(strings.sharedValidation.requiredSelectThe, strings.support.serviceType.toLowerCase())}`),
    shortDescription: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.support.issueTitle.toLowerCase())}`)
        .max(100, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`),
    description: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.description.toLowerCase())}`)
        .max(300, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`),
    source: z.string(),
})

export type createTicketSchemaType = z.infer<typeof createTicketSchema>

export const ticketInquirySchema = z.object({
    userId: z
        .string()
        .min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.shared.nationalId + '/' + strings.shared.residentID)}`)
        .max(10, `${strings.formatString(strings.sharedValidation.exceedsTheAllowedLimit)}`)
        .regex(
            langRegex.nationalOrResidentId,
            `${strings.formatString(strings.sharedValidation.valid, strings.shared.nationalId + '/' + strings.shared.residentID)}`,
        )
        .optional(),
    caseNum: z.string().min(1, `${strings.formatString(strings.sharedValidation.requiredThe, strings.support.ticketNumber)}`),
})

export type ticketInquirySchemaType = z.infer<typeof ticketInquirySchema>
