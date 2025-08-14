import { Button, useLanguage } from 'mada-design-system'

import PortalFooterPage from '@/shared/components/PortalFooterPage'
import PortalHeaderPage from '@/shared/components/PortalHeaderPage'
import { strings } from '@/shared/locales'
import { cn } from 'mada-design-system'

const EParticipation = () => {
    const { lang } = useLanguage()
    //  const checkLanguage = lang === 'ar' ? '18/03/2025 - 11:00 ص' : '18/03/2025, 11:00 AM'

    return (
        <div>
            <PortalHeaderPage titlePage={strings.eParticipation.title} />
            <div className="px-space-04 md:px-space-06">
                <div className="mx-auto flex max-w-container flex-col gap-space-08 py-space-07">
                    <div className="flex flex-col gap-space-04">
                        <EParticipationText text={strings.eParticipation.introduction} />
                        <EParticipationText text={strings.eParticipation.participationOpportunities} />
                        <EParticipationText text={strings.eParticipation.eGovernmentInitiatives} />
                        <EParticipationText text={strings.eParticipation.digitalEngagementTools} />
                    </div>

                    <div className="flex flex-col gap-space-04 rounded-3 border border-border-primary bg-background-card p-space-05 shadow-01">
                        <h1 className={cn('title-01 text-right font-bold text-text-default', { 'text-pretty': lang === 'ar' })}>
                            {strings.eParticipation.subTitle}
                        </h1>{' '}
                        <EParticipationText text={strings.eParticipation.details} />
                        <div className="flex items-center gap-space-05">
                            <p className="shrink-0 text-nowrap text-subtitle-01 font-semibold text-text-default">{strings.shared.email}:</p>
                            <Button variant="default" colors="neutral" rounded="default" className="bg-button-background-black">
                                <a href="mailto:media@moe.gov.sa" className="text-body-02 font-medium text-text-oncolor-primary">
                                    media@moe.gov.sa
                                </a>
                            </Button>
                        </div>
                    </div>
                    <div>
                        <PortalFooterPage modifiedUtc={'2025-01-20T06:59:26.6048026Z'} actionPage="eparticipation" />
                    </div>
                </div>
            </div>
        </div>
    )
}

export default EParticipation

const EParticipationText = ({ text }: { text: string }) => {
    return <p className="text-body-02 text-text-default">{text}</p>
}
