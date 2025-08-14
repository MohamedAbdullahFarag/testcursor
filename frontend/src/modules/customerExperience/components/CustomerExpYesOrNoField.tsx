import { strings } from '@/shared/locales'
import {
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    ToggleGroup,
    ToggleGroupItem,
    useFormContext,
    useLanguage,
} from 'mada-design-system'
import { Answer, Question } from '../models/interfaces/CustomerExperience.types'

type CustomerExpYesOrNoFieldProps = {
    question: Question
    answers: Answer[]
    setSatisfied?: React.Dispatch<React.SetStateAction<'unSatisfied' | 'satisfied' | undefined>>
}

const CustomerExpYesOrNoField = ({ question, answers, setSatisfied }: CustomerExpYesOrNoFieldProps) => {
    const { control } = useFormContext()
    const { dir } = useLanguage()

    return (
        <FormField
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
                <FormItem className="col-span-full flex items-center gap-space-03">
                    <FormLabel className="!mb-space-00 text-body-02 font-semibold">{question.text.replace(/(<([^>]+)>)/gi, '')}</FormLabel>
                    <FormControl>
                        <ToggleGroup
                            type="single"
                            dir={dir}
                            onValueChange={value => {
                                if (!value || !setSatisfied) return
                                field.onChange(value)
                                if (answers?.find(ans => ans?.id === value)?.text === '10') {
                                    setSatisfied('satisfied')
                                } else {
                                    setSatisfied('unSatisfied')
                                }
                            }}
                            value={field.value}
                            className="flex-wrap justify-start gap-space-02">
                            {[...answers]
                                ?.sort((a, b) => a.displayOrder - b.displayOrder)
                                .map(ans => (
                                    <ToggleGroupItem key={ans.id} value={ans.id} aria-label="Toggle bold" size="default" variant="outline">
                                        {ans.text === '10' ? strings.customerExperience.useful : strings.customerExperience.notUseful}
                                    </ToggleGroupItem>
                                ))}
                        </ToggleGroup>
                    </FormControl>
                    <FormMessage />
                </FormItem>
            )}
        />
    )
}

export default CustomerExpYesOrNoField
