import SALogo from '@/assets/images/sa.png'

import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { UserTypes } from '@/shared/models/enums'
import { toBase64 } from '@/shared/utils'
import { zodResolver } from '@hookform/resolvers/zod'
import {
    Breadcrumbs,
    Button,
    ComboboxControl,
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    handleArabicNumbers,
    Input,
    Stack,
    Textarea,
    useForm,
    useToast,
} from 'mada-design-system'
import { useEffect, useState } from 'react'
import { Case, Default, Switch } from 'react-if'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import AddAttachments from '../components/AddAttachments'
import Sidebar from '../components/Sidebar'
import { createTicketSchema, createTicketSchemaType } from '../schema'
import { useGetCategories, useGetSubCategories, useSubmitTicket, useUploadAttachment } from '../services/useServiceNow'
const Create = () => {
    const naviagte = useNavigate()
    const { toast } = useToast()
    const [file, setFile] = useState<File | null>(null)
    const [isError, setIsError] = useState(false)

    const { data: categories, isLoading: categoriesLoading } = useGetCategories({ enabled: true, defaultRole: UserTypes.Anonymous })
    const { mutateAsync, isPending, isSuccess, data: ticketData, reset } = useSubmitTicket()
    const { mutateAsync: uploadAttachment, isPending: attachmentIsPending } = useUploadAttachment({ caseId: ticketData?.data?.caseId ?? '' })
    const { state } = useLocation() as { state: { ServiceCategory?: number; ServiceSubCategory?: number } }

    const form = useForm<createTicketSchemaType>({
        resolver: zodResolver(createTicketSchema),
        mode: 'onSubmit',
        defaultValues: {
            id: '',
            category: '',
            customerEmail: '',
            customerName: '',
            description: '',
            mobile: '',
            shortDescription: '',
            subCategory: '',
            source: 'unified_admission',
        },
    })
    const { watch, setValue } = form
    const watchCategory = watch('category')

    const {
        data: subCategories,
        isLoading: subCategoriesLoading,
        isFetching: subCategoriesFetching,
    } = useGetSubCategories({
        defaultRole: UserTypes.Anonymous,
        categoryId: form?.watch('category'),
    })

    const onSubmit = async (values: createTicketSchemaType) => {
        const submittedData = {
            ...values,
            mobile: `+966${values?.mobile}`,
            customFields: [
                {
                    key: 'u_cultural_mission',
                    value: '1100',
                },
            ],
        }

        try {
            await mutateAsync(submittedData)
        } catch (error) {
            console.log(error)
        }
    }

    const ticketSubmittedSuccessfully = () => {
        reset()
        form.reset({ ...form.formState.defaultValues })
        toast({
            title: `${strings.formatString(strings.support.ticketSubmittedSuccessfully, ticketData?.data?.caseNum ?? '')}`,
            description: strings.support.trackNewTicketMessage,
            variant: 'success',
            action: (
                <Button variant="outline" onClick={() => naviagte(`/support/details`, { state: { caseId: ticketData?.data?.caseId } })}>
                    {strings.support.displayTicket}
                </Button>
            ),
        })
    }

    const onUploadAttachment = async () => {
        if (isError) return
        if (file) {
            try {
                await toBase64(file).then(async base64 => {
                    await uploadAttachment({ file: (base64 as string)?.split('base64,')[1], fileName: file?.name, source: 'unified_portal' })
                        .then(() => {
                            ticketSubmittedSuccessfully()
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

    useEffect(() => {
        if (watchCategory) {
            setValue('subCategory', '')
        }
    }, [watchCategory, setValue])

    useEffect(() => {
        if (categories && state?.ServiceCategory) {
            setValue('category', String(state?.ServiceCategory))
        }
    }, [categories, setValue, state?.ServiceCategory])

    useEffect(() => {
        if (subCategories && state?.ServiceSubCategory) {
            setValue('subCategory', String(state?.ServiceSubCategory))
        }
    }, [subCategories, setValue, state?.ServiceSubCategory])

    return (
        <Stack gap={7} className="relative" direction={'col'}>
            <Switch>
                <Case condition={isSuccess && ticketData?.data?.caseId}>
                    <PortalHeaderPage
                        titlePage={strings.attachments.attachFile}
                        descriptionPage={strings.attachments.attachFileDesc}
                        breadcrumbs={
                            <Breadcrumbs
                                items={[
                                    {
                                        title: strings.shared.Main,
                                        render: <Link to="/">{strings.shared.Main}</Link>,
                                    },
                                    {
                                        title: strings.support.fullTitle,
                                        render: <Link to={pathNames.portalSupport}>{strings.support.fullTitle}</Link>,
                                    },
                                    {
                                        title: strings.support.inquiryTicket,
                                        render: <Link to={pathNames.portalSupportCreate}>{strings.support.inquiryTicket}</Link>,
                                    },
                                    {
                                        title: strings.attachments.attachFile,
                                    },
                                ]}
                            />
                        }
                    />
                </Case>
                <Default>
                    <PortalHeaderPage
                        titlePage={strings.support.createTicket}
                        descriptionPage={strings.support.createTicketDesc}
                        breadcrumbs={
                            <Breadcrumbs
                                items={[
                                    {
                                        title: strings.shared.Main,
                                        render: <Link to="/">{strings.shared.Main}</Link>,
                                    },
                                    {
                                        title: strings.support.fullTitle,
                                        render: <Link to={pathNames.portalSupport}>{strings.support.fullTitle}</Link>,
                                    },
                                    {
                                        title: strings.support.inquiryTicket,
                                    },
                                ]}
                            />
                        }
                    />
                </Default>
            </Switch>
            <Stack className="px-space-04 xl:px-space-06">
                <Stack
                    gap={6}
                    justifyContent={'between'}
                    alignItems={'start'}
                    className="mx-auto w-full max-w-container"
                    data-testid="ticketCardContainer">
                    <div className="grow pb-space-07">
                        <Switch>
                            <Case condition={isSuccess && ticketData?.data?.caseId}>
                                <Stack direction={'col'}>
                                    <AddAttachments CaseId={ticketData?.data?.caseId || ''} setFile={setFile} setIsError={setIsError} />
                                    <Stack gap={4}>
                                        <Button
                                            variant={'outline'}
                                            rounded={'default'}
                                            type="button"
                                            onClick={ticketSubmittedSuccessfully}
                                            disabled={attachmentIsPending}
                                            data-testid="skipBtnTest">
                                            {strings.shared.skip}
                                        </Button>

                                        <Button
                                            colors="primary"
                                            type="submit"
                                            rounded={'default'}
                                            onClick={onUploadAttachment}
                                            disabled={attachmentIsPending}
                                            data-testid="submitTicketBtnTest"
                                            isLoading={attachmentIsPending}>
                                            {strings.attachments.attachFile}
                                        </Button>
                                    </Stack>
                                </Stack>
                            </Case>
                            <Default>
                                <Form {...form}>
                                    <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-space-06" data-testid="ticketFormTest">
                                        <div className="flex flex-col gap-space-04">
                                            <h2 className="text-body-02 font-semibold">{strings.support.personalInfo}</h2>

                                            <div className="flex flex-col gap-space-05 sm:flex-row">
                                                <FormField
                                                    control={form.control}
                                                    name={'id'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="nationalResidenceIdTest">
                                                                {strings.shared.nationalId} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Input
                                                                    variant={'default'}
                                                                    placeholder={`${strings.shared.enter} ${strings.shared.nationalId.toLowerCase()}`}
                                                                    {...field}
                                                                    data-testid="nationalResidenceIdInputTest"
                                                                    maxLength={10}
                                                                    onChange={e => {
                                                                        field.onChange(handleArabicNumbers(e, form.watch('id')))
                                                                    }}
                                                                />
                                                            </FormControl>
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                                <FormField
                                                    control={form.control}
                                                    name={'customerName'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="fullNameLabelTest">
                                                                {strings.shared.fullName} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Input
                                                                    placeholder={`${strings.shared.enter} ${strings.shared.fullName.toLowerCase()}`}
                                                                    variant={'default'}
                                                                    data-testid="fullNameInputTest"
                                                                    {...field}
                                                                    maxLength={100}
                                                                />
                                                            </FormControl>
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                            </div>
                                            <div className="flex flex-col gap-space-05 sm:flex-row">
                                                <FormField
                                                    control={form.control}
                                                    name={'customerEmail'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="emailLabelTest">
                                                                {strings.shared.email} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Input
                                                                    placeholder={`${strings.shared.enter} ${strings.shared.email.toLowerCase()}`}
                                                                    variant={'default'}
                                                                    data-testid="emailInputTest"
                                                                    {...field}
                                                                    maxLength={50}
                                                                />
                                                            </FormControl>
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                                <FormField
                                                    control={form.control}
                                                    name={'mobile'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="mobileNumberLabelTest">
                                                                {strings.shared.mobileNumber} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Input
                                                                    placeholder={`${strings.shared.enter} ${strings.shared.mobileNumber.toLowerCase()}`}
                                                                    variant={'default'}
                                                                    {...field}
                                                                    data-testid="mobileNumberInputTest"
                                                                    maxLength={9}
                                                                    endAdornment={
                                                                        <div className="flex items-center gap-space-02 pl-space-05 ltr:hidden">
                                                                            <span
                                                                                dir="ltr"
                                                                                className="text-body-02 font-normal text-foreground-tertiary">
                                                                                +966
                                                                            </span>
                                                                            <img alt="saFlag" src={SALogo} data-testid="mobilePhoneLogo" />
                                                                        </div>
                                                                    }
                                                                    startAdornment={
                                                                        <div className="flex items-center gap-space-02 pl-space-05 rtl:hidden">
                                                                            <img alt="saFlag" src={SALogo} data-testid="mobilePhoneLogo" />
                                                                            <span
                                                                                dir="ltr"
                                                                                className="text-body-02 font-normal text-foreground-tertiary">
                                                                                +966
                                                                            </span>
                                                                        </div>
                                                                    }
                                                                />
                                                            </FormControl>
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                            </div>
                                        </div>

                                        <div className="flex flex-col gap-space-04">
                                            <h2 className="text-body-02 font-semibold">{strings.support.detrmineService}</h2>

                                            <div className="flex flex-col gap-space-05 sm:flex-row">
                                                <FormField
                                                    control={form.control}
                                                    name={'category'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="serviceCategoryLabelTest">
                                                                {strings.support.serviceCategory} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <ComboboxControl
                                                                disabled={!!state?.ServiceCategory}
                                                                isLoading={categoriesLoading}
                                                                options={categories?.data}
                                                                optionLabel="categoryDescription"
                                                                optionValue="categoryValue"
                                                                placeholder={`${strings.shared.select} ${strings.support.serviceCategory.toLowerCase()}`}
                                                                onChange={field.onChange}
                                                                value={field.value}
                                                                variant={'default'}
                                                            />
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                                <FormField
                                                    control={form.control}
                                                    name={'subCategory'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel data-testid="serviceTypesLabelTest">
                                                                {strings.support.serviceType} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <ComboboxControl
                                                                disabled={!subCategories || !!state?.ServiceSubCategory}
                                                                isLoading={subCategoriesLoading || subCategoriesFetching}
                                                                options={subCategories?.data}
                                                                optionLabel="subCategoryDescription"
                                                                optionValue="subCategoryValue"
                                                                placeholder={`${strings.shared.select} ${strings.support.serviceType.toLowerCase()}`}
                                                                onChange={field.onChange}
                                                                value={field.value}
                                                                variant={'default'}
                                                            />
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                            </div>
                                        </div>

                                        <div className="flex flex-col gap-space-04">
                                            <h2 className="text-body-02 font-semibold">{strings.support.helpYou}</h2>
                                            <FormField
                                                control={form.control}
                                                name={'shortDescription'}
                                                render={({ field }) => (
                                                    <FormItem className="flex-1">
                                                        <FormLabel data-testid="issueTitleLabelTest">
                                                            {strings.support.issueTitle} <span className="text-error">*</span>
                                                        </FormLabel>
                                                        <FormControl>
                                                            <Input
                                                                placeholder={`${strings.shared.enter} ${strings.support.issueTitle.toLowerCase()}`}
                                                                variant={'default'}
                                                                data-testid="issueTitleInputTest"
                                                                {...field}
                                                                maxLength={100}
                                                            />
                                                        </FormControl>
                                                        <FormMessage />
                                                    </FormItem>
                                                )}
                                            />
                                            <FormField
                                                control={form.control}
                                                name={'description'}
                                                render={({ field }) => (
                                                    <FormItem className="flex-1">
                                                        <FormLabel data-testid="descriptionsLabelTest">
                                                            {strings.shared.description} <span className="text-error">*</span>
                                                        </FormLabel>
                                                        <FormControl>
                                                            <Textarea
                                                                placeholder={`${strings.shared.enter} ${strings.shared.description.toLowerCase()}`}
                                                                variant={'default'}
                                                                data-testid="descriptionsTextAreaTest"
                                                                {...field}
                                                                maxLength={300}
                                                            />
                                                        </FormControl>
                                                        <FormMessage />
                                                    </FormItem>
                                                )}
                                            />
                                        </div>
                                        <div>
                                            <Button
                                                variant="default"
                                                colors="primary"
                                                rounded={'default'}
                                                disabled={isPending}
                                                isLoading={isPending}
                                                data-testid="continueToAddAttachmentTest">
                                                {strings.support.createTicket}
                                            </Button>
                                        </div>
                                    </form>
                                </Form>
                            </Default>
                        </Switch>
                    </div>
                    <Sidebar />
                </Stack>
            </Stack>
        </Stack>
    )
}

export default Create
