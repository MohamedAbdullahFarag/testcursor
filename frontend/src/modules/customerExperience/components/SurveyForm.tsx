/* eslint-disable react-hooks/exhaustive-deps */
import { useRecaptcha } from '@/shared/hooks'
import { useGetQuestions, useSubmitSurvey } from '../services'

import { useAuthStore } from '@/modules/auth/store/authStore'
import { CaptchaErrorAlert } from '@/shared/components'
import { pathNames } from '@/shared/constants'
import { strings } from '@/shared/locales'
import classNames from 'classnames'
import { ThumbUpOffAlt } from 'google-material-icons/filled'
import { Close } from 'google-material-icons/outlined'
import { ActionLoader, Button, Form, Link, Skeleton, Stack, useForm, useLanguage } from 'mada-design-system'
import React, { ReactNode, useEffect, useState } from 'react'
import { Else, If, Then, Unless, When } from 'react-if'
import { Link as LinkReact } from 'react-router-dom'
import { useStep } from 'usehooks-ts'
import { v4 as uuidv4 } from 'uuid'
import { processAnswers, sortQuestionsWithAnswers } from '../lib/utils'
import { Answer, Question, ServeyPayload } from '../models/interfaces/CustomerExperience.types'
import CustomerExpInputField from './CustomerExpInputField'
import CustomerExpSelectField from './CustomerExpSelectField'
import CustomerExpYesOrNoField from './CustomerExpYesOrNoField'
import EmojiField from './EmojiField'
import HiddenSourceField from './HiddenSource'
type CustomerExperienceProps = {
    surveyId: string
    actionPage?: string
    headerDismiss?: boolean
    hideCancel?: boolean
    onDismiss?: () => void
    onSuccess?: () => void
    customMessage?: ReactNode
    onReady?: () => void
    disableFirstStepActions?: boolean
    version: number
    isFirstRoundSubmit?: boolean
    isContent?: boolean
    setIsOpen?: React.Dispatch<React.SetStateAction<boolean>>
}

const SurveyForm = ({
    surveyId,
    onSuccess,
    onDismiss,
    actionPage = '',
    headerDismiss,
    customMessage,
    onReady,
    disableFirstStepActions,
    hideCancel,
    version,
    isFirstRoundSubmit = true,
    isContent,
    setIsOpen,
}: CustomerExperienceProps) => {
    const { user, isAuthenticated } = useAuthStore()
    const currentDate = new Date()
    const [isSubmitting, setIsSubmitting] = useState(false)
    // const { user } = useAuth()
    const { lang } = useLanguage()

    const lng = lang === 'en' ? 'en-US' : 'ar-SA'
    const form = useForm({ mode: 'all' })
    const { data, paginatedData, totalPages, isLoading, isError } = useGetQuestions(surveyId, lng)
    const [step, { goToNextStep, canGoToNextStep, canGoToPrevStep, goToPrevStep }] = useStep(totalPages)
    const [showSuccess, setShowSuccess] = useState(false)
    const respondantId = `${uuidv4()}`
    const [isSatisfied, setIsSatisfied] = useState<'unSatisfied' | 'satisfied'>()
    const sortedQuestionsWithAnswers = sortQuestionsWithAnswers(
        paginatedData[step - 1]?.questions as Question[],
        paginatedData[step - 1]?.answers as Answer[],
    )
    const { getToken, isRecaptchaPending } = useRecaptcha()
    const { mutateAsync, error: submitError } = useSubmitSurvey(surveyId, !!isAuthenticated, version, actionPage)
    console.log(submitError)
    // effects
    useEffect(() => {
        if (!isLoading && !isError && data?.questions?.length) onReady?.()
        return () => {}
    }, [isLoading, isError, data])

    useEffect(() => {
        if (form.formState.isValid && form.formState.isDirty && step === 1 && disableFirstStepActions) goToNextStep()
        return () => {}
    }, [form?.formState?.isValid, form.formState.isDirty, step])

    useEffect(() => {
        if (isError && onDismiss) onDismiss()
    }, [isError, onDismiss])

    useEffect(() => {
        if (isSatisfied && setIsOpen) setIsOpen(true)
    }, [isSatisfied])

    // utils
    const renderQuestionAswer = ({ question, answers }: { question: Question; answers: Answer[] }) => {
        const shouldHideQuestion = (question: Question) => {
            // If question has no satisfaction criteria, always show it
            if (!question.isSatisfied) return false

            // If user hasn't selected satisfied/unsatisfied yet, hide questions with satisfaction criteria
            if (!isSatisfied) return true

            // If user is satisfied, show the question only if it's configured to be shown for satisfied users
            if (isSatisfied === 'satisfied') {
                return !question.isSatisfied.showSatisfied
            }

            // If user is unsatisfied, show the question only if it's configured to be shown for unsatisfied users
            if (isSatisfied === 'unSatisfied') {
                return !question.isSatisfied.showUnsatisfied
            }

            return false
        }
        switch (question.typeCSS) {
            case 'ngs-question-hidden':
                if (question?.text?.includes('version')) return <HiddenSourceField question={question} defaultValue={version?.toString()} />
                if (question?.text?.includes('nationalId')) return <HiddenSourceField question={question} defaultValue={user?.id?.toString() ?? ''} />
                // if (question?.text?.includes('role')) return <HiddenSourceField question={question} defaultValue={user?.defaultRole ?? ''} />
                if (question?.text?.includes('environment')) return <HiddenSourceField question={question} defaultValue={import.meta.env.MODE} />
                return <HiddenSourceField question={question} defaultValue={actionPage} />
            case 'ngs-question-single':
                return answers?.length > 1 ? (
                    <CustomerExpSelectField question={question} answers={answers} type="single" shouldHideQuestion={shouldHideQuestion} />
                ) : (
                    <CustomerExpInputField
                        shouldHideQuestion={shouldHideQuestion}
                        question={question}
                        answerId={answers[0].id}
                        answerTypeId={answers[0].answerTypeId}
                        answerTypes={data?.answerTypes}
                    />
                )
            case 'ngs-question-multi':
                return <CustomerExpSelectField question={question} answers={answers} type="multiple" shouldHideQuestion={shouldHideQuestion} />
            case 'ngs-question-list':
                return <CustomerExpSelectField question={question} answers={answers} type="multiple" shouldHideQuestion={shouldHideQuestion} />
            case 'ngs-question-csat-smiley':
                return <EmojiField question={question} answers={answers} />
            case 'ngs-question-csat-image':
                return <CustomerExpYesOrNoField question={question} answers={answers} setSatisfied={setIsSatisfied} />
        }
    }

    async function onSubmit(formData: Record<string, string | string[]>) {
        const token = await getToken()

        if (totalPages > 1 && canGoToNextStep) {
            goToNextStep()
            return
        }

        const answers = processAnswers(respondantId, formData, data?.questions, data?.answers)

        const payload = {
            respondent: {
                respondent: {
                    id: respondantId,
                    surveyId: paginatedData[0]?.surveyId,
                    resumeUId: null,
                    startDate: `${new Date(currentDate).toISOString()}`,
                    iPSource: null,
                    changeUID: null,
                    languageCode: lng,
                    validated: false,
                    platform: navigator.userAgent,
                    userAgent: navigator.userAgent,
                    mobileOS: false,
                    platformOS: 2,
                    resumeQuestionId: sortedQuestionsWithAnswers[0]?.question?.id,
                    resumePageId: paginatedData[0]?.id,
                    timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
                },
                answers: answers,
                dataAttributes: null,
                files: [],
                poolQuestions: [],
                querystring: `?lang=${lng}`,
            },
            Captcha: {
                token,
            },
            securityAuthorization: [],
        }

        setIsSubmitting(true)
        mutateAsync(payload as ServeyPayload)
            .then(() => {
                if (isSatisfied) {
                    sessionStorage.setItem(`contentSureveySuccess-${actionPage}`, 'true')
                }
                setIsSubmitting(false)
                setShowSuccess(true)
                onSuccess?.()
            })
            .catch(error => {
                setIsSubmitting(false)
                console.log(error)
                // if ((error as unknown as APIDTO.ErrorBE)?.data?.Errors?.[0]?.Code !== ErrorCodes.CaptchaError) {
                setShowSuccess(true)
                onSuccess?.()
                // }
            })
    }

    const handleDismiss = () => {
        setIsSubmitting(false)
        setShowSuccess(false)
        onDismiss?.()
        form.reset()
    }

    if (isLoading && isContent) return <Skeleton className="h-space-06 w-[300px]" />
    if (isLoading) return <Loader />

    return (
        <If condition={showSuccess}>
            <Then>
                {customMessage ?? (
                    <>
                        <When condition={headerDismiss}>
                            <div className="flex items-center justify-between">
                                <h2 className="title-01 font-bold">{strings.customerExperience.title}</h2>
                                <Button size="icon" className="h-auto leading-[1]" variant="text" colors="neutral" onClick={handleDismiss}>
                                    <Close />
                                </Button>
                            </div>
                        </When>
                        <div className="flex flex-col items-center justify-center gap-space-05">
                            <div className="flex h-[88px] w-[88px] items-center justify-center rounded-full bg-primary-container text-primary-oncontainer">
                                <ThumbUpOffAlt width={40} height={40} />
                            </div>
                            <p className="title-01 font-semibold">{strings.customerExperience.successMessage}</p>
                        </div>
                    </>
                )}
            </Then>
            <Else>
                <Form {...form}>
                    <form
                        onSubmit={form.handleSubmit(onSubmit)}
                        className={classNames({
                            'flex flex-col gap-space-04 sm:gap-space-05': true,
                            'w-full': isContent,
                        })}>
                        <div
                            className={classNames({
                                'grid w-full grid-cols-2 gap-y-space-06': isContent,
                                'flex flex-col gap-space-04 sm:gap-space-05': !isContent,
                            })}>
                            {sortedQuestionsWithAnswers?.map(q => {
                                return (
                                    <React.Fragment key={q.question.id}>
                                        {renderQuestionAswer({ question: q.question, answers: q.answers })}
                                    </React.Fragment>
                                )
                            })}{' '}
                        </div>
                        <When
                            condition={
                                !isAuthenticated && !isRecaptchaPending
                                //  &&
                                // (submitError as unknown as { data: { Errors: { Code: string }[] } })?.data?.Errors?.[0]?.Code ===
                                //     ErrorCodes.CaptchaError
                            }>
                            <CaptchaErrorAlert />
                        </When>
                        <When
                            condition={
                                ((!disableFirstStepActions && totalPages > 1 && canGoToNextStep) || canGoToPrevStep || !canGoToNextStep) &&
                                (isFirstRoundSubmit || isSatisfied)
                            }>
                            <div className="flex gap-space-02">
                                <When condition={!disableFirstStepActions && totalPages > 1 && canGoToNextStep}>
                                    <>
                                        <Unless condition={hideCancel}>
                                            <Button type="button" colors={'primary'} variant="outline" onClick={handleDismiss}>
                                                {strings.shared.cancel}
                                            </Button>
                                        </Unless>
                                        <Button colors={'primary'} type="submit" disabled={!form.formState.isDirty || !form.formState.isValid}>
                                            {strings.shared.next}
                                        </Button>
                                    </>
                                </When>
                                <Stack justifyContent={'start'} alignItems={'start'} className="w-full">
                                    <When condition={canGoToPrevStep}>
                                        <Button
                                            type="button"
                                            colors={'primary'}
                                            variant="outline"
                                            onClick={() => {
                                                goToPrevStep()
                                                // temp soluation
                                                form.reset()
                                            }}>
                                            {strings.shared.previous}
                                        </Button>
                                    </When>
                                    <Unless condition={canGoToNextStep}>
                                        <div className="flex w-full flex-col items-start justify-between gap-space-05 sm:flex-row sm:items-center sm:gap-space-00">
                                            <When condition={isContent}>
                                                <div className="flex items-center gap-space-03">
                                                    <span className="text-body-02 text-text-default">{strings.customerExperience.forMoreInfo}</span>
                                                    <Link colors="primary" underline={'always'} asChild>
                                                        <LinkReact to={pathNames.eParticipation}>{strings.eParticipation.title}</LinkReact>
                                                    </Link>
                                                </div>
                                            </When>
                                            <Button colors={'primary'} rounded="default" type="submit" disabled={isSubmitting || isRecaptchaPending}>
                                                {strings.customerExperience.sendFeedback}
                                                {(isSubmitting || isRecaptchaPending) && <ActionLoader />}
                                            </Button>
                                        </div>
                                    </Unless>
                                </Stack>
                            </div>
                        </When>
                    </form>
                </Form>
            </Else>
        </If>
    )
}

export default SurveyForm

const Loader = () => {
    return (
        <div className="flex flex-col gap-space-03">
            <Skeleton className="h-[30px] w-3/4" />
            <Skeleton className="h-[30px] w-1/2" />
            <Skeleton className="h-[30px] w-3/4" />
            <Skeleton className="h-[30px] w-1/3" />
            <Skeleton className="h-[30px] w-1/4" />
            <Skeleton className="h-[30px] w-3/4" />
        </div>
    )
}
