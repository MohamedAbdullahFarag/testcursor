import {
    Emoji,
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
import { When } from 'react-if'
import { Answer, Question } from '../models/interfaces/CustomerExperience.types'
import { strings } from '@/shared/locales'

type EmojiFieldProps = { question: Question; answers: Answer[] }

const getEmoName = (rate: number) => {
    switch (rate) {
        case 1:
            return 'awful'
        case 2:
            return 'bad'
        case 3:
            return 'ok'
        case 4:
            return 'good'
        case 5:
            return 'great'

        default:
            return ''
    }
}
const EmojiField = ({ question, answers }: EmojiFieldProps) => {
    const { control } = useFormContext()
    const { dir } = useLanguage()
    const sortedAns = [...answers]?.sort((a, b) => a.displayOrder - b.displayOrder)
    const startText = sortedAns?.slice(0, 1).shift()?.text
    const endText = sortedAns?.slice(-1).pop()?.text
    return (
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
                <FormItem>
                    <FormLabel className="mb-space-03 text-body-02 font-semibold">{question.text.replace(/(<([^>]+)>)/gi, '')}</FormLabel>
                    <FormControl className="mb-space-03">
                        <>
                            <ToggleGroup
                                dir={dir}
                                onValueChange={field.onChange}
                                defaultValue={field.value}
                                type="single"
                                className="items-start justify-start gap-space-02 max-[380px]:gap-space-01">
                                {sortedAns.map(ans => (
                                    <ToggleGroupItem
                                        key={ans.id}
                                        size="fit"
                                        value={ans.id}
                                        aria-label="Toggle bold"
                                        className="flex !h-auto flex-col items-center justify-center gap-space-02 !bg-transparent !p-0">
                                        <Emoji
                                            variant={getEmoName(ans?.displayOrder) as 'awful' | 'bad' | 'ok' | 'good' | 'great' | null | undefined}
                                        />
                                    </ToggleGroupItem>
                                ))}
                            </ToggleGroup>
                            <When condition={!!startText && !!endText}>
                                <div className="mt-space-02 flex max-w-[312px] justify-between">
                                    <span className="w-[60px] text-wrap text-center text-caption-01 text-foreground-secondary">{startText}</span>
                                    <span className="w-[60px] text-wrap text-center text-caption-01 text-foreground-secondary">{endText}</span>
                                </div>
                            </When>
                        </>
                    </FormControl>
                    <FormMessage />
                </FormItem>
            )}
        />
    )
}

export default EmojiField
