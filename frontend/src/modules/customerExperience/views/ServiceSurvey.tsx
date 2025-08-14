import { strings } from '@/shared/locales'
import { Close } from 'google-material-icons/outlined'
import { Button } from 'mada-design-system'
import { Unless } from 'react-if'
import SurveyForm from '../components/SurveyForm'

type ServiceSurveyProps = {
    onClose?: () => void
    onReady?: () => void
    surveyId: string
    disableFirstStepActions?: boolean
    hideCancel?: boolean
    noHeaderCancel?: boolean
    onSuccess?: () => void
    version: number
    className?: string
    onDismiss?: () => void
}

const ServiceSurvey = ({ onClose, className, onDismiss, ...rest }: ServiceSurveyProps) => {
    return (
        <div className={className ?? 'max-w-[551px] rounded-4 border bg-card p-space-04 text-card-foreground sm:p-space-05'}>
            <div className="mb-space-04 flex items-center justify-between sm:mb-space-05">
                <h2 className="title-01 font-bold">{strings.customerExperience.title}</h2>
                <Unless condition={rest.noHeaderCancel}>
                    <Button
                        size="icon"
                        className="h-auto leading-[1]"
                        variant="text"
                        colors="neutral"
                        onClick={() => {
                            onClose?.()
                        }}>
                        <Close />
                    </Button>
                </Unless>
            </div>
            <SurveyForm onDismiss={onDismiss ?? onClose} {...rest} />
        </div>
    )
}

export default ServiceSurvey
