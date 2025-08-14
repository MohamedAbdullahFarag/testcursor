import {
    Checkbox,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    Label,
    RadioGroup,
    RadioGroupItem,
    useFormContext,
    useLanguage,
} from 'mada-design-system'
import { useEffect, useRef, useState } from 'react'
import { Else, If, Then } from 'react-if'
import { Answer, Question } from '../models/interfaces/CustomerExperience.types'
import { strings } from '@/shared/locales'

type CustomerExpSelectFieldProps = {
    question: Question
    answers: Answer[]
    type: 'single' | 'multiple'
    shouldHideQuestion: (question: Question) => boolean
}

const CustomerExpSelectField = ({ question, answers, type, shouldHideQuestion }: CustomerExpSelectFieldProps) => {
    const { dir } = useLanguage()
    const [checkedItems, setCheckedItems] = useState<string[]>([])
    const { control, unregister } = useFormContext()
    const wasHidden = useRef(false)
    const isHidden = shouldHideQuestion(question)

    useEffect(() => {
        if (!wasHidden.current && isHidden) {
            unregister(question.id)
            setCheckedItems([])
        }
        wasHidden.current = isHidden
    }, [isHidden, question.id, unregister])

    if (shouldHideQuestion(question)) return null

    return (
        <If condition={type === 'multiple'}>
            <Then>
                <FormField
                    defaultValue=""
                    key={question.id}
                    control={control}
                    name={question?.id}
                    rules={{
                        required: {
                            value: !!question?.minSelectionRequired,
                            message: `${strings.formatString(
                                strings.sharedValidation.requiredSelectThe,
                                question.text.replace(/(<([^>]+)>)/gi, '').toLowerCase(),
                            )}`,
                        },
                    }}
                    render={({ field }) => (
                        <FormItem className="col-span-1 col-start-1">
                            <FormLabel className="mb-space-03 text-body-02 font-semibold">{question.text.replace(/(<([^>]+)>)/gi, '')}</FormLabel>
                            <FormControl>
                                <div className="flex flex-col gap-space-04">
                                    {[...answers]
                                        ?.sort((a, b) => a.displayOrder - b.displayOrder)
                                        .map(ans => (
                                            <label htmlFor={ans?.id} key={ans.id} aria-label="Toggle bold" className="flex items-center gap-space-04">
                                                <Checkbox
                                                    checked={!!checkedItems?.includes(ans?.id)}
                                                    id={ans?.id}
                                                    onCheckedChange={(isChecked: boolean) => {
                                                        if (isChecked) {
                                                            setCheckedItems(prev => {
                                                                field.onChange([...prev, ans?.id])
                                                                return [...prev, ans?.id]
                                                            })
                                                        } else {
                                                            setCheckedItems(prev => {
                                                                field.onChange(prev?.filter(item => item !== ans?.id))
                                                                return prev?.filter(item => item !== ans?.id)
                                                            })
                                                        }
                                                    }}
                                                />
                                                {ans.text}
                                            </label>
                                        ))}
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
            </Then>
            <Else>
                <FormField
                    defaultValue=""
                    key={question.id}
                    control={control}
                    name={question?.id}
                    rules={{
                        required: {
                            value: !!question?.minSelectionRequired,
                            message: `${strings.formatString(
                                strings.sharedValidation.requiredSelectThe,
                                question.text.replace(/(<([^>]+)>)/gi, '').toLowerCase(),
                            )}`,
                        },
                    }}
                    render={({ field }) => (
                        <FormItem className="col-span-full flex flex-col gap-space-04 sm:flex-row sm:items-center">
                            <FormLabel className="!mb-space-00 text-body-02 font-semibold">{question.text.replace(/(<([^>]+)>)/gi, '')}</FormLabel>
                            <FormControl>
                                <RadioGroup
                                    dir={dir}
                                    onValueChange={field.onChange}
                                    defaultValue={field.value}
                                    className="flex flex-col justify-start gap-space-02 sm:flex-row sm:flex-wrap">
                                    {[...answers]
                                        ?.sort((a, b) => a.displayOrder - b.displayOrder)
                                        .map(ans => (
                                            <div key={ans.id} className="flex items-center gap-x-space-02">
                                                <RadioGroupItem value={ans.id} aria-label="Toggle bold" />
                                                <Label htmlFor="r1">{ans?.text}</Label>
                                            </div>
                                        ))}
                                </RadioGroup>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
            </Else>
        </If>
    )
}

export default CustomerExpSelectField
