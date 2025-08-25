import { ErrorOutline } from 'google-material-icons/outlined'
import { Alert, AlertDescription } from 'mada-design-system'
import { Else, If, Then } from 'react-if'
import { Link } from 'react-router-dom'
import { pathNames } from '@/shared/constants'
import { strings } from '../locales'

const CaptchaErrorAlert = ({ isSupport }: { isSupport?: boolean }) => {
    return (
        <Alert variant="outline" colors="error">
            <AlertDescription className="flex items-start gap-space-02">
                <ErrorOutline className="size-[20px]" />
                <p className="inline-block text-body-01">
                    <If condition={isSupport}>
                        <Then>{strings.captchaError.captchaError}</Then>
                        <Else>
                            {strings.captchaError.captchaError}{' '}
                            {strings.formatString(
                                strings.captchaError.captchaError2,
                                <Link className="font-semibold underline" to={pathNames.portalSupport}>
                                    {strings.captchaError.askSupportTeam}
                                </Link>,
                            )}
                        </Else>
                    </If>
                </p>
            </AlertDescription>
        </Alert>
    )
}

export default CaptchaErrorAlert
