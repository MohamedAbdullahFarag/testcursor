import { CheckCircle, HighlightOff } from 'google-material-icons/outlined'
import { Button, Skeleton, Stack, cn } from 'mada-design-system'
import { useState } from 'react'
import { Else, If, Then, When } from 'react-if'

import { useIsAuthLayout } from '@/shared/hooks'
import { strings } from '@/shared/locales'
import SurveyForm from '../components/SurveyForm'
import { useGetServiceRating } from '../services'

const Msg = () => (
    <div className="flex items-center gap-space-05">
        <CheckCircle className="size-space-05 text-icon-primary" />
        <p className="text-body-02 text-text-default">{strings.customerExperience.surveyThanksMsg}</p>
    </div>
)
const ContentSurvey = ({ actionPage = '', version = 1 }: { actionPage?: string; version?: number }) => {
    const isAuthLayout = useIsAuthLayout()
    const [isOpen, setIsOpen] = useState(false)
    const [resetKey, setResetKey] = useState(0)
    const {
        data: ratingData,
        isPending,
        isError,
    } = useGetServiceRating(import.meta.env.VITE_CONTENT_EVALUATION_REPORT_ID, version, import.meta.env.MODE, actionPage)
    const numberOfVisitors = ratingData?.reportItems?.[0]?.chartsData?.[0]?.answerData?.[0]?.answersTotal
    const [isSuccess] = useState(!!sessionStorage.getItem(`contentSureveySuccess-${actionPage}`))

    const handleCancel = () => {
        setIsOpen(false)
        setResetKey(prev => prev + 1)
    }

    if ((isError || isPending) && !isSuccess) return null

    return (
        <div className="border-t-2 border-border-primary-default px-space-04 py-space-05 md:px-space-06">
            <div className={cn('flex flex-col gap-space-05', { 'mx-auto max-w-container': !isAuthLayout })}>
                <Stack
                    justifyContent="between"
                    className={cn('flex-col items-start sm:flex-row', {
                        '!px-space-03': isAuthLayout,
                    })}>
                    <If condition={isSuccess}>
                        <Then>
                            <Msg />
                        </Then>
                        <Else>
                            <Stack
                                gap={3}
                                className={cn({
                                    'w-full flex-col items-start sm:flex-row sm:items-center': true,
                                    'order-2 sm:order-1': isOpen,
                                })}>
                                <SurveyForm
                                    key={`${actionPage}-${resetKey}`} // Use the resetKey to force remount
                                    onSuccess={() => {
                                        setIsOpen(false)
                                    }}
                                    setIsOpen={setIsOpen}
                                    isContent
                                    surveyId={import.meta.env.VITE_CONTENT_EVALUATION}
                                    actionPage={actionPage}
                                    customMessage={<Msg />}
                                    hideCancel
                                    version={version}
                                    isFirstRoundSubmit={false}
                                />
                            </Stack>
                        </Else>
                    </If>
                    <If condition={isOpen}>
                        <Then>
                            <Button
                                variant="text"
                                colors="neutral"
                                size="default"
                                onClick={handleCancel}
                                className="order-1 self-end sm:order-2 sm:self-stretch">
                                {strings.shared.cancel}
                                <HighlightOff />
                            </Button>
                        </Then>
                        <Else>
                            <If condition={isPending}>
                                <Then>
                                    <Skeleton className="h-space-05 w-[200px]" />
                                </Then>
                                <Else>
                                    <When condition={!numberOfVisitors}>
                                        <p className="shrink-0 text-body-01 text-text-default">{strings.customerExperience.beTheFirstOneWhoRates}</p>
                                    </When>
                                    <When condition={!!numberOfVisitors}>
                                        <p className="shrink-0 text-body-01 text-text-default">
                                            {strings.formatString(strings.customerExperience.contentUseful, numberOfVisitors ?? '')}
                                        </p>
                                    </When>
                                </Else>
                            </If>
                        </Else>
                    </If>
                </Stack>
            </div>
        </div>
    )
}

export default ContentSurvey
