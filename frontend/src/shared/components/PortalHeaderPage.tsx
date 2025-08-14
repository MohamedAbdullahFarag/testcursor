import { strings } from '@/shared/locales'
import { Breadcrumbs } from 'mada-design-system'
import { ReactElement } from 'react'
import { When } from 'react-if'

interface PortalHeaderPageProps {
    breadcrumbs?: ReactElement
    titlePage: string
    descriptionPage?: string
}

export default function PortalHeaderPage({ titlePage, descriptionPage, breadcrumbs }: PortalHeaderPageProps) {
    return (
        <div className="bg-background-primary-25 px-space-04 py-space-07 xl:px-space-06">
            <div className="mx-auto flex max-w-container flex-col gap-space-04">
                {breadcrumbs ?? (
                    <Breadcrumbs
                        items={[
                            {
                                title: strings.shared.home,
                                path: '/',
                            },
                            { title: titlePage },
                        ]}
                    />
                )}
                <h1 className="title-03 font-bold">{titlePage}</h1>
                <When condition={!!descriptionPage}>
                    <p className="text-body-02">{descriptionPage}</p>
                </When>
            </div>
        </div>
    )
}
