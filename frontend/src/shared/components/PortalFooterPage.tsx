import { ContentSurvey } from '@/modules/customerExperience/views'
import { strings } from '@/shared/locales'
import { useLanguage } from 'mada-design-system'
import moment from 'moment'
import { When } from 'react-if'

export default function PortalFooterPage({ modifiedUtc, actionPage }: { modifiedUtc: string; actionPage?: string }) {
    const { lang } = useLanguage()
    return (
        <>
            <div className="flex py-space-04">
                <p className="text-default text-body-01">
                    {strings.shared.lastUpdatedLabel}{' '}
                    {lang === 'ar'
                        ? moment(modifiedUtc).locale(lang).format('D/M/YYYY - h:mm A')
                        : moment(modifiedUtc).locale(lang).format('D/M/YYYY, h:mm A')}{' '}
                    {strings.shared.timeZoneNote}
                </p>
            </div>
            <When condition={actionPage}>
                <ContentSurvey actionPage={actionPage} />
            </When>
        </>
    )
}
