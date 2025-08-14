import { useAuthStore } from '@/modules/auth/store/authStore'
import { strings } from '@/shared/locales'
// import { Error } from '@/shared/models/interfaces'
import { UserTypes } from '@/shared/models/enums'
import { toBase64 } from '@/shared/utils'
import { zodResolver } from '@hookform/resolvers/zod'
import { AddCircle } from 'google-material-icons/outlined'
import {
    Button,
    ComboboxControl,
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    Input,
    Sheet,
    SheetBody,
    SheetContent,
    SheetFooter,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
    Textarea,
    useForm,
    useToast,
} from 'mada-design-system'
import { useEffect, useMemo, useState } from 'react'
import { Else, If, Then, Unless } from 'react-if'
import { useLocation, useNavigate } from 'react-router-dom'
import { AddAttachments } from '.'
import { createTicketSchema, createTicketSchemaType } from '../schema'
import { useGetCategories, useGetSubCategories, useSubmitTicket, useUploadAttachment } from '../services/useServiceNow'
import NotAvailableSupport from './NotAvailableSupport'

interface CreateTicketDashboardProps {
    isOutlined?: boolean
}
const Create = ({ isOutlined }: CreateTicketDashboardProps) => {
    const navigate = useNavigate()
    const { user } = useAuthStore(state => state)

    //const { data: userEmailsData, isError: isErrorEmail } = useGetEmailBasedOnRoles({ nationalId: user?.id, roleName: user?.defaultRole })
    // const { data: phoneBasedOnRole, isError: isErrorMobile } = useGetTelephoneBasedOnRoles({ nationalId: user?.id, roleName: user?.defaultRole })

    const [file, setFile] = useState<File | null>(null)
    const [isError, setIsError] = useState(false)
    const { toast } = useToast()
    const { state } = useLocation() as {
        state: {
            ServiceCategory?: number
            ServiceSubCategory?: number
            location?: string
            noDefaultCategory?: boolean
        }
    }
    const [isSheetOpen, setIsSheetOpen] = useState(!!state?.ServiceCategory || state?.noDefaultCategory)
    const {
        data: categories,
        isLoading: categoriesLoading,
        isError: isErrorCategories,
    } = useGetCategories({ enabled: isSheetOpen, defaultRole: user?.defaultRole as UserTypes })
    const { mutateAsync, isPending, isSuccess, data, reset } = useSubmitTicket()
    const getUserMobile = user?.mobile === '' ? '966534798594' : user?.mobile

    const mobileStartsWithFive = useMemo(() => {
        const indexOfFive = typeof getUserMobile === 'string' ? getUserMobile.indexOf('5') : -1
        const mobileStartsFromFive = getUserMobile && indexOfFive !== -1 ? getUserMobile.slice(indexOfFive) : ''

        return mobileStartsFromFive
    }, [getUserMobile])

    const {
        mutateAsync: uploadAttachment,
        isPending: attachmentIsPending,
        isSuccess: isSuccessAttachment,
        reset: resetAttachment,
    } = useUploadAttachment({ caseId: data?.data?.caseId ?? '' })

    const form = useForm<createTicketSchemaType>({
        resolver: zodResolver(createTicketSchema),
        mode: 'onSubmit',
        defaultValues: {
            id: user?.id,
            customerEmail: user?.email ?? 'b@gmil.com',
            //customerName: `${user?.firstName} ${user?.lastName}`,
            customerName: user?.fullName,
            mobile: mobileStartsWithFive,
            category: '',
            description: '',
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
        isError: isErrorSubCategories,
    } = useGetSubCategories({
        categoryId: form?.watch('category'),
        defaultRole: user?.defaultRole as UserTypes,
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
                .then(() => {})
                .catch(() => {
                    toast({
                        //   title: err?.data?.errors?.[0]?.message,
                        variant: 'destructive',
                    })
                })
        } catch (error) {
            console.log(error)
        }
    }

    const onUploadAttachment = async () => {
        if (isError) return
        if (file) {
            try {
                await toBase64(file).then(async base64 => {
                    await uploadAttachment({
                        file: (base64 as string)?.split('base64,')[1],
                        fileName: file?.name,
                        source: 'unified_portal',
                    })
                        .then(() => {
                            setFile(null)
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

    const onSkip = () => {
        setIsSheetOpen(false)
        reset()
        form.reset({ ...form.formState.defaultValues })
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
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col">
                <Sheet
                    open={isSheetOpen}
                    onOpenChange={isOpen => {
                        if (!isOpen && !!(state?.ServiceCategory || state?.ServiceSubCategory)) {
                            if (state?.location && !isSuccess && !isSuccessAttachment) {
                                navigate(-1)
                            } else {
                                navigate(location.pathname, { state: {} })
                            }
                        }
                        setIsSheetOpen(isOpen)
                        if (!isOpen) {
                            setFile(null)
                            reset()
                            resetAttachment()
                            form.reset({ ...form.formState.defaultValues })
                        }
                    }}>
                    <SheetTrigger asChild>
                        <Button
                            size="sm"
                            onClick={() => setIsSheetOpen(true)}
                            type="button"
                            colors="primary"
                            variant={isOutlined ? 'outline' : 'default'}
                            className="flex items-center gap-space-01 whitespace-nowrap">
                            <AddCircle className="size-[20px]" />
                            {strings.support.newTicket}
                        </Button>
                    </SheetTrigger>
                    <SheetContent ignoreInteractOutside>
                        <SheetHeader>
                            <SheetTitle>{isSuccess ? strings.support.addAttachment : strings.support.newTicket}</SheetTitle>
                        </SheetHeader>

                        <If condition={isErrorCategories || isErrorSubCategories}>
                            <Then>
                                <NotAvailableSupport hasCategories={!!state?.ServiceCategory || !!state?.ServiceSubCategory} />
                            </Then>
                            <Else>
                                <SheetBody>
                                    {!isSuccess && (
                                        <div className="flex flex-col gap-space-05">
                                            <div className="flex flex-col gap-space-04">
                                                <h1 className="text-subtitle-01 font-bold text-foreground">
                                                    {strings.support.chooseServiceThatYouNeedHelp}
                                                </h1>
                                                <div className="flex flex-col gap-space-05 sm:flex-row">
                                                    <FormField
                                                        control={form.control}
                                                        name={'category'}
                                                        render={({ field }) => (
                                                            <FormItem className="flex-1">
                                                                <FormLabel>
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
                                                                <FormLabel>
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
                                                                />

                                                                <FormMessage />
                                                            </FormItem>
                                                        )}
                                                    />
                                                </div>
                                            </div>
                                            <div className="flex flex-col gap-space-04">
                                                <h1 className="text-subtitle-01 font-bold text-foreground">{strings.support.howCanWeHelp}</h1>
                                                <FormField
                                                    control={form.control}
                                                    name={'shortDescription'}
                                                    render={({ field }) => (
                                                        <FormItem className="flex-1">
                                                            <FormLabel>
                                                                {strings.support.issueTitle} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Input
                                                                    placeholder={`${strings.shared.enter} ${strings.support.issueTitle.toLowerCase()}`}
                                                                    variant="outline"
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
                                                            <FormLabel>
                                                                {strings.shared.description} <span className="text-error">*</span>
                                                            </FormLabel>
                                                            <FormControl>
                                                                <Textarea
                                                                    placeholder={`${strings.shared.enter} ${strings.support.ticketDescription.toLowerCase()}`}
                                                                    variant="outline"
                                                                    {...field}
                                                                    maxLength={300}
                                                                />
                                                            </FormControl>
                                                            <FormMessage />
                                                        </FormItem>
                                                    )}
                                                />
                                            </div>
                                        </div>
                                    )}
                                    {isSuccess && (
                                        <AddAttachments
                                            CaseId={data?.data?.caseId ?? ''}
                                            setFile={setFile}
                                            setIsError={setIsError}
                                            isSuccess={isSuccessAttachment}
                                        />
                                    )}
                                </SheetBody>
                                <Unless condition={isSuccessAttachment}>
                                    <SheetFooter>
                                        <If condition={isSuccess}>
                                            <Then>
                                                <div className="flex w-full items-center justify-center gap-space-02">
                                                    <Button colors="gray" className="flex-1" type="submit" variant="outline" onClick={onSkip}>
                                                        {strings.shared.skip}
                                                    </Button>
                                                    <Button
                                                        colors="primary"
                                                        className="flex-1"
                                                        type="submit"
                                                        onClick={onUploadAttachment}
                                                        disabled={attachmentIsPending || !file}
                                                        isLoading={attachmentIsPending}>
                                                        {strings.attachments.applyFile}
                                                    </Button>
                                                </div>
                                            </Then>
                                            <Else>
                                                <Button
                                                    colors="primary"
                                                    className="w-full"
                                                    type="submit"
                                                    onClick={form.handleSubmit(onSubmit)}
                                                    disabled={isPending}
                                                    isLoading={isPending}>
                                                    {strings.support.applyTicket}
                                                </Button>
                                            </Else>
                                        </If>
                                    </SheetFooter>
                                </Unless>
                            </Else>
                        </If>
                    </SheetContent>
                </Sheet>
            </form>
        </Form>
    )
}

export default Create
