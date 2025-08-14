import { strings } from '@/shared/locales'
import { FormControl, FormField, FormItem, FormLabel, FormMessage, useFormContext } from 'mada-design-system'
import { AnswerType, Question } from '../models/interfaces/CustomerExperience.types'
// import data from './data'
import { Input, Textarea } from 'mada-design-system'
import { useEffect, useRef } from 'react'

type CustomerExpInputFieldProps = {
    question: Question
    answerId: string
    answerTypeId: string
    answerTypes: { answerType: AnswerType }[]
    shouldHideQuestion: (question: Question) => boolean
}

const CustomerExpInputField = ({ question, answerTypeId, answerTypes, shouldHideQuestion }: CustomerExpInputFieldProps) => {
    const answerType = answerTypes?.find(an => an.answerType.id === answerTypeId)
    const { control, unregister } = useFormContext()
    const wasHidden = useRef(false)
    const isHidden = shouldHideQuestion(question)

    useEffect(() => {
        if (!wasHidden.current && isHidden) {
            unregister(question.id)
        }
        wasHidden.current = isHidden
    }, [isHidden, question.id, unregister])

    if (shouldHideQuestion?.(question)) return null

    return (
        <FormField
            defaultValue=""
            key={question.id}
            control={control}
            rules={{
                required: {
                    value: !!question?.minSelectionRequired,
                    message: `${strings.formatString(
                        strings.sharedValidation.requiredThe,
                        question.text.replace(/(<([^>]+)>)/gi, '').toLowerCase(),
                    )}`,
                },
                maxLength: { value: 120, message: strings.sharedValidation.exceedsTheAllowedLimit },
            }}
            name={question.id}
            render={({ field }) => (
                <FormItem className="col-span-full lg:col-span-1">
                    <FormLabel className="mb-space-03 text-body-02 font-semibold text-card-foreground">
                        {question.text.replace(/(<([^>]+)>)/gi, '')}
                        {!question?.minSelectionRequired && <span className="font-normal">({strings.customerExperience.optional})</span>}
                    </FormLabel>
                    <FormControl>
                        {answerType?.answerType.typeCSS.includes('ngs-question__answer-input-large') ? (
                            <Textarea placeholder={strings.customerExperience.textAreaPlaceholder} {...field} />
                        ) : (
                            <Input {...field} />
                        )}
                    </FormControl>

                    <FormMessage />
                </FormItem>
            )}
        />
    )
}

export default CustomerExpInputField
