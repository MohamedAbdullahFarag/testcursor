import { strings } from '@/shared/locales'
import classNames from 'classnames'
import {
    Accessibility as AccessibilityIcon,
    ChevronLeft,
    Contrast,
    FontDownload,
    HearingDisabled,
    Mic,
    OpenInNew,
    TextDecrease,
    TextIncrease,
} from 'google-material-icons/outlined'
import { accessibilityTools, Button, Popover, PopoverContent, PopoverTrigger, Switch } from 'mada-design-system'
import { ReactNode } from 'react'
const Accessibility = () => {
    const { toggleColors, isActive, increaseSize, decreaseSize, defaultSize } = accessibilityTools(state => state)

    return (
        <Popover>
            <PopoverTrigger asChild data-testid="accessibilityButtonTest">
                <Button  size={"icon-sm"} variant={"text"} colors={"gray"} tooltip={strings.accessibility.accessibility}  aria-label="Accessibility">
                    <AccessibilityIcon />
                </Button>
            </PopoverTrigger>
            <PopoverContent
                align="end"
                className={classNames({
                    'flex w-[36rem] flex-col gap-space-02 border-none bg-background px-space-03 py-space-04': true,
                    grayscale: isActive,
                })}>
                <div className="px-space-03 py-space-02">
                    <h2 className="text-subtitle-02 font-bold text-foreground-secondary">{strings.accessibility.accessibility}</h2>
                </div>
                <div className="flex flex-col gap-space-02 rounded-4 bg-card">
                    <div className="flex flex-col">
                        <div className="flex h-[7.2rem] justify-between gap-space-02 border-b border-b-border p-space-04">
                            <div className="flex items-center gap-space-02">
                                <FontDownload className="text-primary" />
                                <p className="text-body-01 font-bold text-foreground">{strings.accessibility.fontSize}</p>
                            </div>
                            <div className="flex items-center justify-center gap-space-02">
                                <button
                                    className="flex h-16 w-16 items-center justify-center rounded-full border border-border"
                                    onClick={decreaseSize}>
                                    <TextDecrease />
                                </button>
                                <button
                                    className="flex h-16 w-16 items-center justify-center rounded-full border border-border"
                                    onClick={defaultSize}>
                                    <p className="text-[20px]">A</p>
                                </button>
                                <button
                                    className="flex h-16 w-16 items-center justify-center rounded-full border border-border"
                                    onClick={increaseSize}>
                                    <TextIncrease />
                                </button>
                            </div>
                        </div>
                        <div className="flex h-[7.2rem] items-center justify-between gap-space-02 border-b border-b-border p-space-04">
                            <div className="flex items-center gap-space-02">
                                <Contrast className="text-primary" />
                                <p className="text-body-01 font-bold text-foreground">{strings.accessibility.colorContrast}</p>
                            </div>
                            <Switch checked={isActive} onCheckedChange={toggleColors} />
                        </div>
                        <a
                            href="https://deaf.dga.gov.sa"
                            target="_blank"
                            rel="noreferrer"
                            className="flex h-[7.2rem] items-center justify-between gap-space-02 border-b border-b-border p-space-04">
                            <div className="flex items-center gap-space-02">
                                <HearingDisabled />
                                <p className="text-body-01 font-bold">{strings.accessibility.signLanguage}</p>
                            </div>
                            <ChevronLeft className="ltr:rotate-180" />
                        </a>
                        <div className="flex h-[7.2rem] cursor-not-allowed items-center justify-between gap-space-02 p-space-04">
                            <div className="flex items-center gap-space-02">
                                <Mic className="text-disabled" />
                                <p className="text-body-01 font-bold text-disabled">{strings.accessibility.voiceCommands}</p>
                            </div>
                            <ChevronLeft className="text-disabled ltr:rotate-180" />
                        </div>
                    </div>
                </div>
            </PopoverContent>
        </Popover>
    )
}

export const AccessibilityMobile = ({ Close, onBack }: { Close: ReactNode; onBack: () => void }) => {
    const { toggleColors, isActive, increaseSize, decreaseSize, defaultSize } = accessibilityTools(state => state)

    return (
        <div
            className={classNames({
                'flex flex-col gap-space-02 border-none': true,
                grayscale: isActive,
            })}>
            <div className="flex items-center justify-between pr-space-02">
                <button className="flex items-center gap-space-02 py-space-03 text-primary" onClick={onBack}>
                    <ChevronLeft className="h-space-05 w-space-05 rtl:rotate-180" />
                    <span className="text-body-02">{strings.common?.back || 'Back'}</span>
                </button>
                {Close}
            </div>
            <div className="flex-col gap-space-03">
                <h1 className="title-01 px-space-04 py-space-03 font-bold">{strings.accessibility.accessibility}</h1>
                <div className="">
                    <div className="flex justify-between p-space-04">
                        <div className="flex items-center gap-space-02">
                            <FontDownload className="text-primary" />
                            <p className="text-body-01 font-bold text-foreground">{strings.accessibility.fontSize}</p>
                        </div>
                        <div className="flex items-center justify-center gap-space-02">
                            <button className="flex h-16 w-16 items-center justify-center rounded-full border border-border" onClick={decreaseSize}>
                                <TextDecrease />
                            </button>
                            <button className="flex h-16 w-16 items-center justify-center rounded-full border border-border" onClick={defaultSize}>
                                <p className="text-[20px]">A</p>
                            </button>
                            <button className="flex h-16 w-16 items-center justify-center rounded-full border border-border" onClick={increaseSize}>
                                <TextIncrease />
                            </button>
                        </div>
                    </div>
                    <div className="flex justify-between p-space-04">
                        <div className="flex items-center gap-space-02">
                            <Contrast className="text-primary" />
                            <p className="text-body-01 font-bold text-foreground">{strings.accessibility.colorContrast}</p>
                        </div>
                        <Switch checked={isActive} onCheckedChange={toggleColors} />
                    </div>
                    <div className="flex justify-between p-space-04">
                        <div className="flex items-center gap-space-02">
                            <HearingDisabled className="text-disabled" />
                            <p className="text-body-01 font-bold text-disabled">{strings.accessibility.signLanguage}</p>
                        </div>
                        <OpenInNew className="text-disabled ltr:rotate-180" />
                    </div>
                    <div className="flex justify-between p-space-04">
                        <div className="flex items-center gap-space-02">
                            <Mic className="text-disabled" />
                            <p className="text-body-01 font-bold text-disabled">{strings.accessibility.voiceCommands}</p>
                        </div>
                        <ChevronLeft className="text-disabled ltr:rotate-180" />
                    </div>
                </div>
            </div>
        </div>
    )
}

export default Accessibility
