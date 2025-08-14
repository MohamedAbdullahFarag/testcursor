import { FormField, Input, useFormContext } from 'mada-design-system'
import { Question } from '../models/interfaces/CustomerExperience.types'

type HiddenSourceFieldProps = { question: Question; defaultValue: string }

const HiddenSourceField = ({ question, defaultValue }: HiddenSourceFieldProps) => {
    const { control } = useFormContext()

    return (
        <FormField
            defaultValue={defaultValue}
            key={question.id}
            control={control}
            name={question.id}
            render={({ field }) => <Input className="hidden" {...field} />}
        />
    )
}

export default HiddenSourceField
