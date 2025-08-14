import { StreamLineIcon } from '@/shared/components'
import { HelpGray } from 'streamline-icons'

import { strings } from '@/shared/locales'

import { Call, CopyAll, Mail } from 'google-material-icons/outlined'
import { Button, SheetBody, toast } from 'mada-design-system'
import { Unless } from 'react-if'
import { supportConstants } from '../models/constants'

interface NotAvailableSupportProps {
    hasCategories?: boolean
    hasNoContactInfo?: boolean
}

const NotAvailableSupport = ({ hasCategories, hasNoContactInfo }: NotAvailableSupportProps) => {
    const shareLink = (text: string) => {
        navigator.clipboard.writeText(text).then(() => {
            toast({
                description: strings.shared.copiedSuccessfully,
            })
        })
    }
    return (
        <SheetBody className="flex flex-col gap-space-05">
            <div className="flex flex-1 flex-col items-center justify-end gap-space-05">
                <StreamLineIcon light={HelpGray} dark={HelpGray} />
                <div className="flex flex-col items-center justify-center gap-space-01">
                    <h2 className="text-body-02 font-semibold text-card-foreground">
                        {hasNoContactInfo ? strings.support.updateContactInfo : strings.support.supportNotAvailable}
                    </h2>
                    <p className="text-center text-body-01 text-foreground-secondary">
                        {hasNoContactInfo ? strings.support.useOtherOptions : strings.support.supportNotAvailableDesc}
                    </p>
                </div>
            </div>
            <div className="flex flex-1 flex-col gap-space-04">
                <Unless condition={hasCategories}>
                    <div className="flex items-center gap-space-02 rounded-2 border p-space-03">
                        <Mail className="size-space-05 text-primary" />
                        <div className="flex grow flex-col gap-space-01">
                            <h2 className="text-body-01 font-semibold text-card-foreground">{strings.shared.email}</h2>
                            <span className="text-body-02 text-primary">{supportConstants.Email}</span>
                        </div>
                        <Button
                            type="button"
                            tooltip={strings.shared.copy}
                            variant="ghost"
                            colors="primary"
                            size="icon-sm"
                            onClick={() => shareLink(supportConstants.Email)}>
                            <CopyAll className="size-[20px]" />
                        </Button>
                    </div>
                    <div className="flex items-center gap-space-02 rounded-2 border p-space-03">
                        <Call className="size-space-05 text-primary" />
                        <div className="flex grow flex-col gap-space-01">
                            <h2 className="text-body-01 font-semibold text-card-foreground">{strings.shared.phone}</h2>
                            <span className="text-body-02 text-primary">{supportConstants.Phone}</span>
                        </div>
                        <Button
                            type="button"
                            tooltip={strings.shared.copy}
                            variant="ghost"
                            colors="primary"
                            size="icon-sm"
                            onClick={() => shareLink(supportConstants.Phone)}>
                            <CopyAll className="size-[20px]" />
                        </Button>
                    </div>
                </Unless>
                {/* <div className="flex items-center gap-space-02 rounded-2 border p-space-03">
                    <HelpOutline className="size-space-05 text-primary" />
                    <div className="flex grow flex-col gap-space-01">
                        <h2 className="text-body-01 font-semibold text-card-foreground">{strings.faq.title}</h2>
                        <span className="text-body-02 text-primary">{strings.support.exploreFAQ}</span>
                    </div>
                    <Button variant="ghost" colors="primary" size="icon-sm" asChild>
                        <Link to="/faq">
                            <ChevronLeft className="size-[20px] ltr:rotate-180" />
                        </Link>
                    </Button>
                </div> */}
            </div>
        </SheetBody>
    )
}

export default NotAvailableSupport
