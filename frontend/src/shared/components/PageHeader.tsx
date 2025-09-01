import { GoogleMaterialIcon, MoreVert } from 'google-material-icons/filled'
import { cn, Popover, PopoverContent, PopoverTrigger, Stack } from 'mada-design-system'
import React from 'react'
import { When } from 'react-if'

interface PageHeaderProps extends React.HtmlHTMLAttributes<HTMLDivElement> {
    title: string
    description?: string
    button?: JSX.Element[]
    icon?: GoogleMaterialIcon
    divider?: boolean
    breadCrumb?: JSX.Element
    /** Optional test id applied to the title heading for E2E tests */
    dataTestId?: string
}
const PageHeader = ({ title, description, button, icon: Icon, divider = true, breadCrumb, className, dataTestId, ...props }: PageHeaderProps) => {
    return (
        <Stack direction={'col'} gap={4}>
            {breadCrumb}
            <Stack direction={'col'} gap={2} className={cn({ 'border-b pb-space-04': divider }, className)} {...props}>
                <Stack gap={4} alignItems={'start'} justifyContent={'between'}>
                    <Stack className="flex-col justify-center md:flex-row md:items-center" gap={4} alignItems={'start'}>
                        {Icon && (
                            <div className="rounded-1 bg-primary-container p-space-02 text-primary-oncontainer md:p-space-03">
                                <Icon className="h-space-06 w-space-06 md:h-space-07 md:w-space-07" />
                            </div>
                        )}
                        <Stack direction={'col'} gap={2} justifyItems={'center'}>
                            <h1 className="title-01 font-bold" data-testid={dataTestId}>{title}</h1>
                            <When condition={!!description}>
                                <p className="hidden text-body-02 text-foreground-secondary md:block">{description}</p>
                            </When>
                        </Stack>
                    </Stack>

                    <div className="flex flex-shrink-0 items-start md:mt-space-00 lg:flex-shrink">
                        <div className="flex items-center gap-space-02">
                            <When condition={!!button?.length}>
                                {button?.slice(0, 2).map((btn, i) => <React.Fragment key={'haederBtn' + i}>{btn}</React.Fragment>)}
                                <When condition={button && button?.length > 2}>
                                    <Popover>
                                        <PopoverTrigger asChild>
                                            <button className="flex items-center justify-center" aria-label="Accessibility">
                                                <MoreVert />
                                            </button>
                                        </PopoverTrigger>
                                        <PopoverContent>
                                            {button?.slice(2).map((btn, i) => <div key={'haederBtn' + i + 1}>{btn}</div>)}
                                        </PopoverContent>
                                    </Popover>
                                </When>
                            </When>
                        </div>
                    </div>
                </Stack>

                <p className="text-body-02 text-foreground-secondary md:hidden">{description}</p>
            </Stack>
        </Stack>
    )
}

export default PageHeader
