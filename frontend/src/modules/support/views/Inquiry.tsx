import { zodResolver } from '@hookform/resolvers/zod'
import { WarningAmber } from 'google-material-icons/outlined'
import {
    Alert,
    AlertDescription,
    Breadcrumbs,
    Button,
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    handleArabicNumbers,
    Input,
    Stack,
    useForm,
    useSwitchData,
} from 'mada-design-system'
import { When } from 'react-if'

import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import { Link, useNavigate } from 'react-router-dom'
import Sidebar from '../components/Sidebar'
import { ticketInquirySchema, ticketInquirySchemaType } from '../schema'
import { useGetCaseByCaseNumAndSourceAndUserId } from '../services/useServiceNow'

const Inquiry = () => {
    const localize = useSwitchData()
    const { mutateAsync, isPending, isError, error } = useGetCaseByCaseNumAndSourceAndUserId()
    const navigate = useNavigate()
    const form = useForm<ticketInquirySchemaType>({
        resolver: zodResolver(ticketInquirySchema),
        mode: 'onSubmit',
        defaultValues: {
            userId: '',
            caseNum: '',
        },
    })

    const onSubmit = async (values: ticketInquirySchemaType) => {
        const submittedData = {
            ...values,
        }
        mutateAsync(submittedData)
            .then(res => {
                navigate(`/support/details`, { state: { caseId: res?.data?.caseId } })
            })
            .catch(error => console.log(error))
    }

    return (
        <Stack gap={7} className="relative" direction={'col'}>
            <PortalHeaderPage
                titlePage={strings.support.ticketInquiry}
                descriptionPage={strings.support.inquiryDesc}
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
                                title: strings.support.ticketInquiry,
                            },
                        ]}
                    />
                }
            />
            <Stack className="px-space-04 xl:px-space-06">
                <Stack
                    gap={6}
                    justifyContent={'between'}
                    alignItems={'start'}
                    className="mx-auto w-full max-w-container"
                    data-testid="ticketCardContainer">
                    <div className="grow pb-space-07" data-testid="inqueryCardContainer">
                        <Form {...form}>
                            <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-space-05">
                                <When condition={isError}>
                                    <Alert colors="warning" className="items-center">
                                        <div className="flex items-center gap-space-02">
                                            <WarningAmber className="size-[20px]" />
                                            <AlertDescription>
                                                {localize(
                                                    (error as unknown as { errors: { code: string; message: string; messageAr: string }[] })
                                                        ?.errors?.[0]?.messageAr,
                                                    (error as unknown as { errors: { code: string; message: string; messageAr: string }[] })
                                                        ?.errors?.[0]?.message,
                                                )}
                                            </AlertDescription>
                                        </div>
                                    </Alert>
                                </When>
                                <h2 className="text-body-02 font-semibold">{strings.support.ticketData}</h2>

                                <Stack>
                                    <FormField
                                        control={form.control}
                                        name={'caseNum'}
                                        render={({ field }) => (
                                            <FormItem className="flex-1">
                                                <FormLabel data-testid="ticketNumberLabelTest">
                                                    {strings.support.ticketNumber} <span className="text-error">*</span>
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        placeholder={`${strings.shared.enter} ${strings.support.ticketNumber.toLowerCase()}`}
                                                        variant={'default'}
                                                        data-testid="ticketNumberInputTest"
                                                        {...field}
                                                        maxLength={10}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name={'userId'}
                                        render={({ field }) => (
                                            <FormItem className="flex-1">
                                                <FormLabel data-testid="enterNationalResidenceIDLabelTest">
                                                    {strings.shared.nationalId} <span className="text-error">*</span>
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        placeholder={`${strings.shared.enter} ${strings.shared.nationalId.toLowerCase()}`}
                                                        variant={'default'}
                                                        {...field}
                                                        data-testid="enterNationalResidenceIdInputTest"
                                                        maxLength={10}
                                                        onChange={e => {
                                                            field.onChange(handleArabicNumbers(e, form.watch('userId')))
                                                        }}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                </Stack>
                                <div className="flex items-center gap-space-02">
                                    <Button
                                        colors="primary"
                                        rounded={'default'}
                                        disabled={isPending}
                                        isLoading={isPending}
                                        data-testid="inquireBtnTest">
                                        {strings.support.inquiry}
                                    </Button>
                                </div>
                            </form>
                        </Form>
                    </div>
                    <Sidebar />
                </Stack>
            </Stack>
        </Stack>
    )
}

export default Inquiry
